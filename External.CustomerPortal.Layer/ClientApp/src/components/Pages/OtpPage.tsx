import { createRef, useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { InputText } from "primereact/inputtext";
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import OtpInput from "react-otp-input";
import { Toast } from 'primereact/toast';
import dayjs from "dayjs";
import { v4 as uuidv4, validate } from "uuid";
import { ConfirmDialog } from 'primereact/confirmdialog';

import "../../App.css";
import "../../../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../../../node_modules/primereact/resources/primereact.min.css";
import "../../../node_modules/primeicons/primeicons.css";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.min.js';
import 'jquery/dist/jquery';

// Images import
import logo from '../../assets/Logo.svg';

import { ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { useAlertDialog } from '../../store/AlertDialogProvider';
import { IState } from '../../Interfaces';
import { EMAIL_REGEX } from '../../Constants';
import { handleAlphanumericKeyPress, handleEmailKeyPress, handleNumericKeyPress, MaskEmail, MaskText, UseRegex } from '../../helpers/CommonMethods';
import OtpTimerComponent from '../common/OtpTimerComponent';
import { getNewOTPAPI, verifyOTPAPI } from '../../apis/APIList';
import { CommonMessage, showErrorToast, showSuccessToast, showWarningToast, toastMessageType } from '../common/ToastComponent';
import LoaderComponent from '../common/LoaderComponent';
import { v4 } from 'uuid';
import OtpSkeleton from '../skeletons/OtpSkeleton';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';

//#region Interfaces

interface ILoadingState {
    pageLoading: boolean;
    buttonLoading: boolean;
}

//#endregion

const OtpPage = () => {
    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const { showDialog, hideDialog } = useAlertDialog();

    const location = useLocation();
    const navigate = useNavigate();

    const { validityInSec, recipient, expiredOn } = location.state || {};

    //#region State

    // const setSignedInVerified = useAuthStore((state) => state.signIn);
    // const setCaptchaVerified = useAuthStore((state) => state.setCaptchaVerified);
    // const signOut = useAuthStore((state) => state.signOut);
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const isCaptchaVerified = useAuthStore((state) => state.isCaptchaVerified);
    const setOtpVerified = useAuthStore((state) => state.setOtpVerified);
    const setSessionId = useAuthStore((state) => state.setSessionId);
    const setSessionExpiresOn = useAuthStore((state) => state.setSessionExpiresOn);
    const signIn = useAuthStore((state) => state.signIn);

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
                setOtpValidityInSec({ ...otpValidityInSec, Value: 90
                    // (response.data?.validityInSec) / 2 
                });
                setOtpExpiredOn({ ...otpExpiredOn, Value: response.data?.expiredOn });
                setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false, pageLoading: false }));
            }
            else if (response && response.data && response.data.message) {
                showErrorToast(toastRef, toastMessageType.Error, response.data.message);
                handleShowAlert('Attention Required', response.data.message);
                navigate(ROUTE_PATH.SIGN_IN);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
                navigate(ROUTE_PATH.SIGN_IN);
            }
        }
        catch (error) {
            console.error("Error at getNewOTP():", error);
            showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
            navigate(ROUTE_PATH.SIGN_IN);
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

            if (response && response.data && response.data.status > 0 && response.data.jwtToken) {
                showSuccessToast(toastRef, toastMessageType.Success, 'OTP has been verified!');

                // const { validityInSec, recipient, expiredOn } = response.data;
                // localStorage.setItem("otpData", JSON.stringify({ validityInSec, recipient, expiredOn }));
                handleOtpSuccess(response.data);
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
            console.error("Error at verifyOTP():", error);
            showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
            resetPage();
        }
    };

    //#endregion


    //#region Helper Methods

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    const handleOtpSuccess = (data: any) => {
        setOtpVerified(true);
        setSessionId(data.sessionId);
        setSessionExpiresOn(data.jwtTokenExpiry);
        signIn();
        navigate(ROUTE_PATH.HOME, { state: { sessionId: data.sessionId, jwtToken: data.jwtToken, jwtTokenExpiry: data.jwtTokenExpiry } });
    };

    const resetPage = async () => {
        setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false, pageLoading: false }));
        setOtp({ ...otp, Value: '', IsValid: false, ErrorMessage: '' });
    };

    //#endregion


    //#region Event Handlers

    const handleOtpKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        handleNumericKeyPress(event);
        if (event.key === 'Enter') {
            handleOnSubmit();
        }
    };

    //#endregion


    //#region OnChange Methods

    const handleEmailChange = (value: string) => {
        setEmail({ ...email, IsValid: true, Value: value });
    };

    const handleOtpChange = (value: string) => {
        setOtp({ ...otp, IsValid: value?.length === 6 ? true : false, Value: value });
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

    const handleGoBack = async () => {
        const isConfirmed = await showConfirmDialog({
            header: 'Attention Required',
            message: 'Do you want to go back?',
            icon: 'pi pi-exclamation-triangle',
        });

        if (isConfirmed) {
            navigate(ROUTE_PATH.SIGN_IN);
        }
    }

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
        if (hasHydrated && !isCaptchaVerified) {
            console.log("Captcha not verified. Redirecting to SignIn page...");
            navigate(ROUTE_PATH.SIGN_IN);
        }
    }, [hasHydrated, isCaptchaVerified, navigate]);

    useEffect(() => {
        isMountedRef.current = true;

        if (!recipient || !validityInSec) {
            const storedData = localStorage.getItem("otpData");
            if (storedData) {
                console.log("storedData: ", storedData);

                const { validityInSec, recipient, expiredOn } = JSON.parse(storedData);
                console.log("storedData-validityInSec: ", validityInSec);
                console.log("storedData-recipient: ", recipient);
                console.log("storedData-expiredOn: ", expiredOn);

                const maskedEmail = MaskEmail(recipient);

                // Batch state updates
                if (isMountedRef.current) {
                    setOtpExpired({ ...otpExpired, Value: false });
                    setEmail({ ...email, Value: recipient, IsValid: true });
                    setEmailMasked({ ...emailMasked, Value: maskedEmail, IsValid: true });
                    // setOtpValidityInSec({ ...otpValidityInSec, Value: validityInSec });
                    setOtpValidityInSec({ ...otpValidityInSec, Value: 90 });
                    setOtpExpiredOn({ ...otpExpiredOn, Value: expiredOn });
                    setComponentLoading({ ...componentLoading, pageLoading: false });
                }
            } else {
                navigate(ROUTE_PATH.SIGN_IN);
            }
        } else {
            localStorage.setItem("otpData", JSON.stringify({ validityInSec, recipient, expiredOn }));

            const maskedEmail = MaskEmail(recipient);

            // Batch state updates
            if (isMountedRef.current) {
                setOtpExpired({ ...otpExpired, Value: false });
                setEmail({ ...email, Value: recipient, IsValid: true });
                setEmailMasked({ ...emailMasked, Value: maskedEmail, IsValid: true });
                // setOtpValidityInSec({ ...otpValidityInSec, Value: validityInSec });
                setOtpValidityInSec({ ...otpValidityInSec, Value: 90 });
                setOtpExpiredOn({ ...otpExpiredOn, Value: expiredOn });
                setComponentLoading({ ...componentLoading, pageLoading: false });
            }
        }

        return () => {
            isMountedRef.current = false;
        };
    }, [recipient, validityInSec, expiredOn, navigate]);

    // Check OTP expiry
    useEffect(() => {
        if (otpExpiredOn.Value) {
            const expiredDate = dayjs(otpExpiredOn.Value);

            const checkExpiry = () => {
                const currentDate = dayjs();

                // Check if the current datetime exceeds expiredOn
                if (currentDate.isAfter(expiredDate)) {
                    console.log("OTP expired on " + expiredDate.format('YYYY-MM-DD HH:mm:ss') + ".. Redirecting to SignIn page...");
                    showWarningToast(toastRef, toastMessageType.Warn, "OTP expired. Please generate a new OTP.");
                    navigate(ROUTE_PATH.SIGN_IN);
                }
            };

            checkExpiry();

            intervalRef.current = setInterval(checkExpiry, 10000); // Check every 10 seconds
        }

        // Prevent going back to the SignIn page
        window.history.pushState(null, "", window.location.href);
        window.onpopstate = () => {
            window.history.pushState(null, "", window.location.href);
        };

        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
            }
        };
    }, [otpExpiredOn.Value, navigate]);

    //#endregion

    if (!hasHydrated) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <LoaderComponent />
            </div>
        )
    }

    if (componentLoading.pageLoading) {
        return (
            <OtpSkeleton />
        )
    }

    return (
        <div>
            <div className='auth-wrapper'>
                <div className="auth-bg">
                    <div className="container">

                        <ConfirmDialog />
                        <Toast ref={toastRef} />

                        <div className="auth-form-wrapper">
                            <img className='brand' src={logo} alt="Embee" />
                            <h3 style={{ color: "#10446f" }}>Enter OTP</h3>
                            {/* <p>to access ITSM Automation Portal</p> */}
                            <div className='mt-4 mb-4'>
                                <p className='m-0'>Please enter OTP sent on your email ID</p>
                                <div className='auth-id'>{emailMasked.Value}</div>
                            </div>
                            <div className="mb-4 autocpmplete">
                                <OtpInput
                                    containerStyle={{
                                        marginBottom: "15px",
                                    }}
                                    value={otp.Value}
                                    onChange={handleOtpChange}
                                    numInputs={6}
                                    renderSeparator={<span style={{ width: "8px" }}></span>}
                                    inputType="tel"
                                    shouldAutoFocus={true}
                                    inputStyle={{
                                        border: "1px solid transparent",
                                        background: '#EAEEF1',
                                        borderRadius: "8px",
                                        width: "40px",
                                        height: "40px",
                                        fontSize: "14px",
                                        color: "#000",
                                        fontWeight: "500",
                                        caretColor: "blue"
                                    }}
                                    renderInput={(inputProps) => (
                                        <input
                                            {...inputProps}
                                            disabled={componentLoading.buttonLoading}
                                            pattern="[0-9]*"
                                            // onInput={(e?: any) => {
                                            //     e.target.value = e.target.value.replace(/[^0-9]/g, '');
                                            // }}
                                            onKeyDown={(e) => handleOtpKeyDown(e)}
                                        />
                                    )}
                                />
                                {otp.ErrorMessage && !otp.IsValid && <small className='require'>{otp.ErrorMessage}</small>}
                            </div>
                            <div className="d-flex flex-column flex-md-row gap-3 justify-content-between align-items-center mb-4">
                                <Button className='w-100' outlined label="Go to Back"
                                    onClick={handleGoBack}
                                    disabled={componentLoading.buttonLoading}
                                />
                                <Button className='w-100 primary'
                                    label={componentLoading.buttonLoading ? '' : 'Verify OTP'}
                                    loading={componentLoading.buttonLoading}
                                    onClick={handleOnSubmit}
                                    disabled={!otp.IsValid || componentLoading.buttonLoading}
                                    style={{
                                        cursor: componentLoading.buttonLoading ? "not-allowed" : "pointer",
                                    }}
                                />
                            </div>
                            <span style={{ display: 'flex', alignItems: 'center' }}>
                                Did not receive the code?
                                <div
                                    onClick={handleResendOtp}
                                    aria-disabled={resendButton.IsDisabled}
                                    style={{
                                        color: resendButton.IsDisabled ? "#84818a" : "#10446f",
                                        cursor: resendButton.IsDisabled ? "not-allowed" : "pointer",
                                        textDecoration: 'none',
                                        marginLeft: '8px', // Add some space between the text and the button
                                    }}
                                >
                                    {!otpExpired.Value ? 'Resend in ' : 'Resend '}
                                    {/* {!otpExpired.Value && <OtpTimerComponent duration={validityInSec ?? otpValidityInSec.Value} onExpire={handleOtpExpire} />} */}
                                    {!otpExpired.Value && <OtpTimerComponent duration={90} onExpire={handleOtpExpire} />}
                                </div>
                            </span>
                            {/* <span style={{ display: 'flex', alignItems: 'center' }}>
                                <div
                                    onClick={handleGoBack}
                                    style={{
                                        color: 'blue',
                                        cursor: "pointer",
                                        textDecoration: 'none',
                                        marginTop: '5px',
                                    }}
                                >
                                    Go back?
                                </div>
                            </span> */}
                        </div>

                    </div>
                </div>
            </div>

        </div>
    );
};

export default OtpPage;