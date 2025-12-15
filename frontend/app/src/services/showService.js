import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5125/api';

const getAuthHeader = () => {
  const token = localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

const showService = {
  getAllShows: async () => {
    try {
      const response = await axios.get(`${API_URL}/Shows`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  getShowById: async (id) => {
    try {
      const response = await axios.get(`${API_URL}/Shows/${id}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  createShow: async (showData) => {
    try {
      const response = await axios.post(`${API_URL}/Shows`, {
        title: showData.title,
        ticketPrice: parseFloat(showData.ticketPrice),
        singer: showData.singer,
        presentationDate: new Date(showData.presentationDate).toISOString(),
        maxTicketQuantity: parseInt(showData.maxTicketQuantity),
      }, {
        headers: getAuthHeader(),
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  updateShow: async (id, showData) => {
    try {
      const response = await axios.put(`${API_URL}/Shows/${id}`, {
        title: showData.title,
        ticketPrice: parseFloat(showData.ticketPrice),
        singer: showData.singer,
        presentationDate: showData.presentationDate,
        maxTicketQuantity: parseInt(showData.maxTicketQuantity),
      }, {
        headers: getAuthHeader(),
      });

      return response.data;
    } catch (error) {
      throw error;
    }
  },

  deleteShow: async (id) => {
    try {
      const response = await axios.delete(`${API_URL}/Shows/${id}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default showService;
