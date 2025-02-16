import React, { useState } from 'react';
import axios from 'axios';

const Login = ({ setAuthToken, setIsLoggedIn }) => {
  const [name, setName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:5240/api/filmstudio/authenticate', {
        name: name,
        password: password,
      });
      setAuthToken(response.data.token);
      setIsLoggedIn(true);
      setError('');
    } catch (err) {
      setError('Invalid name or password');
    }
  };

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <div>
          <label>Name:</label>
          <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div>
          <label>Password:</label>
          <input type="" value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        <button type="submit">Login</button>
      </form>
      {error && <p>{error}</p>}
    </div>
  );
};

export default Login;