// src/store/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
    isSignedIn: boolean;
    isCaptchaVerified: boolean;
    isOtpVerified: boolean;
    userEmail: string | null;
    sessionId: string | null;
    sessionExpiresOn: Date | null;

    setUserEmail: (email: string) => void;
    setSessionId: (id: string) => void;
    setSessionExpiresOn: (secs: Date) => void;
    setCaptchaVerified: (verified: boolean) => void;
    setOtpVerified: (verified: boolean) => void;
    signIn: () => void;
    signOut: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            isSignedIn: false,
            isCaptchaVerified: false,
            isOtpVerified: false,
            userEmail: null,
            sessionId: null,
            sessionExpiresOn: null,

            setUserEmail: (email) => set({ userEmail: email }),
            setSessionId: (id) => set({ sessionId: id }),
            setSessionExpiresOn: (date) => set({ sessionExpiresOn: date }),
            setCaptchaVerified: (verified) => set({ isCaptchaVerified: verified }),
            setOtpVerified: (verified) => set({ isOtpVerified: verified }),
            signIn: () => set({ isSignedIn: true }),
            signOut: () => set({ isSignedIn: false, isCaptchaVerified: false, isOtpVerified: false, userEmail: null, sessionId: null, sessionExpiresOn: null }),
        }),
        {
            name: 'auth-storage',
            // getStorage: () => sessionStorage, // for single tab-specific storage
            getStorage: () => localStorage, // for mutiple tab-specific storage
            onRehydrateStorage: () => (state) => {
                if (process.env.NODE_ENV === 'development') {
                    console.log("Rehydrating state from sessionStorage:", state);
                }
            },
        }
    )
);