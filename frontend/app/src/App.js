import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Sidebar from './components/Sidebar';
import Register from './pages/Register';
import Login from './pages/Login';
import Home from './pages/Home';
import Shows from './pages/Shows';
import ShowDetails from './pages/ShowDetails';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/register" element={<Register />} />
          <Route path="/login" element={<Login />} />
          <Route
            path="/*"
            element={
              <ProtectedRoute>
                <div className="app-layout">
                  <Sidebar />
                  <main className="app-content">
                    <Routes>
                      <Route path="/home" element={<Home />} />
                      <Route path="/shows" element={<Shows />} />
                      <Route path="/shows/:id" element={<ShowDetails />} />
                      <Route path="/tickets" element={<div>Página de Ingressos - Em construção</div>} />
                      <Route path="/" element={<Navigate to="/shows" replace />} />
                    </Routes>
                  </main>
                </div>
              </ProtectedRoute>
            }
          />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
