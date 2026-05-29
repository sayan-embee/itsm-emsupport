import React, { useState, useEffect } from 'react';

const Loader = () => {
    return (
        <div className="loader-overlay">
            <div className="loader">
                <img src={require("../../assets/logo.png")} alt="Loading..." className="loader-logo" />
                <div className="loader-ring"></div>
            </div>
        </div>
    );
};

export default Loader;