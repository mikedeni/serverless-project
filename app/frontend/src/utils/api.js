import axios from 'axios';

const api = axios.create({
  baseURL: '/api'
});

// Automatically inject JWT Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Standardize error handling
api.interceptors.response.use(
  (response) => response.data, // Only return the data payload
  (error) => {
    if (error.response && error.response.status === 401 && !error.config.url.includes('/auth/login')) {
      localStorage.clear();
      window.location.href = '/login';
    }
    const message = error.response?.data?.message || error.message || "An error occurred";
    return Promise.reject(new Error(message));
  }
);

export default api;
