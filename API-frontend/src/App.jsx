import React, { useState, useContext } from 'react';
import { AuthContext } from './AuthContext';
import Login from './Login';
import Logout from './Logout';
import Films from './Films';
import RentedFilms from './RentedFilms';

const App = () => {
  const { authToken, setAuthToken } = useContext(AuthContext);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <div>
      <h1>Film Studio Client</h1>
      {isLoggedIn ? (
        <>
          <Logout setAuthToken={setAuthToken} setIsLoggedIn={setIsLoggedIn} />
          <RentedFilms />
          <Films />
        </>
      ) : (
        <Login setAuthToken={setAuthToken} setIsLoggedIn={setIsLoggedIn} />
      )}
    </div>
  );
};

export default App;