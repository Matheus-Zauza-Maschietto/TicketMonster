import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { loadStripe } from '@stripe/stripe-js';
import {
  Elements,
  PaymentElement,
  useStripe,
  useElements
} from '@stripe/react-stripe-js';
import '../styles/Payment.css';

const stripePromise = loadStripe(process.env.REACT_APP_STRIPE_PUBLIC_KEY || 'pk_test_51ScvBuI9Cljh3iELZQa6ysLHfsPYgmxU3jZkLb9NSENlW7zjPTlVSgt9hDx238DoWnZhZAS8qb6g6DyIxAWKJ1Wn00KAYJh5Hs');

const CheckoutForm = ({ clientSecret }) => {
  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!stripe || !elements) {
      return;
    }

    setLoading(true);

    const { error, paymentIntent } = await stripe.confirmPayment({
      elements,
      confirmParams: {
        return_url: `${window.location.origin}/tickets`,
      },
    });

    if (error) {
      setErrorMessage(error.message || 'Payment failed');
      console.error('Payment error:', error);
    } else if (paymentIntent?.status === 'succeeded') {
      console.log('Payment successful:', paymentIntent);
      navigate('/tickets');
    }

    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit} className="payment-form">
      <PaymentElement />
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      <button 
        type="submit" 
        disabled={!stripe || loading}
        className="btn-primary"
      >
        {loading ? 'Processing...' : 'Pay Now'}
      </button>
    </form>
  );
};

export default function Payment() {
  const navigate = useNavigate();
  const location = useLocation();
  const { showId } = useParams();
  const [error, setError] = useState('');
  const clientSecret = location.state?.clientSecret;

  useEffect(() => {
    if (!clientSecret) {
      setError('Unable to load payment form. Missing payment configuration.');
    } else if (!clientSecret.startsWith('pi_')) {
      setError('Invalid payment configuration. Expected PaymentIntent.');
    }
  }, [clientSecret]);

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

  const options = {
    clientSecret: clientSecret,
  };

  return (
    <div className="payment-container">
      <div className="payment-content">
        <h1>Complete Your Ticket Purchase</h1>
        <Elements stripe={stripePromise} options={options}>
          <CheckoutForm clientSecret={clientSecret} />
        </Elements>
      </div>
    </div>
  );
}
