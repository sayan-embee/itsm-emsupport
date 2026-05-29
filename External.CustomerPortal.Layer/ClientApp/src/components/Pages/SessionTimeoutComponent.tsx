import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../../store/authStore";
import { useCallback, useEffect } from "react";
import { HOME_ROUTE_PATH, ROUTE_PATH } from "../../router";
import { logoutAPI } from "../../apis/APIList";

import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { useQueryClient } from "@tanstack/react-query";

dayjs.extend(utc);

const SessionTimeoutComponent: React.FC = () => {
    const navigate = useNavigate();

    const { sessionExpiresOn } = useAuthStore();
    const signOut = useAuthStore((state) => state.signOut);
    const queryClient = useQueryClient();

    const handleSignOut = useCallback(async () => {
        logoutAPI();

        queryClient.removeQueries(['ticketDetailsForCount']); // Used in TicketsCardComponent
        queryClient.removeQueries(['ticketDetailsForPercent']); // Used in TicketsCardComponent
        queryClient.clear(); // Clear all React Query cache

        signOut();
        navigate(ROUTE_PATH.SIGN_IN);
    }, [signOut, navigate]);

    useEffect(() => {
        if (!sessionExpiresOn) return;
        console.log('Session Checking...');

        const expiryTime = dayjs.utc(sessionExpiresOn).subtract(5, 'minute').valueOf();
        const currentTime = dayjs.utc().valueOf();
        const timeoutDuration = expiryTime - currentTime;

        // Handle automatic logout when token expires
        if (timeoutDuration > 0) {
            const timeoutId = setTimeout(() => {
                // handleSignOut();
                navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }, timeoutDuration);

            return () => clearTimeout(timeoutId);
        } else {
            // handleSignOut();
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
        }
    }, [sessionExpiresOn, navigate]);

    useEffect(() => {
        if (!sessionExpiresOn) return;

        const checkExpirationOnFocus = () => {
            console.log('Session Checking on focus...');

            const expiryTime = dayjs.utc(sessionExpiresOn).valueOf();
            const currentTime = dayjs.utc().valueOf();
            if (currentTime >= expiryTime) {
                // handleSignOut();
                navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }
        };

        window.addEventListener('focus', checkExpirationOnFocus);
        return () => window.removeEventListener('focus', checkExpirationOnFocus);
    }, [sessionExpiresOn, navigate]);

    return null;
};

export default SessionTimeoutComponent;