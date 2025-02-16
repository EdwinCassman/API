import React from 'react';

const Logout = ({ setAuthToken }) => {
  const handleLogout = () => {
    setAuthToken(null);
  };

  return (
    <div>
        <button onClick={handleLogout}>Logout</button>
    </div>
  );
};

export default Logout;