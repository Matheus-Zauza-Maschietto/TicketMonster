const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5125';

export const ticketService = {
  // Get all tickets for the current user
  getAllTickets: async () => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/api/Tickets`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch tickets');
    }

    return response.json();
  },

  // Get a single ticket by ID
  getTicketById: async (id) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/api/Tickets/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch ticket');
    }

    return response.json();
  },

  // Create a new ticket
  createTicket: async (showId) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/api/Tickets`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify({
        showId: showId,
      }),
    });

    if (!response.ok) {
      throw new Error('Failed to create ticket');
    }

    return response.json();
  },

  // Delete a ticket
  deleteTicket: async (id) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/api/Tickets/${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to delete ticket');
    }

    return response.json();
  },
};
