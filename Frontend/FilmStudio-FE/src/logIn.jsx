import React, { useState } from 'react';
import './Login.css'; // Optional: If you want to use CSS for styling

const Login = ({ hasAccount, onToggle }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!username || !password) {
      setError('Please fill in all fields.');
      return;
    }

    // Simulate a login process (replace this with your actual API call)
    if (username === 'admin' && password === 'password') {
      setError('');
      alert('Login successful!');
      // Redirect to another page or perform other actions
    } else {
      setError('Invalid username or password.');
    }
  };

  return (
    <div className="login-container">
      <h2>{hasAccount ? 'Login' : 'Sign Up'}</h2>

      {/* Show a message if hasAccount is true */}
      {hasAccount && <p>You already have an account. Please log in.</p>}

      {/* Render the form regardless of hasAccount */}
      {!hasAccount && (
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label htmlFor="username">Username:</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Enter your username"
            />
          </div>

          <div className="input-group">
            <label htmlFor="password">Password:</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
            />
          </div>

          {error && <p className="error-message">{error}</p>}

          <button type="submit">{hasAccount ? 'Login' : 'Sign Up'}</button>
        </form>
      )}

      {/* Add the "Don't have an account?" link */}
      <p>
        {hasAccount
          ? "Don't have an account yet? "
          : 'Already have an account? '}
        <span className="toggle-link" onClick={() => onToggle()}>
          {hasAccount ? 'Sign up' : 'Log in'}
        </span>
      </p>
    </div>
  );
};

export default Login;