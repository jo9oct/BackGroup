// src/pages/ReturnPage.js
import React, { useState, useEffect } from 'react';
import { returnBook } from '../services/apiService';
import { useNavigate } from 'react-router-dom';

function ReturnPage() {
    const [identifier, setIdentifier] = useState(''); // Could be bookId or loanId
    const [message, setMessage] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('jwtToken');
        if (!token) {
            navigate('/login'); // Redirect if no token
        }
    }, [navigate]);

    const handleReturn = async (e) => {
        e.preventDefault();
        setMessage('');
        if (!identifier) {
            setMessage('Please enter a Book ID or Loan ID to return.');
            return;
        }
        try {
            // Adjust the payload based on what your `/api/returns` endpoint expects
            // Assuming it expects { bookId: '...' } or { loanId: '...' }
            // For this minimal example, let's assume it takes a bookId.
            await returnBook({ bookId: identifier }); // Or { loanId: identifier }
            setMessage(`Book (ID: ${identifier}) returned successfully!`);
            setIdentifier(''); // Clear input
        } catch (err) {
            console.error('Failed to return book:', err);
            setMessage(`Failed to return book: ${err.response?.data?.message || err.message}`);
        }
    };

    return (
        <div>
            <h2>Return a Book</h2>
            <form onSubmit={handleReturn}>
                <div>
                    <label>Book ID (or Loan ID) to Return:</label>
                    <input
                        type="text"
                        value={identifier}
                        onChange={(e) => setIdentifier(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Return Book</button>
            </form>
            {message && <p style={{ color: message.startsWith('Failed') ? 'red' : 'green' }}>{message}</p>}
        </div>
    );
}

export default ReturnPage;
