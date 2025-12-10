import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import showService from '../services/showService';
import CreateShowModal from '../components/CreateShowModal';
import '../styles/Shows.css';

function ShowsList() {
  const [shows, setShows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    fetchShows();
  }, []);

  const fetchShows = async () => {
    try {
      setLoading(true);
      const data = await showService.getAllShows();
      setShows(data);
      setError('');
    } catch (err) {
      setError('Erro ao carregar shows. Tente novamente mais tarde.');
      console.error('Erro ao buscar shows:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Tem certeza que deseja deletar este show?')) {
      try {
        await showService.deleteShow(id);
        setShows(shows.filter((show) => show.id !== id));
      } catch (err) {
        setError('Erro ao deletar show. Tente novamente.');
        console.error('Erro ao deletar show:', err);
      }
    }
  };

  const handleShowAdded = () => {
    setIsModalOpen(false);
    fetchShows();
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  if (loading) {
    return <div className="shows-container">Carregando shows...</div>;
  }

  return (
    <div className="shows-container">
      <div className="shows-header">
        <h1>Shows</h1>
        <button className="btn-primary" onClick={() => setIsModalOpen(true)}>
          + Novo Show
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      {shows.length === 0 ? (
        <div className="empty-state">
          <p>Nenhum show cadastrado ainda.</p>
          <button className="btn-primary" onClick={() => setIsModalOpen(true)}>
            Criar Primeiro Show
          </button>
        </div>
      ) : (
        <div className="shows-grid">
          {shows.map((show) => (
            <div key={show.id} className="show-card">
              <div className="show-card-header">
                <h3>{show.title}</h3>
                <span className="show-artist">{show.singer}</span>
              </div>

              <div className="show-card-body">
                <div className="show-info">
                  <p>
                    <strong>Data:</strong> {formatDate(show.presentationDate)}
                  </p>
                  <p>
                    <strong>Preço:</strong> {formatCurrency(show.ticketPrice)}
                  </p>
                  <p>
                    <strong>Ingressos Disponíveis:</strong>{' '}
                    {show.maxTicketQuantity}
                  </p>
                </div>
              </div>

              <div className="show-card-footer">
                <button
                  className="btn-secondary"
                  onClick={() => navigate(`/shows/${show.id}`)}
                >
                  Ver Detalhes
                </button>
                <button
                  className="btn-danger"
                  onClick={() => handleDelete(show.id)}
                >
                  Deletar
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {isModalOpen && (
        <CreateShowModal
          onClose={() => setIsModalOpen(false)}
          onShowAdded={handleShowAdded}
        />
      )}
    </div>
  );
}

export default ShowsList;
