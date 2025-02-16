import React, {useState, useEffect} from "react";
import axios from "axios";

const Films = () => {
    const [films, setFilms] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchFilms = async () => {
            try {
                const response = await axios.get('http://localhost:5240/api/films');
                setFilms(response.data);
            } catch (err) {
                setError("failed to fetch films");
            }
        };
        fetchFilms();
    }, []);

    return (
        <div>
          <h2>Available Films</h2>
          {error && <p>{error}</p>}
          <ul>
            {films.map((film) => (
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
    
    export default Films;
