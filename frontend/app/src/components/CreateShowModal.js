import React, { useState } from 'react';
import showService from '../services/showService';
import '../styles/Shows.css';

function CreateShowModal({ onClose, onShowAdded }) {
  const [formData, setFormData] = useState({
    title: '',
    singer: '',
    ticketPrice: '',
    presentationDate: '',
    maxTicketQuantity: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
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
      setLoading(true);
      await showService.createShow(formData);
      onShowAdded();
    } catch (err) {
      setError('Erro ao criar show. Tente novamente.');
      console.error('Erro ao criar show:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Novo Show</h2>
          <button className="btn-close" onClick={onClose}>
            ✕
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit} className="modal-form">
          <div className="form-group">
            <label htmlFor="title">Título do Show *</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              placeholder="Ex: Rock ao Vivo"
              disabled={loading}
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
              placeholder="Ex: The Beatles"
              disabled={loading}
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
                placeholder="0.00"
                step="0.01"
                min="0"
                disabled={loading}
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
                placeholder="100"
                min="1"
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="presentationDate">Data e Hora da Apresentação *</label>
            <input
              type="datetime-local"
              id="presentationDate"
              name="presentationDate"
              value={formData.presentationDate}
              onChange={handleChange}
              disabled={loading}
            />
          </div>

          <div className="modal-footer">
            <button
              type="button"
              className="btn-secondary"
              onClick={onClose}
              disabled={loading}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn-primary"
              disabled={loading}
            >
              {loading ? 'Criando...' : 'Criar Show'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateShowModal;
