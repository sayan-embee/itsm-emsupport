import React from 'react';
import { Message } from 'primereact/message'; // Assuming you're using PrimeReact for the Message component
import accessDeniedImage from '../../assets/access_denied.jpg'; // Import image statically

interface ErrorComponentProps {
    message?: string;
    severity?: 'warn' | 'error';
}

const ErrorComponent: React.FC<ErrorComponentProps> = ({ message, severity }) => {

    if (!message) {
        message = 'Something went wrong. Please refresh the application.';
    }

    if (!severity) {
        severity = 'warn';
    }

    return (
        <div className="d-flex flex-column justify-content-center align-items-center vh-100">
            <img className="error-image" src={accessDeniedImage} alt="Access Denied" />
            {message && (
                <div className="mt-3">
                    <Message severity={severity} text={message} />
                </div>
            )}
        </div>
    );
};

export default ErrorComponent;