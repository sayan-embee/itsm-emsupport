import React, { useState, useEffect } from 'react';
import { Message } from 'primereact/message';
import { useLocation, useNavigate } from 'react-router-dom';

import accessDeniedImage from "../../assets/access_denied.jpg";
import { Button } from 'primereact/button';
import { HOME_ROUTE_PATH, ROUTE_PATH } from '../../router';

interface UnauthorizedComponentProps {
    message?: string;
    severity: 'warn' | 'error';
    redirect?: boolean;
}

const UnauthorizedComponent: React.FC<UnauthorizedComponentProps> = ({ message, severity, redirect = true }) => {
    const location = useLocation();
    const navigate = useNavigate();

    const [countdown, setCountdown] = useState(5);

    // useEffect(() => {
    //     if (redirect && countdown > 0) {
    //         const timer = setInterval(() => {
    //             setCountdown(prevCount => prevCount - 1);
    //         }, 1000);

    //         return () => clearInterval(timer);
    //     } else {
    //         handleCountdownComplete();
    //     }
    // }, [redirect, countdown]);

    const handleCountdownComplete = () => {
        // console.log("Countdown complete! Calling function...");
        // navigate(-1);
        window.location.replace(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD);
    };

    return (
        // <div className="d-flex flex-column justify-content-center align-items-center vh-100">
        <div className="overlay-container">
            <img className="error-image" src={accessDeniedImage} alt="Access Denied" />
            {message && (
                <>
                    <div className="mt-3">
                        {/* <Message severity={severity} text={`${message} Redirecting in ${countdown}...`} /> */}
                        <Message severity={severity} text={`${message}`} />
                    </div>
                    <div className="mt-2">
                        <Button className="btn btn-danger" onClick={() => handleCountdownComplete()}
                            icon='pi pi-arrow-left'
                            label='Go Back'
                        />
                    </div>
                </>
            )}
        </div>
    );
};

export default UnauthorizedComponent;