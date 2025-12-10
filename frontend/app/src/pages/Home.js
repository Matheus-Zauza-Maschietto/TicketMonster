import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/Home.css';

const Home = () => {
  const { logout, user } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="home-container">
      <nav className="navbar">
        <div className="nav-content">
          <h1 className="nav-title">TicketMonster</h1>
          <div className="nav-actions">
            {user && <span className="user-info">Bem-vindo, {user.email}!</span>}
            <button onClick={handleLogout} className="btn-logout">
              Sair
            </button>
          </div>
        </div>
      </nav>

      <main className="home-content">
        <div className="welcome-section">
          <h2>Bem-vindo ao TicketMonster</h2>
          <p>Aqui você poderá gerenciar seus ingressos de shows</p>
        </div>
      </main>
    </div>
  );
};

export default Home;
