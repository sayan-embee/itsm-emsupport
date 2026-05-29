// import React, { useState, useEffect } from 'react';

// const OtpTimerComponent: React.FC<{ duration: number; onExpire: () => void }> = ({ duration, onExpire }) => {
//     const [timeLeft, setTimeLeft] = useState(() => {
//         const storedData = localStorage.getItem("otpData");
//         if (storedData) {
//             const { validityInSec } = JSON.parse(storedData);
//             return validityInSec;
//         }
//         return duration;
//     });

//     useEffect(() => {
//         if (timeLeft <= 0) {
//             onExpire();
//             return;
//         }

//         const timerId = setInterval(() => {
//             setTimeLeft((prev: number) => {
//                 const newTime = prev - 1;
//                 // Update local storage every 10 seconds
//                 if (newTime % 10 === 0) {
//                     const storedData = localStorage.getItem('otpData');
//                     if (storedData) {
//                         const data = JSON.parse(storedData);
//                         const updatedData = {
//                             ...data,
//                             validityInSec: newTime, // Update only validityInSec
//                         };
//                         localStorage.setItem('otpData', JSON.stringify(updatedData));
//                     }
//                 }
//                 return newTime;
//             });
//         }, 1000);

//         return () => {
//             clearInterval(timerId);
//         };
//     }, [timeLeft, onExpire]);

//     const formatTime = (seconds: number) => {
//         const minutes = Math.floor(seconds / 60);
//         const remainingSeconds = seconds % 60;
//         return `${String(minutes).padStart(2, '0')}:${String(remainingSeconds).padStart(2, '0')}`;
//     };

//     return <>{formatTime(timeLeft)}</>;
// };

// export default OtpTimerComponent;


import React, { useState, useEffect } from 'react';

const OtpTimerComponent: React.FC<{ duration: number; onExpire: () => void }> = ({ duration, onExpire }) => {
    const [timeLeft, setTimeLeft] = useState(duration);
console.log("OtpTimerComponent: duration", duration);
    useEffect(() => {
        if (timeLeft <= 0) {
            onExpire();
            return;
        }

        const timerId = setInterval(() => {
            setTimeLeft((prev) => prev - 1);
        }, 1000);

        return () => clearInterval(timerId);
    }, [timeLeft, onExpire]);

    const formatTime = (seconds: number) => {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        return `${String(minutes).padStart(2, '0')}:${String(remainingSeconds).padStart(2, '0')}`;
    };

    return <>{formatTime(timeLeft)}</>;
};

export default OtpTimerComponent;
