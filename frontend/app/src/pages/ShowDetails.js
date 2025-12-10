import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import showService from '../services/showService';
import '../styles/Shows.css';

function ShowDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [show, setShow] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({});
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    fetchShowDetails();
  }, [id]);

  const fetchShowDetails = async () => {
    try {
      setLoading(true);
      const data = await showService.getShowById(id);
      setShow(data);
      setFormData(data);
      setError('');
    } catch (err) {
      setError('Erro ao carregar os detalhes do show.');
      console.error('Erro ao buscar show:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setError('');

    // Validações
    if (
      !formData.title ||
      !formData.singer ||
      !formData.ticketPrice ||
      !formData.presentationDate ||
      !formData.maxTicketQuantity
    ) {
      setError('Todos os campos são obrigatórios');
      return;
    }

    if (parseFloat(formData.ticketPrice) <= 0) {
      setError('O preço do ingresso deve ser maior que zero');
      return;
    }

    if (parseInt(formData.maxTicketQuantity) <= 0) {
      setError('A quantidade de ingressos deve ser maior que zero');
      return;
    }

    try {
      setIsSaving(true);
      const updatedShow = await showService.updateShow(id, formData);
      setShow(updatedShow);
      setFormData(updatedShow);
      setIsEditing(false);
    } catch (err) {
      setError('Erro ao atualizar show. Tente novamente.');
      console.error('Erro ao atualizar show:', err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (
      window.confirm(
        'Tem certeza que deseja deletar este show? Esta ação não pode ser desfeita.'
      )
    ) {
      try {
        await showService.deleteShow(id);
        navigate('/shows');
      } catch (err) {
        setError('Erro ao deletar show. Tente novamente.');
        console.error('Erro ao deletar show:', err);
      }
    }
  };

  const handleBuyTicket = () => {
    navigate(`/payment/${id}`);
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

  const formatDateTimeLocal = (dateString) => {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  if (loading) {
    return <div className="show-details-container">Carregando...</div>;
  }

  if (!show) {
    return (
      <div className="show-details-container">
        <div className="error-message">Show não encontrado.</div>
        <button className="btn-secondary" onClick={() => navigate('/shows')}>
          Voltar para Shows
        </button>
      </div>
    );
  }

  return (
    <div className="show-details-container">
      <div className="details-header">
        <button className="btn-back" onClick={() => navigate('/shows')}>
          ← Voltar
        </button>
        <h1>{show.title}</h1>
        <div className="header-actions">
          {!isEditing && (
            <>
              <button
                className="btn-success"
                onClick={handleBuyTicket}
              >
                Comprar Ingresso
              </button>
              <button
                className="btn-primary"
                onClick={() => setIsEditing(true)}
              >
                Editar
              </button>
              <button
                className="btn-danger"
                onClick={handleDelete}
              >
                Deletar
              </button>
            </>
          )}
        </div>
      </div>

      {error && <div className="error-message">{error}</div>}

      {isEditing ? (
        <form onSubmit={handleSave} className="edit-form">
          <div className="form-group">
            <label htmlFor="title">Título do Show *</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              disabled={isSaving}
            />
          </div>

          <div className="form-group">
            <label htmlFor="singer">Artista *</label>
            <input
              type="text"
              id="singer"
              name="singer"
              value={formData.singer}
              onChange={handleChange}
              disabled={isSaving}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="ticketPrice">Preço do Ingresso (R$) *</label>
              <input
                type="number"
                id="ticketPrice"
                name="ticketPrice"
                value={formData.ticketPrice}
                onChange={handleChange}
                step="0.01"
                min="0"
                disabled={isSaving}
              />
            </div>

            <div className="form-group">
              <label htmlFor="maxTicketQuantity">Quantidade de Ingressos *</label>
              <input
                type="number"
                id="maxTicketQuantity"
                name="maxTicketQuantity"
                value={formData.maxTicketQuantity}
                onChange={handleChange}
                min="1"
                disabled={isSaving}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="presentationDate">Data e Hora da Apresentação *</label>
            <input
              type="datetime-local"
              id="presentationDate"
              name="presentationDate"
              value={formatDateTimeLocal(formData.presentationDate)}
              onChange={(e) => {
                const dateString = e.target.value;
                setFormData((prev) => ({
                  ...prev,
                  presentationDate: new Date(dateString).toISOString(),
                }));
              }}
              disabled={isSaving}
            />
          </div>

          <div className="form-actions">
            <button
              type="button"
              className="btn-secondary"
              onClick={() => {
                setIsEditing(false);
                setFormData(show);
              }}
              disabled={isSaving}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn-primary"
              disabled={isSaving}
            >
              {isSaving ? 'Salvando...' : 'Salvar Alterações'}
            </button>
          </div>
        </form>
      ) : (
        <div className="show-details-view">
          <div className="details-card">
            <div className="details-section">
              <h3>Informações do Show</h3>
              <div className="detail-item">
                <span className="label">Artista:</span>
                <span className="value">{show.singer}</span>
              </div>
              <div className="detail-item">
                <span className="label">Data e Hora:</span>
                <span className="value">{formatDate(show.presentationDate)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Preço do Ingresso:</span>
                <span className="value">{formatCurrency(show.ticketPrice)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Quantidade de Ingressos:</span>
                <span className="value">{show.maxTicketQuantity}</span>
              </div>
              <div className="detail-item">
                <span className="label">ID:</span>
                <span className="value">{show.id}</span>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default ShowDetails;
