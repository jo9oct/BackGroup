// src/App.js
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import BooksPage from './pages/BooksPage';
import ReturnPage from './pages/ReturnPage';
import './App.css'; // For basic styling

function App() {
    const token = localStorage.getItem('jwtToken');

    const handleLogout = () => {
        localStorage.removeItem('jwtToken');
        // Force re-render or redirect to login, window.location.href is a simple way
        window.location.href = '/login';
    };

    return (
        <Router>
            <div>
                <nav>
                    <ul>
                        <li><Link to="/login">Login</Link></li>
                        {token && <li><Link to="/books">Books</Link></li>}
                        {token && <li><Link to="/return">Return Book</Link></li>}
                        {token && <li><button onClick={handleLogout}>Logout</button></li>}
                    </ul>
                </nav>
                <hr />
                <div className="content">
                    <Routes>
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/books" element={token ? <BooksPage /> : <Navigate to="/login" />} />
                        <Route path="/return" element={token ? <ReturnPage /> : <Navigate to="/login" />} />
                        <Route path="/" element={token ? <Navigate to="/books" /> : <Navigate to="/login" />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;