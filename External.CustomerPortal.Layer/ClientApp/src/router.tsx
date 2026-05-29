import { Navigate, Route, Routes } from "react-router-dom";
import HomePage from "./components/Pages/HomePage";
import OtpPage from "./components/Pages/OtpPage";
import SignInPage from "./components/Pages/SignInPage";
import { useAuthStore } from "./store/authStore";
import DashboardPage from "./components/Pages/DashboardPage";
import TicketDetailsPage from "./components/Pages/TicketDetailsPage";
import WebChatPage from "./components/Pages/WebChatPage";
import UnauthorizedComponent from "./components/common/UnauthorizedComponent";
import SessionExpiredComponent from "./components/common/SessionExpiredComponent";
import { CommonMessage } from "./components/common/ToastComponent";

type TRoute = {
  path: string;
  component: React.ReactNode;
  isProtected?: boolean;
};

// Public Routes
export const ROUTE_PATH = {
  SIGN_IN: "/signIn",
  OTP: "/otp",
  HOME: "/home"
} as const;

// Home Sub-Routes
export const HOME_ROUTE_PATH = {
  SESSION_EXPIRED: "/session-expired",
  UNAUTHORIZED: "/unauthorized",
  DASHBOARD: "/dashboard",
  TICKETS: "/tickets",
  WEBCHAT: "/support-chat",
} as const;

// Captcha Protection Wrapper
const CaptchaProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isCaptchaVerified } = useAuthStore();

  console.log('isCaptchaVerified: ' + isCaptchaVerified);

  if (!isCaptchaVerified) {
    return <Navigate to={ROUTE_PATH.SIGN_IN} />;
  }

  return <>{children}</>;
};

// OTP Protection Wrappers
const OtpProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isSignedIn, isOtpVerified, isCaptchaVerified } = useAuthStore();

  console.log('isSignedIn: ' + isSignedIn);
  console.log('isCaptchaVerified: ' + isCaptchaVerified);
  console.log('isOtpVerified: ' + isOtpVerified);

  if (!isCaptchaVerified) {
    return <Navigate to={ROUTE_PATH.SIGN_IN} />;
  }

  if (!isOtpVerified) {
    return <Navigate to={ROUTE_PATH.OTP} />;
  }

  return isSignedIn ? <>{children}</> : <Navigate to={ROUTE_PATH.SIGN_IN} />;
};

// Main Routes
export const ROUTES: TRoute[] = [
  {
    path: ROUTE_PATH.SIGN_IN,
    component: <SignInPage />,
  },
  {
    path: ROUTE_PATH.OTP,
    component: <CaptchaProtectedRoute><OtpPage /></CaptchaProtectedRoute>,
    isProtected: true,
  },
  {
    path: ROUTE_PATH.HOME + "/*",  // Allow nested routes
    component: <OtpProtectedRoute><HomePage /></OtpProtectedRoute>,
    isProtected: true,
  },
  {
    path: "*",
    component: <Navigate to={ROUTE_PATH.SIGN_IN} />,
  },
];

// Home Routes (Nested Inside HomePage)
export const HOME_ROUTES: TRoute[] = [
  {
    path: HOME_ROUTE_PATH.SESSION_EXPIRED,
    component: <SessionExpiredComponent />,
    isProtected: true,
  },
  {
    path: HOME_ROUTE_PATH.UNAUTHORIZED,
    component: <UnauthorizedComponent message={CommonMessage.Unauthorized} severity="error" />,
    isProtected: true,
  },
  {
    path: HOME_ROUTE_PATH.DASHBOARD,
    component: <DashboardPage />,
    isProtected: true,
  },
  {
    path: HOME_ROUTE_PATH.TICKETS,
    component: <TicketDetailsPage />,
    isProtected: true,
  },
  {
    path: HOME_ROUTE_PATH.WEBCHAT,
    component: < WebChatPage />,
    isProtected: true,
  },
  {
    path: "*",
    component: <Navigate to={ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD} />,
  },
];