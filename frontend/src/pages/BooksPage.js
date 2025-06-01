// src/pages/BooksPage.js
import React, { useState, useEffect } from 'react';
import { fetchBooks, issueLoan } from '../services/apiService';
import { useNavigate } from 'react-router-dom'; // For redirecting if not authenticated


function BooksPage() {
    const [books, setBooks] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');
    const [loanBookId, setLoanBookId] = useState(''); // For the issue loan input
    const [borrowerId, setBorrowerId] = useState(''); // For the issue loan input
    const [loanMessage, setLoanMessage] = useState('');
    const navigate = useNavigate();
    

    useEffect(() => {
        const loadBooks = async () => {
            try {
                const token = localStorage.getItem('jwtToken');
                if (!token) {
                    navigate('/login'); // Redirect if no token
                    return;
                }
                const data = await fetchBooks();
                setBooks(data);
            } catch (err) {
                console.error('Failed to fetch books:', err);
                setError('Failed to load books. Ensure you are logged in or try again later.');
                if (err.response && err.response.status === 401) {
                     navigate('/login'); // Unauthorized
                }
            } finally {
                setIsLoading(false);
            }
        };
        loadBooks();
    }, [navigate]);

    const handleIssueLoan = async (bookIdToLoan) => {
        if (!borrowerId) {
            setLoanMessage('Please enter a Borrower ID.');
            return;
        }
        setLoanMessage('');
        try {
            // Assuming your API expects { bookId, borrowerId }
            const result = await issueLoan({ bookId: bookIdToLoan, borrowerId });
            setLoanMessage(`Book (ID: ${bookIdToLoan}) issued successfully! Due Date: ${result.dueDate || 'N/A'}`);
            // Optionally, refresh books list or update specific book's available copies
            const updatedBooks = await fetchBooks();
            setBooks(updatedBooks);
            setBorrowerId(''); // Clear borrowerId input
        } catch (err) {
            console.error('Failed to issue loan:', err);
            setLoanMessage(`Failed to issue loan: ${err.response?.data?.message || err.message}`);
        }
    };

    if (isLoading) return <p>Loading books...</p>;
    if (error) return <p style={{ color: 'red' }}>{error}</p>;

    return (
        <div>
            <h2>Available Books</h2>
            {loanMessage && <p style={{ color: loanMessage.startsWith('Failed') ? 'red' : 'green' }}>{loanMessage}</p>}
            <div>
                <label>Borrower ID for Loan: </label>
                <input
                    type="text"
                    value={borrowerId}
                    onChange={(e) => setBorrowerId(e.target.value)}
                    placeholder="Enter Borrower ID"
                />
            </div>
            <ul style={{ listStyle: 'none', padding: 0 }}>
                {books.map(book => (
                    <li key={book.id} style={{ border: '1px solid #ccc', margin: '10px', padding: '10px' }}>
                        <h3>{book.title}</h3>
                        <p>Author: {book.author}</p>
                        <p>ISBN: {book.isbn}</p>
                        <p>Available Copies: {book.availableCopies}</p>
                        <button
                            onClick={() => handleIssueLoan(book.id)}
                            disabled={book.availableCopies <= 0 || !borrowerId}
                        >
                            Issue Loan
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
}
export default BooksPage;