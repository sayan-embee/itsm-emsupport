import React, { useState, useEffect } from 'react';
import { Alert } from '@fluentui/react-northstar';
import { ExclamationTriangleIcon, AcceptIcon, InfoIcon } from '@fluentui/react-icons-northstar';

export const NorthStar_Alert_TYPES = {
    NONE: 'none',
    INFO: 'info',
    SUCCESS: 'success',
    DANGER: 'danger',
    WARNING: 'warning',
};

interface NorthStarAlertComponentProps {
    message: string;
    type: string;
    duration?: number;
}

export enum CommonMessages {
    NoData = 'No data available',
    MandatoryFields = 'Please provide all the required fields',
    Error = 'There was an error processing your request.',
    Refresh = 'Please refresh the page and try again'
}

const NorthStarAlert: React.FC<NorthStarAlertComponentProps> = ({ message, type, duration = 7000 }) => {
    const [isVisible, setVisible] = useState(false);

    useEffect(() => {
        if (type !== NorthStar_Alert_TYPES.NONE) {
            setVisible(true);

            const timeoutId = setTimeout(() => {
                setVisible(false);
            }, duration);

            return () => clearTimeout(timeoutId);
        }
    }, [type, duration]);

    if (!isVisible) {
        return null;
    }
    else {
        return (
            <Alert
                className="mt-2 mb-2"
                content={message}
                icon={
                    type === 'danger' || type === 'warning' ? (
                        <ExclamationTriangleIcon />
                    ) : type === 'success' ? (
                        <AcceptIcon />
                    ) : (
                        <InfoIcon />
                    )
                }
                info={type === 'info'}
                success={type === 'success'}
                danger={type === 'danger'}
                warning={type === 'warning'}
                dismissible
                dismissAction={{
                    'aria-label': 'close',
                    onClick: () => setVisible(false),
                }}
            />
        )
    }
};

export default NorthStarAlert;