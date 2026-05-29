import React, { useState, useEffect } from 'react';

const LoaderComponent = () => {
    return (
        <div className="loader-overlay">
            <div className="loader">
                <img src={require("../../assets/loader-logo.png")} alt="Loading..." className="loader-logo" />
                <div className="loader-ring"></div>
            </div>
        </div>
    );
};

export default LoaderComponent;