import React, { createContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../utils/api';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    // Check if token exists in local storage on initialization
    const token = localStorage.getItem('token');
    const storedUserId = localStorage.getItem('userId');
    const storedCompanyId = localStorage.getItem('companyId');
    const storedRole = localStorage.getItem('role');

    if (token) {
      setUser({ userId: storedUserId, companyId: storedCompanyId, role: storedRole });
      setIsAuthenticated(true);
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const data = await api.post('/auth/login', { email, password });
      
      saveSession(data);
      navigate('/dashboard');
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const register = async (name, companyName, email, password) => {
    try {
      const data = await api.post('/auth/register', { name, companyName, email, password });

      saveSession(data);
      navigate('/dashboard');
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const saveSession = (data) => {
    localStorage.setItem('token', data.token);
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('companyId', data.companyId);
    localStorage.setItem('role', data.role);
    setUser({ userId: data.userId, companyId: data.companyId, role: data.role });
    setIsAuthenticated(true);
  };

  const logout = () => {
    localStorage.clear();
    setUser(null);
    setIsAuthenticated(false);
    navigate('/login');
  };

  if (loading) return <div className="loader-container"><div className="spinner"></div></div>;

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
