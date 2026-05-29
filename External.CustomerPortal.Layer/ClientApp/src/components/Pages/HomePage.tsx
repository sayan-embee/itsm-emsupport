import { createRef, useCallback, useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate, Outlet, Routes, Route } from 'react-router-dom';
import { InputText } from "primereact/inputtext";
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import OtpInput from "react-otp-input";
import { Toast } from 'primereact/toast';

import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';

import { v4 as uuidv4, validate } from "uuid";
import { ConfirmDialog } from 'primereact/confirmdialog';
import { Calendar } from "primereact/calendar";
import { Sidebar } from 'primereact/sidebar';
import { Dialog } from 'primereact/dialog';

import "../../App.css";
import "../../../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../../../node_modules/primereact/resources/primereact.min.css";
import "../../../node_modules/primeicons/primeicons.css";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.min.js';
import 'jquery/dist/jquery';

// Images import
import logo from '../../assets/Logo.svg';
import virtualbot from '../../assets/virtual-bot.svg';
import virtualbotsm from '../../assets/virtual-bot-sm.png';
import report from '../../assets/history.png';
import ticket from '../../assets/ticket.svg';
import avater from '../../assets/avater.png';
import office from '../../assets/corporate.svg';
import email from '../../assets/email-icon.svg';
import phone from '.../../assets/phone.svg';
import chatIcon from '../../assets/chat-icon.svg';

