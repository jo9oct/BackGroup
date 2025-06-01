// src/services/apiService.js
import axios from 'axios';

const API_URL = 'https://localhost:7123/api'; // Replace with your actual backend URL

const getAuthHeaders = () => {
    const token = localStorage.getItem('jwtToken');
    return token ? { Authorization: `Bearer ${token}` } : {};
};

export const loginUser = async (credentials) => {
    // credentials: { username: 'user', password: 'password' } - adjust to your backend
    const response = await axios.post(`${API_URL}/auth/login`, credentials); // Adjust endpoint
    return response.data; // Assuming backend returns { token: '...' }
};

export const fetchBooks = async () => {
    const response = await axios.get(`${API_URL}/books`, { headers: getAuthHeaders() });
    return response.data;
};

export const issueLoan = async (loanData) => {
    // loanData: { bookId: '...', borrowerId: '...' } - adjust to your backend needs
    const response = await axios.post(`${API_URL}/loans`, loanData, { headers: getAuthHeaders() });
    return response.data;
};

export const returnBook = async (returnData) => {
    // returnData: { bookId: '...' } or { loanId: '...' } - adjust to your backend needs
    const response = await axios.post(`${API_URL}/returns`, returnData, { headers: getAuthHeaders() });
    return response.data;
};

// Add other API functions as needed (register, getBookById, etc.)