import React, { useState, useEffect, useRef } from 'react';
import { Message } from 'primereact/message';
import { CommonMessage } from './ToastComponent';

import accessDeniedImage from "../../assets/expired.svg";
import { logoutAPI } from '../../apis/APIList';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { ROUTE_PATH } from '../../router';
import LoaderComponent from './LoaderComponent';

// interface SessionExpiredComponentProps {
//     message?: string;
//     severity: 'warn' | 'error';
// }

const SessionExpiredComponent: React.FC = () => {
    const navigate = useNavigate();
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const signOut = useAuthStore((state) => state.signOut);

    const isMountedRef = useRef(true);
    const [countdown, setCountdown] = useState(5);

    const handleCountdownComplete = () => {
        console.log("Countdown complete! Calling function...");
        logoutAPI();
        signOut();
        navigate(ROUTE_PATH.SIGN_IN);
    };

    useEffect(() => {
        window.history.pushState(null, "", window.location.href);
        window.onpopstate = () => {
            window.history.pushState(null, "", window.location.href);
        };
    }, []);

    useEffect(() => {
        if (hasHydrated) {
            isMountedRef.current = true;
        }
    }, [hasHydrated]);

    useEffect(() => {
        if (countdown > 0) {
            const timer = setInterval(() => {
                setCountdown(prevCount => prevCount - 1);
            }, 1000);

            return () => clearInterval(timer);
        } else {
            handleCountdownComplete();
        }
    }, [countdown]);

    return (
        <div className="overlay-container">
            {!hasHydrated && <LoaderComponent />}
            <img className="error-image" src={accessDeniedImage} alt="Session Expired" />
            <div className="mt-4">
                <Message severity='error' text={`${CommonMessage.SessionExpired} Redirecting in ${countdown}...`} />
            </div>
        </div>
    );
};

export default SessionExpiredComponent;