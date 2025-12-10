import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import '../styles/Sidebar.css';

function Sidebar() {
  const [isOpen, setIsOpen] = useState(true);
  const location = useLocation();

  const isActive = (path) => {
    return location.pathname.startsWith(path);
  };

  const toggleSidebar = () => {
    setIsOpen(!isOpen);
  };

  return (
    <>
      <button className="sidebar-toggle" onClick={toggleSidebar}>
        {isOpen ? '✕' : '☰'}
      </button>

      <aside className={`sidebar ${isOpen ? 'open' : 'closed'}`}>
        <div className="sidebar-header">
          <h2>TicketMonster</h2>
        </div>

        <nav className="sidebar-nav">
          <Link
            to="/shows"
            className={`nav-item ${isActive('/shows') ? 'active' : ''}`}
            onClick={() => window.innerWidth < 768 && setIsOpen(false)}
          >
            <span className="nav-icon">🎭</span>
            <span className="nav-label">Shows</span>
          </Link>

          <Link
            to="/tickets"
            className={`nav-item ${isActive('/tickets') ? 'active' : ''}`}
            onClick={() => window.innerWidth < 768 && setIsOpen(false)}
          >
            <span className="nav-icon">🎫</span>
            <span className="nav-label">Ingressos</span>
          </Link>

          <Link
            to="/home"
            className={`nav-item ${isActive('/home') ? 'active' : ''}`}
            onClick={() => window.innerWidth < 768 && setIsOpen(false)}
          >
            <span className="nav-icon">🏠</span>
            <span className="nav-label">Home</span>
          </Link>
        </nav>

        <div className="sidebar-footer">
          <Link to="/login" className="nav-item logout">
            <span className="nav-icon">🚪</span>
            <span className="nav-label">Sair</span>
          </Link>
        </div>
      </aside>
    </>
  );
}

export default Sidebar;
