import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { loadStripe } from '@stripe/stripe-js';
import {
  EmbeddedCheckoutProvider,
  EmbeddedCheckout
} from '@stripe/react-stripe-js';
import '../styles/Payment.css';

const stripePromise = loadStripe(process.env.REACT_APP_STRIPE_PUBLIC_KEY || '');

export default function Payment() {
  const { showId } = useParams();
  const navigate = useNavigate();
  const [clientSecret, setClientSecret] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    // Create a payment intent on the server
    const createPaymentIntent = async () => {
      try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${process.env.REACT_APP_API_URL || 'http://localhost:5125'}/api/Payment/create-intent`, {
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
          throw new Error('Failed to create payment intent');
        }

        const data = await response.json();
        setClientSecret(data.clientSecret);
      } catch (err) {
        setError(err.message || 'An error occurred');
        console.error('Payment Intent Error:', err);
      } finally {
        setLoading(false);
      }
    };

    createPaymentIntent();
  }, [showId]);

  if (loading) {
    return <div className="payment-container"><p>Loading payment form...</p></div>;
  }

  if (error) {
    return (
      <div className="payment-container">
        <div className="error-message">
          <p>Error: {error}</p>
          <button onClick={() => navigate(-1)}>Go Back</button>
        </div>
      </div>
    );
  }

  if (!clientSecret) {
    return <div className="payment-container"><p>Unable to load payment form</p></div>;
  }

  return (
    <div className="payment-container">
      <div className="payment-content">
        <h1>Complete Your Ticket Purchase</h1>
        <EmbeddedCheckoutProvider stripe={stripePromise} options={{ clientSecret }}>
          <EmbeddedCheckout />
        </EmbeddedCheckoutProvider>
      </div>
    </div>
  );
}
