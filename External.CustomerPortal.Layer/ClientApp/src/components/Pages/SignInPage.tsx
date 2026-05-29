import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { InputText } from "primereact/inputtext";
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Toast } from 'primereact/toast';
import { v4 as uuidv4 } from "uuid";

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
import { IClientInfo, IState } from '../../Interfaces';
import { EMAIL_REGEX } from '../../Constants';
import { GetClientInfo, handleAlphanumericKeyPress, handleEmailKeyPress, UseRegex } from '../../helpers/CommonMethods';
import { toastMessageType, showSuccessToast, showErrorToast, CommonMessage, showWarningToast } from '../common/ToastComponent';
import { getNewOTPAPI } from '../../apis/APIList';
import { Skeleton } from 'primereact/skeleton';
import { ConfirmDialog } from 'primereact/confirmdialog';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import { Tooltip } from 'primereact/tooltip';

//#region Interfaces

interface ILoadingState {
    pageLoading: boolean;
    buttonLoading: boolean;
}

//#endregion

const SignInPage = () => {
    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);
    const captchaTimerRef = useRef<NodeJS.Timeout | null>(null);
    const { showDialog, hideDialog } = useAlertDialog();
    //#region State

    // const setSignedInVerified = useAuthStore((state) => state.signIn);
    const setUserEmail = useAuthStore((state) => state.setUserEmail);
    const setCaptchaVerified = useAuthStore((state) => state.setCaptchaVerified);
    const signOut = useAuthStore((state) => state.signOut);
    const navigate = useNavigate();

    const [clientInfo, setClientInfo] = useState<IClientInfo>({
        ClientIP: null,
        UserAgent: null,
        DeviceType: null,
        Location: {
            latitude: null,
            longitude: null,
        }
    });

    const [email, setEmail] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        ErrorMessage: ''
    });

    const [captchaId, setCaptchaId] = useState<string | null>(null);

    const [captchaExpression, setCaptchaExpression] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        IsLoading: true,
        ErrorMessage: ''
    });

    const [captcha, setCaptcha] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        ErrorMessage: ''
    });

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: true,
        buttonLoading: false
    });

    const [submitButtonValid, setSubmitButtonValid] = useState<boolean>(false);

    //#endregion

    //#region API

    const getNewOTP = async (): Promise<void> => {
        try {
            setComponentLoading((prevState) => ({ ...prevState, buttonLoading: true }));

            const jsonBody = {
                "userEmail": email.Value,
                "userAgent": clientInfo.UserAgent,
                "deviceType": clientInfo.DeviceType,
                "location": `${clientInfo.Location.latitude},${clientInfo.Location.longitude}`
            }
            const response = await getNewOTPAPI(jsonBody);
            console.log("getNewOTPAPI: ", response);

            if (response && response.data && response.data.otP_Id > 0) {
                showSuccessToast(toastRef, toastMessageType.Success, 'OTP has been sent!');
                handleCaptchaSuccess(response.data);
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
            console.error("Error at getNewOTP():", error);
            showErrorToast(toastRef, toastMessageType.Error, CommonMessage.Error);
            resetPage();
        }
    };

    //#endregion

    const handleCaptchaSuccess = (data?: any) => {
        setCaptchaVerified(true);
        navigate(ROUTE_PATH.OTP, {
            state: {
                validityInSec: (data.validityInSec) / 2,
                recipient: data.recipient,
                expiredOn: data.expiredOn
            },
        });
    };

    //#region Helper Methods

    const focusEmailInput = () => {
        const input = document.getElementById("email-InputText") as HTMLInputElement;
        if (input) {
            input.focus();
        }
    };

    const focusCaptchaInput = () => {
        const input = document.getElementById("captcha") as HTMLInputElement;
        if (input) {
            input.focus();
        }
    };

    const generateCaptcha = async () => {
        if (isMountedRef.current) {
            const newCaptchaId = uuidv4();
            setCaptchaId(newCaptchaId);

            if (captchaTimerRef.current) {
                clearTimeout(captchaTimerRef.current);
            }

            setSubmitButtonValid(false);
            setCaptchaExpression({ ...captchaExpression, Value: '', IsLoading: true });

            const characters = "ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
            let result = '';
            for (let i = 0; i < 6; i++) {
                const randomIndex = Math.floor(Math.random() * characters.length);
                result += characters[randomIndex];
            }
            console.log('Captcha: ', result);

            await new Promise((resolve) => setTimeout(resolve, 1000));

            setCaptcha({ ...captcha, Value: '', IsValid: false });
            setCaptchaExpression({ ...captchaExpression, Value: result, IsLoading: false });

            captchaTimerRef.current = setTimeout(() => {
                generateCaptcha();
            }, 120000);
        }
    };

    //#endregion


    //#region Event Handlers

    const handleCaptchKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        handleAlphanumericKeyPress(event);
        if (event.key === 'Enter') {
            handleOnSubmit();
        }
    };

    //#endregion


    //#region OnChange Methods

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    const handleEmailChange = (value: string) => {
        setEmail({ ...email, Value: value, IsValid: false, ErrorMessage: '' });
    };

    const handleCaptchaChange = (value: string) => {
        setCaptcha({ ...captcha, Value: value, IsValid: false, ErrorMessage: '' });
    };

    const handleOnSubmit = async () => {
        await validateFields();

        if (email.IsValid && captcha.IsValid && submitButtonValid) {
            setUserEmail(email.Value);
            // await new Promise((resolve) => setTimeout(resolve, 2000));

            // const storedUser = JSON.parse(localStorage.getItem('auth-storage') || '{}');
            // if (storedUser?.userEmail && storedUser.userEmail !== email.Value) {
            //     const isConfirmed = await showConfirmDialog({
            //         header: 'Attention Required',
            //         message: 'Another user is already logged in. Do you want to sign out?',
            //         icon: 'pi pi-exclamation-triangle',
            //     });
            //     if (isConfirmed) {
            //         localStorage.removeItem('auth-storage');
            //         window.location.reload();
            //         return;
            //     }
            //     else {
            //         return;
            //     }
            // }
            // else {
            //     await getNewOTP();
            // }
            await getNewOTP();
        }
    };

    const resetPage = async () => {
        setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false }));
        // setEmail({ ...email, IsValid: true });
        // setCaptcha({ ...captcha, IsValid: true });
        generateCaptcha();
    };

    //#endregion


    //#region Validation Methods

    const validateFields = async () => {
        await validateEmail();
        await validateCaptcha();
    };

    const validateEmail = async () => {
        if (email.Value === '') {
            setEmail({ ...email, IsValid: false, ErrorMessage: 'Enter an email' });
            setSubmitButtonValid(false);
            focusEmailInput();
        } else if (!UseRegex(EMAIL_REGEX, email.Value)) {
            setEmail({ ...email, IsValid: false, ErrorMessage: 'Please enter a valid email address' });
            setSubmitButtonValid(false);
            focusEmailInput();
        } else {
            setEmail({ ...email, IsValid: true, ErrorMessage: '' });
        }
    };

    const validateCaptcha = async () => {
        if (captcha.Value === '') {
            setCaptcha({ ...captcha, IsValid: false, ErrorMessage: 'Enter captcha' });
            // focusCaptchaInput();
            setSubmitButtonValid(false);
        } else if (captcha.Value !== captchaExpression.Value) {
            setCaptcha({ ...captcha, IsRequired: true, IsValid: false, ErrorMessage: 'Please enter a valid captcha' });
            // focusCaptchaInput();
            setSubmitButtonValid(false);
        } else {
            setCaptcha({ ...captcha, IsValid: true, ErrorMessage: '' });
        }
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
        isMountedRef.current = true;
        localStorage.removeItem('otpData');

        hideDialog();
        generateCaptcha();
        signOut();

        GetClientInfo().then((clientInfo) => {
            setClientInfo(clientInfo);
        });

        focusEmailInput();

        return () => {
            isMountedRef.current = false;
            if (captchaTimerRef.current) {
                console.log('Clearing captcha timer...');
                clearTimeout(captchaTimerRef.current);
            }
        };
    }, []);

    useEffect(() => {
        if (captcha.Value.length === 6) {
            validateCaptcha();
        }
    }, [captcha.Value]);

    useEffect(() => {
        if (!captcha.IsValid) {
            generateCaptcha();
        }
        else if (captcha.IsValid) {
            setSubmitButtonValid(true);
        }
    }, [captcha.IsValid]);

    //#endregion

    return (
        <div>
            <div className='auth-wrapper'>
                <div className="auth-bg">
                    <div className="container">

                        <ConfirmDialog />
                        <Toast ref={toastRef} />

                        <div className="auth-form-wrapper">
                            <img className='brand' src={logo} alt="Embee" />
                            <h3 style={{ color: "#10446f" }}>Sign in</h3>
                            {/* <Tooltip target=".tooltip-target" />
                            <p>
                                To access{' '}
                                <span
                                    className="tooltip-target"
                                    style={{ cursor: 'pointer' }}
                                    data-pr-tooltip="Embee Intelligent Support"
                                    data-pr-position="top"
                                >
                                    EIS {' '}
                                </span>
                                Automation Portal
                            </p> */}

                            <p>
                                To access AI EmSupport
                            </p>

                            <div className='mb-4'>
                                <span className="p-float-label">
                                    <InputText type="email" id="email-InputText"
                                        className={`w-100 ${email.ErrorMessage && !email.IsValid ? 'p-invalid' : ''}`}
                                        value={email.Value}
                                        onChange={(e) => handleEmailChange(e.target.value)}
                                        onBlur={validateEmail}
                                        onPaste={validateEmail}
                                        onKeyDown={handleEmailKeyPress}
                                        maxLength={150}
                                        disabled={componentLoading.buttonLoading}
                                    />
                                    <label htmlFor="ic">Email address<small className='require'>*</small></label>
                                </span>
                                {email.ErrorMessage && !email.IsValid && <small className='require'>{email.ErrorMessage}</small>}
                            </div>

                            <div className='captcha-box d-flex align-items-center mb-4'>
                                {captchaExpression.IsLoading
                                    // ? <ProgressSpinner style={{ width: '50px', height: '50px' }} />
                                    ? <Skeleton width="230px" height="60px" />
                                    :
                                    (
                                        <>
                                            <div key={captchaId} className="captchaText">
                                                {captchaExpression.Value}
                                                <div className="border_bg"></div>
                                            </div>
                                            <Button className='ms-2' text aria-label="refresh" icon="pi pi-refresh"
                                                disabled={componentLoading.buttonLoading}
                                                onClick={() => generateCaptcha()}
                                            />
                                        </>
                                    )
                                }
                            </div>

                            <div className='mb-4'>
                                <span className="p-float-label">
                                    <InputText id="captcha"
                                        className={`w-100 ${captcha.ErrorMessage && !captcha.IsValid ? 'p-invalid' : ''}`}
                                        value={captcha.Value}
                                        onChange={(e) => handleCaptchaChange(e.target.value)}
                                        onBlur={validateCaptcha}
                                        onPaste={validateCaptcha}
                                        maxLength={6}
                                        onKeyDown={handleCaptchKeyDown}
                                        disabled={componentLoading.buttonLoading}
                                    />
                                    <label htmlFor="captcha">Enter captcha<small className='require'>*</small></label>
                                </span>
                                {captcha.ErrorMessage && !captcha.IsValid && <small className='require'>{captcha.ErrorMessage}</small>}
                            </div>

                            <Button className='w-100 primary'
                                label={componentLoading.buttonLoading ? '' : 'Send OTP'}
                                loading={componentLoading.buttonLoading}
                                onClick={() => handleOnSubmit()}
                                // disabled={!email.IsValid || !captcha.IsValid || componentLoading.buttonLoading}
                                disabled={!submitButtonValid}
                                style={{
                                    cursor: componentLoading.buttonLoading ? "not-allowed" : "pointer",
                                }}
                            />

                        </div>
                    </div>
                </div>
            </div>

        </div>
    );
};

export default SignInPage;