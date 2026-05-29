import React, { useState, useEffect } from 'react';
import dayjs from 'dayjs';

const getOrdinalSuffix = (day: number) => {
    if (day > 3 && day < 21) return 'th';
    switch (day % 10) {
        case 1: return 'st';
        case 2: return 'nd';
        case 3: return 'rd';
        default: return 'th';
    }
};

const CurrentDateTime = () => {
    const formatDateTime = () => {
        const now = dayjs();
        const day = now.date();
        const dayWithSuffix = `${day}${getOrdinalSuffix(day)}`;
        return `${now.format('MMMM')} ${dayWithSuffix}, ${now.format('YYYY hh:mm A')}`;
    };

    const [currentDateTime, setCurrentDateTime] = useState(formatDateTime);

    useEffect(() => {
        const updateAndReschedule = () => {
            setCurrentDateTime(formatDateTime());

            const delay = (60 - new Date().getSeconds()) * 1000;
            setTimeout(updateAndReschedule, delay);
        };

        updateAndReschedule();
    }, []);

    return <span>{currentDateTime}</span>;
};

export default CurrentDateTime;