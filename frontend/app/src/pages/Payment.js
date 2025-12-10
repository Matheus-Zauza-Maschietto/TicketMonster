import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { loadStripe } from '@stripe/stripe-js';
import {
  EmbeddedCheckoutProvider,
  EmbeddedCheckout
} from '@stripe/react-stripe-js';
import '../styles/Payment.css';

const stripePromise = loadStripe(process.env.REACT_APP_STRIPE_PUBLIC_KEY || '');

export default function Payment() {
  const navigate = useNavigate();
  const location = useLocation();
  const { showId } = useParams();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const clientSecret = location.state?.clientSecret;

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
