import React, { useState, useEffect } from 'react';
import { ticketService } from '../services/ticketService';
import '../styles/Tickets.css';

export default function Tickets() {
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchTickets();
  }, []);

  const fetchTickets = async () => {
    try {
      setLoading(true);
      const data = await ticketService.getAllTickets();
      setTickets(data);
    } catch (err) {
      setError(err.message || 'Failed to load tickets');
      console.error('Error fetching tickets:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (ticketId) => {
    if (window.confirm('Are you sure you want to delete this ticket?')) {
      try {
        await ticketService.deleteTicket(ticketId);
        setTickets(tickets.filter(ticket => ticket.id !== ticketId));
      } catch (err) {
        setError(err.message || 'Failed to delete ticket');
        console.error('Error deleting ticket:', err);
      }
    }
  };

  if (loading) {
    return (
      <div className="tickets-container">
        <div className="loading">Loading your tickets...</div>
      </div>
    );
  }

  return (
    <div className="tickets-container">
      <div className="tickets-header">
        <h1>My Tickets</h1>
        <p>Manage and view your event tickets</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      {tickets.length === 0 ? (
        <div className="empty-state">
          <h2>No tickets yet</h2>
          <p>You haven't purchased any tickets. Start exploring shows to get your tickets!</p>
        </div>
      ) : (
        <div className="tickets-grid">
          {tickets.map(ticket => (
            <div key={ticket.id} className="ticket-card">
              <div className="ticket-header">
                <div className="ticket-code">
                  <label>Ticket Code</label>
                  <p className="code">{ticket.code}</p>
                </div>
                <div className="ticket-status">
                  <span className={`status-badge status-paid`}>
                    PAID
                  </span>
                </div>
              </div>

              <div className="ticket-details">
                <div className="detail-row">
                  <label>Show ID</label>
                  <p>{ticket.showId}</p>
                </div>
                <div className="detail-row">
                  <label>Payment ID</label>
                  <p>{ticket.paymentId}</p>
                </div>
                <div className="detail-row">
                  <label>Ticket Owner</label>
                  <p>{ticket.userOwnerId || 'Unknown'}</p>
                </div>
              </div>

              <div className="ticket-actions">
                <button
                  className="btn-delete"
                  onClick={() => handleDelete(ticket.id)}
                >
                  Delete Ticket
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
