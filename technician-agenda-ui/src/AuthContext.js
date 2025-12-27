import React, { createContext, useState, useContext, useEffect } from 'react';
import axios from 'axios';

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem('token'));
    const [loading, setLoading] = useState(true);

    const API_URL = 'https://technicianagenda-production.up.railway.app';

    useEffect(() => {
        if (token) {
            localStorage.setItem('token', token);
            // Podrías decodificar el JWT aquí para obtener info del usuario
        } else {
            localStorage.removeItem('token');
        }
        setLoading(false);
    }, [token]);

    const login = async (username, password) => {
        try {
            const response = await axios.post(`${API_URL}/api/auth/login`, {
                username,
                password
            });
            setToken(response.data.token);
            setUser({
                username: response.data.username,
                email: response.data.email,
                fullName: response.data.fullName
            });
            return { success: true };
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Error al iniciar sesión'
            };
        }
    };

    const register = async (username, email, password, fullName) => {
        try {
            const response = await axios.post(`${API_URL}/api/auth/register`, {
                username,
                email,
                password,
                fullName
            });
            setToken(response.data.token);
            setUser({
                username: response.data.username,
                email: response.data.email,
                fullName: response.data.fullName
            });
            return { success: true };
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Error al registrarse'
            };
        }
    };

    const logout = () => {
        setToken(null);
        setUser(null);
        localStorage.removeItem('token');
    };

    return (
        <AuthContext.Provider value={{ user, token, login, register, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
};