import { HOME_ROUTE_PATH, HOME_ROUTES, ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { useAlertDialog } from '../../store/AlertDialogProvider';
import { IState } from '../../Interfaces';
import { EMAIL_REGEX } from '../../Constants';
import { handleAlphanumericKeyPress, handleEmailKeyPress, handleErrorHelper, MaskEmail, MaskText, UseRegex } from '../../helpers/CommonMethods';
import OtpTimerComponent from '../common/OtpTimerComponent';
import { GetCustomerDetailsAPI, getNewOTPAPI, logoutAPI, verifyOTPAPI } from '../../apis/APIList';
import { CommonMessage, showErrorToast, showSuccessToast, showWarningToast, toastMessageType } from '../common/ToastComponent';
import LoaderComponent from '../common/LoaderComponent';
import { v4 } from 'uuid';
import OtpSkeleton from '../skeletons/OtpSkeleton';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import SidePanelComponent from './SidePanelComponent';
import HeaderComponent from './HeaderComponent';
import { useData } from '../../store/DataProvider';
import UnauthorizedComponent from '../common/UnauthorizedComponent';
import SessionTimeoutComponent from './SessionTimeoutComponent';


//#region Interfaces

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

interface ILoadingState {
    pageLoading: boolean;
    buttonLoading: boolean;
}

//#endregion

const HomePage = () => {
    dayjs.extend(utc);

    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const { showDialog, hideDialog } = useAlertDialog();

    const { setData } = useData();
    const location = useLocation();
    const navigate = useNavigate();

    const { jwtToken, jwtTokenExpiry } = location.state || {};

    //#region State
    const [isJwtValid, setIsJwtValid] = useState(true);
    // const setSignedInVerified = useAuthStore((state) => state.signIn);
    // const setCaptchaVerified = useAuthStore((state) => state.setCaptchaVerified);
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const isCaptchaVerified = useAuthStore((state) => state.isCaptchaVerified);
    const isOtpVerified = useAuthStore((state) => state.isOtpVerified);
    const isSignedIn = useAuthStore((state) => state.isSignedIn);
    const signOut = useAuthStore((state) => state.signOut);

    const { sessionId, sessionExpiresOn } = useAuthStore();

    const [sidebarIsVisible, setSidebarIsVisible] = useState<boolean>(false);
    const [reportDialogIsVisible, setReportDialogIsVisible] = useState<boolean>(false);

    const [customerDetails, setCustomerDetails] = useState<any>(null);

    const [email, setEmail] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        ErrorMessage: ''
    });

    const [emailMasked, setEmailMasked] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        ErrorMessage: ''
    });

    const [otp, setOtp] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [otpValidityInSec, setOtpValidityInSec] = useState<IState>({
        Value: 0,
        IsRequired: true,
        IsValid: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [otpExpiredOn, setOtpExpiredOn] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [otpExpired, setOtpExpired] = useState<IState>({
        Value: false,
        IsRequired: true,
        IsValid: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [resendButton, setResetButton] = useState<IState>({
        Value: '',
        IsRequired: false,
        IsValid: false,
        IsDisabled: true,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [startDate, setStartDate] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [endDate, setEndDate] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [signInToken, setSignInToken] = useState<any>(null);

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: true,
        buttonLoading: false
    });

    //#endregion


    //#region API

    const getNewOTP = async (): Promise<void> => {
        try {
            localStorage.removeItem('otpData');
            setResetButton({ ...resendButton, IsDisabled: true });
            setComponentLoading((prevState) => ({ ...prevState, buttonLoading: true, pageLoading: true }));
            const jsonBody = {
                "customerEmail": email.Value
            }
            const response = await getNewOTPAPI(jsonBody);
            console.log("getNewOTPAPI: ", response);

            if (response && response.data && response.data.otP_Id > 0) {
                showSuccessToast(toastRef, toastMessageType.Success, 'OTP has been sent!');

                const { validityInSec, recipient, expiredOn } = response.data;
                localStorage.setItem("otpData", JSON.stringify({ validityInSec, recipient, expiredOn }));

                setOtpExpired({ ...otpExpired, Value: false });
                setEmail({ ...email, Value: response.data?.recipient, IsValid: true });
                setOtp({ ...otp, Value: '', IsValid: false, ErrorMessage: '' });
                setOtpValidityInSec({ ...otpValidityInSec, Value: (response.data?.validityInSec) / 2 });
                setOtpExpiredOn({ ...otpExpiredOn, Value: response.data?.expiredOn });
                setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false, pageLoading: false }));
            }
            else if (response && response.data && response.data.message) {
                showErrorToast(toastRef, toastMessageType.Error, response.data.message);
                handleShowAlert('Attention Required', response.data.message);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
            }
        }
        catch (error) {
            const { statusCode, errorMessage } = handleErrorHelper("getNewOTP", error);
            if (statusCode === 401 || statusCode === 403) {
                setIsJwtValid(false);
                // navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, errorMessage);
            }
        }
    };

    const verifyOTP = async (): Promise<void> => {
        try {
            setResetButton({ ...resendButton, IsDisabled: true });
            setComponentLoading((prevState) => ({ ...prevState, buttonLoading: true }));
            const jsonBody = {
                "code": otp.Value,
                "recipient": email.Value,
            }
            const response = await verifyOTPAPI(jsonBody);
            console.log("verifyOTPAPI: ", response);

            if (response && response.data && response.data.status > 0) {
                showSuccessToast(toastRef, toastMessageType.Success, 'OTP has been verified!');

                // const { validityInSec, recipient, expiredOn } = response.data;
                // localStorage.setItem("otpData", JSON.stringify({ validityInSec, recipient, expiredOn }));

                resetPage();
            }
            else if (response && response.data && response.data.message) {
                handleShowAlert('Attention Required', response.data.message);
                resetPage();
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
                resetPage();
            }
        }
        catch (error) {
            const { statusCode, errorMessage } = handleErrorHelper("verifyOTP", error);
            if (statusCode === 401 || statusCode === 403) {
                setIsJwtValid(false);
                // navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, errorMessage);
            }
        }
    };

    const getCustomerDetails = async (): Promise<void> => {
        try {
            const response = await GetCustomerDetailsAPI();

            if (!response || response.status >= 400 || !response.data) {
                throw new Error(response?.data?.message);
            }

            console.log("getCustomerDetailsAPI: ", response);
            setData(response.data);
            setIsJwtValid(true);
            setCustomerDetails(response.data);
            setComponentLoading({ ...componentLoading, pageLoading: false });
        }
        catch (error: any) {
            const { statusCode, errorMessage } = handleErrorHelper("getCustomerDetails", error);
            if (statusCode === 401 || statusCode === 403) {
                setIsJwtValid(false);
                // navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, errorMessage);
            }
        }
        finally {
            setComponentLoading({ ...componentLoading, pageLoading: false });
        }
    };

    //#endregion


    //#region Helper Methods

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    const handleSignOut = useCallback(async () => {
        logoutAPI();
        signOut();
        navigate(ROUTE_PATH.SIGN_IN);
    }, [signOut, navigate]);

    const resetPage = async () => {
        setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false, pageLoading: false }));
        setOtp({ ...otp, Value: '', IsValid: false, ErrorMessage: '' });
    };



    //#endregion


    //#region OnChange Methods

    const handleDateChange = async (e: any, type: DateType) => {
        console.log("handleDateChange: ", e);

        // const MAX_DAY_DIFF = 30;

        switch (type) {
            case DateType.FromDate:
                {
                    setStartDate({ ...startDate, Value: e, IsValid: true, ErrorMessage: '' });
                    setEndDate({ ...endDate, Value: undefined, IsValid: true, ErrorMessage: '' });
                    break;
                }

            case DateType.ToDate:
                {
                    // if (dayjs(e).diff(dayjs(selectedFromDate), 'day') > MAX_DAY_DIFF) {
                    //     setToDate(dayjs(selectedFromDate).add(MAX_DAY_DIFF, 'day').toDate());
                    // }
                    setEndDate({ ...endDate, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    const handleEmailChange = (value: string) => {
        setEmail({ ...email, IsValid: true, Value: value });
    };

    const handleOtpChange = (value: string) => {
        setOtp({ ...otp, IsValid: true, Value: value });
    };

    const handleOtpExpire = () => {
        setOtpExpired({ ...otpExpired, Value: true });
        setResetButton({ ...resendButton, IsDisabled: false });
        console.log('OTP expired');
    };

    const handleResendOtp = async () => {
        await validateFields('NewOTP');

        if (!email.IsValid) {
            return;
        }
        else {
            setOtp({ ...otp, Value: '', IsValid: false, ErrorMessage: '' });
            await getNewOTP();
        }
    };

    const handleOnSubmit = async () => {
        const isValid = await validateFields('VerifyOTP');
        if (!isValid) {
            return;
        }

        await verifyOTP();
    };

    //#endregion


    //#region Validation Methods

    const validateFields = async (type: 'NewOTP' | 'VerifyOTP') => {
        let result = false;
        switch (type) {
            case 'NewOTP': {
                if (email.Value === '') {
                    setEmail({ ...email, IsValid: false, ErrorMessage: 'Email is required' });
                } else if (!UseRegex(EMAIL_REGEX, email.Value)) {
                    setEmail({ ...email, IsValid: false, ErrorMessage: 'Please enter a valid email address' });
                } else {
                    setEmail({ ...email, IsValid: true, ErrorMessage: '' });
                    result = true;
                }

                break;
            }

            case 'VerifyOTP': {
                if (!otp.Value) {
                    setOtp({ ...otp, IsValid: false, ErrorMessage: 'Enter OTP' });
                } else if (otp.Value && (otp.Value).toString().length < 6) {
                    setOtp({ ...otp, IsValid: false, ErrorMessage: 'Please enter 6 digit OTP' });
                } else {
                    setOtp({ ...otp, IsValid: true, ErrorMessage: '' });
                    result = true;
                }

                break;
            }
        }

        return result;
    };

    //#endregion


    //#region Effects

    useEffect(() => {
        const syncAuthState = (event: StorageEvent) => {
            if (event.key === 'auth-storage' && event.newValue) {
                try {
                    const newState = JSON.parse(event.newValue);
                    if (newState?.state) {
                        useAuthStore.setState(newState.state);
                    }
                } catch (error) {
                    console.error('Error parsing auth-storage:', error);
                }
            }
        };

        window.addEventListener('storage', syncAuthState);
        return () => window.removeEventListener('storage', syncAuthState);
    }, []);


    useEffect(() => {
        if (hasHydrated && (!isCaptchaVerified || !isOtpVerified || !isSignedIn)) {
            console.log("Redirecting to SignIn page...");
            handleSignOut();
        }
    }, [hasHydrated, isCaptchaVerified, isOtpVerified, isSignedIn, handleSignOut]);

    useEffect(() => {
        isMountedRef.current = true;
        if (isMountedRef.current) {
            console.log('onPageLoad:');
            console.log('jwtToken: ', jwtToken);
            console.log('utcDatetime: ', dayjs().utc().format('YYYY-MM-DD HH:mm:ss'));
            console.log('sessionExpiryInSecs: ', sessionExpiresOn);
            getCustomerDetails();
        }
    }, []);

    useEffect(() => {
        if (!isJwtValid) {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
        }
    }, [isJwtValid]);

    //#endregion

    if (!hasHydrated) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <LoaderComponent />
                <Toast ref={toastRef} />
            </div>
        )
    }

    if (componentLoading.pageLoading) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <LoaderComponent />
                <Toast ref={toastRef} />
            </div>
        )
    }

    return (
        <div>

            <ConfirmDialog />

            {/* Header */}
            <HeaderComponent
                headerTitle=""
                sidebarIsVisible={sidebarIsVisible}
                setSidebarIsVisible={setSidebarIsVisible}
            />

            {/* User Section - Small Device */}
            <Sidebar visible={sidebarIsVisible} position="right" onHide={() => setSidebarIsVisible(false)}>
                <div className="user-section-small-device d-flex flex-column justify-content-between">
                    <SidePanelComponent
                        customerDetails={customerDetails}
                        userName={customerDetails?.[0]?.customerName}
                        userEmail={customerDetails?.[0]?.customerEmail}
                        userMobile={customerDetails?.[0]?.customerPhone}
                        officeName={customerDetails?.[0]?.customerAddress}
                    />
                </div>
            </Sidebar>

            <main className="mt-4">
                <SessionTimeoutComponent />
                <Routes>
                    {HOME_ROUTES.map((route, index) => (
                        <Route key={index} path={route.path} element={route.component} />
                    ))}
                    <Route path="*" element={<Outlet />} />
                </Routes>
                <Outlet />
            </main>

        </div>
    );
};

export default HomePage;