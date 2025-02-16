import React, { useState, useEffect, useContext } from 'react';
import axios from 'axios';
import { AuthContext } from './AuthContext';

const RentedFilms = () => {
  const { authToken } = useContext(AuthContext);
  const [rentedFilms, setRentedFilms] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchRentedFilms = async () => {
      try {
        const response = await axios.get('http://localhost:5240/api/mystudio/rentals', {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        });
        setRentedFilms(response.data);
      } catch (err) {
        setError('Failed to fetch rented films');
      }
    };

    fetchRentedFilms();
  }, [authToken]);

  return (
    <div>
      <h2>Rented Films</h2>
      {error && <p>{error}</p>}
      <ul>
        {rentedFilms.map((film) => (
          <li key={film.id}>
            <h3>{film.title}</h3>
            <p>{film.description}</p>
            <p>Genre: {film.genre}</p>
            <p>Release Year: {film.releaseYear}</p>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default RentedFilms;