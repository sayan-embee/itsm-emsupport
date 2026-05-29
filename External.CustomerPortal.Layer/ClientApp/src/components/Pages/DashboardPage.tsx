import { createRef, useCallback, useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { InputText } from "primereact/inputtext";
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import OtpInput from "react-otp-input";
import { Toast } from 'primereact/toast';
import dayjs from "dayjs";
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

import { HOME_ROUTE_PATH, ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { useAlertDialog } from '../../store/AlertDialogProvider';
import { IState } from '../../Interfaces';
import { EMAIL_REGEX } from '../../Constants';
import { handleAlphanumericKeyPress, handleEmailKeyPress, handleErrorHelper, handleNumericKeyPress, MaskEmail, MaskText, UseRegex } from '../../helpers/CommonMethods';
import OtpTimerComponent from '../common/OtpTimerComponent';
import { GetCustomerDetailsAPI, getNewOTPAPI, getTicketDetailsAPI, logoutAPI, verifyOTPAPI } from '../../apis/APIList';
import { CommonMessage, showErrorToast, showSuccessToast, showWarningToast, toastMessageType } from '../common/ToastComponent';
import LoaderComponent from '../common/LoaderComponent';
import { v4 } from 'uuid';
import OtpSkeleton from '../skeletons/OtpSkeleton';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import SidePanelComponent from './SidePanelComponent';
import HeaderComponent from './HeaderComponent';
import TicketCardsComponent from './TicketCardsComponent';
import { useData } from '../../store/DataProvider';
import UnauthorizedComponent from '../common/UnauthorizedComponent';
import { set } from 'lodash';
import useIsMobile from '../common/useIsMobile';


//#region Interfaces

enum TextType {
    TicketId = "TicketId"
}

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

interface ILoadingState {
    pageLoading: boolean;
    cardLoading: boolean;
    buttonLoading: boolean;
}

//#endregion

const DashboardPage = () => {
    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const { showDialog, hideDialog } = useAlertDialog();

    const isMobile = useIsMobile();

    const { data } = useData();
    const location = useLocation();
    const navigate = useNavigate();

    const { errorMsg } = location.state || {};

    //#region State
    const [isJwtValid, setIsJwtValid] = useState(true);
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const signOut = useAuthStore((state) => state.signOut);

    const [sidebarIsVisible, setSidebarIsVisible] = useState<boolean>(false);
    const [showButton, setShowButton] = useState(false);
    const [reportDialogIsVisible, setReportDialogIsVisible] = useState<boolean>(false);

    const [customerDetails, setCustomerDetails] = useState<any>(null);

    const [ticketId, setTicketId] = useState<IState>({
        Value: undefined,
        IsRequired: false,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [startDate, setStartDate] = useState<IState>({
        Value: undefined,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [endDate, setEndDate] = useState<IState>({
        Value: undefined,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: false,
        cardLoading: true,
        buttonLoading: false
    });

    //#endregion


    //#region API



    //#endregion


    //#region Helper Methods

    const toggleSidebar = () => {
        setSidebarIsVisible(prev => !prev);
    };

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    const handleSignOut = useCallback(async () => {
        await logoutAPI();
        signOut();
        navigate(ROUTE_PATH.SIGN_IN);
    }, [signOut, navigate]);

    const resetPage = async () => {
        setComponentLoading((prevState) => ({ ...prevState, buttonLoading: false, pageLoading: false }));
    };

    const handleNavigate = async (type: 'Webchat' | 'Ticket') => {
        if (type === 'Ticket') {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.TICKETS, { state: { ticketIdProp: ticketId.Value, startDateProp: startDate.Value, endDateProp: endDate.Value } });
        } else if (type === 'Webchat') {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.WEBCHAT + '?access=1');
        }
    };

    //#endregion


    //#region OnChange Methods

    const handleDateChange = (e: any, type: DateType) => {
        console.log("handleDateChange: ", e);

        // const MAX_DAY_DIFF = 30;

        switch (type) {
            case DateType.FromDate:
                {
                    setStartDate({ ...startDate, Value: e, IsValid: true, ErrorMessage: '' });
                    if (e && endDate.Value && dayjs(endDate.Value).isBefore(dayjs(e))) {
                        const newEndDate = dayjs(e).add(1, 'day').toDate();
                        setEndDate({ ...endDate, Value: newEndDate, IsValid: true, ErrorMessage: '' });
                    }
                    else if (e && !endDate.Value) {
                        const newEndDate = dayjs(e).add(1, 'day').toDate();
                        setEndDate({ ...endDate, Value: newEndDate, IsValid: true, ErrorMessage: '' });
                    }
                    // setEndDate({ ...endDate, Value: undefined, IsValid: true, ErrorMessage: '' });

                    setTicketId({ ...ticketId, Value: '', IsValid: true, ErrorMessage: '' });
                    break;
                }

            case DateType.ToDate:
                {
                    // if (dayjs(e).diff(dayjs(selectedFromDate), 'day') > MAX_DAY_DIFF) {
                    //     setToDate(dayjs(selectedFromDate).add(MAX_DAY_DIFF, 'day').toDate());
                    // }
                    setEndDate({ ...endDate, Value: e, IsValid: true, ErrorMessage: '' });
                    setTicketId({ ...ticketId, Value: '', IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    const handleTextChange = (e: any, type: TextType) => {
        console.log("handleTextChange: ", e);

        switch (type) {
            case TextType.TicketId:
                {
                    setTicketId({ ...ticketId, Value: e, IsValid: true, ErrorMessage: '' });
                    setStartDate({ ...startDate, Value: undefined, IsValid: true, ErrorMessage: '' });
                    setEndDate({ ...endDate, Value: undefined, IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    // const handleOnSubmit = async () => {
    //     const isValid = await validateFields('VerifyOTP');
    //     if (!isValid) {
    //         return;
    //     }
    // };

    //#endregion


    //#region Validation Methods

    // const validateFields = async (type: 'NewOTP' | 'VerifyOTP') => {
    //     let result = false;
    //     switch (type) {
    //         case 'NewOTP': {
    //             if (email.Value === '') {
    //                 setEmail({ ...email, IsValid: false, ErrorMessage: 'Email is required' });
    //             } else if (!UseRegex(EMAIL_REGEX, email.Value)) {
    //                 setEmail({ ...email, IsValid: false, ErrorMessage: 'Please enter a valid email address' });
    //             } else {
    //                 setEmail({ ...email, IsValid: true, ErrorMessage: '' });
    //                 result = true;
    //             }

    //             break;
    //         }

    //         case 'VerifyOTP': {
    //             if (!otp.Value) {
    //                 setOtp({ ...otp, IsValid: false, ErrorMessage: 'Enter OTP' });
    //             } else if (otp.Value && (otp.Value).toString().length < 6) {
    //                 setOtp({ ...otp, IsValid: false, ErrorMessage: 'Please enter 6 digit OTP' });
    //             } else {
    //                 setOtp({ ...otp, IsValid: true, ErrorMessage: '' });
    //                 result = true;
    //             }

    //             break;
    //         }
    //     }

    //     return result;
    // };

    //#endregion


    //#region Effects

    // useEffect(() => {
    //     const syncAuthState = (event: StorageEvent) => {
    //         if (event.key === 'auth-storage' && event.newValue) {
    //             try {
    //                 const newState = JSON.parse(event.newValue);
    //                 if (newState?.state) {
    //                     useAuthStore.setState(newState.state);
    //                 }
    //             } catch (error) {
    //                 console.error('Error parsing auth-storage:', error);
    //             }
    //         }
    //     };

    //     window.addEventListener('storage', syncAuthState);
    //     return () => window.removeEventListener('storage', syncAuthState);
    // }, []);

    useEffect(() => {
        const timer = setTimeout(() => {
            setShowButton(true);
        }, 5000);

        return () => clearTimeout(timer);
    }, []);


    useEffect(() => {
        if (hasHydrated && (!data)) {
            navigate(ROUTE_PATH.HOME);
        }
    }, [hasHydrated, data, navigate]);


    useEffect(() => {
        if (location.state?.errorMsg) {
            handleShowAlert('Attention Required', location.state?.errorMsg);
            const timeout = setTimeout(() => {
                navigate(location.pathname, { replace: true, state: {} });
            }, 0);
            return () => clearTimeout(timeout);
        }
    }, [handleShowAlert, location, navigate]);


    useEffect(() => {
        isMountedRef.current = true;
        if (isMountedRef.current) {
            console.log('dashboard-customer-data: ', data);
            setCustomerDetails(data);
        }
    }, []);

    // useEffect(() => {
    //     if (!isJwtValid) {
    //         navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
    //     }
    // }, [isJwtValid]);

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
        <div className="container-fluid px-md-5 px-2 mb-4">
            <Toast ref={toastRef} />
            <div className="row">

                <div className="col-md-9">

                    <TicketCardsComponent />

                    <div className="row">

                        <div className="col-md-6 mb-2">
                            <div className="dash-card-horizontal odd mb-4">
                                <div className="d-flex align-items-center mb-4">
                                    <div className="icon">
                                        <img src={virtualbot} alt="" />
                                    </div>
                                    <h4 className='text-blue'>Your Virtual IT Support Assistant—Always Ready to Help!</h4>
                                </div>
                                <div>
                                    <p>Got questions? Our smart chatbot is here 24/7 to assist and guide you to the right solutions.</p>
                                    <Button className="primary" label="Chat Now" onClick={() => handleNavigate('Webchat')} disabled={!showButton} />
                                    {/* <Button className='ms-2' outlined label="Create Ticket" /> */}
                                </div>
                            </div>
                        </div>

                        <div className="col-md-6 mb-2">
                            <div className="dash-card-horizontal even">
                                <div className="d-flex align-items-center mb-4">
                                    <div className="icon">
                                        <img src={report} alt="" />
                                    </div>
                                    <h4 className='text-blue'>Your Support History, Simplified and Accessible Anytime</h4>
                                </div>
                                <div>
                                    <p>Looking for insights on your tickets? Search here.</p>
                                    <div className="mb-3 form report-form web">
                                        <Calendar className="me-2"
                                            inputId="startDate"
                                            showIcon
                                            placeholder={`From Date *`}
                                            // minDate={FromMinDateTime}
                                            // maxDate={selectedToDate ?? null}
                                            value={startDate.Value}
                                            dateFormat="dd/mm/yy"
                                            onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                            invalid={startDate.IsValid}
                                        />
                                        <Calendar className="me-2"
                                            inputId="endDate"
                                            showIcon
                                            placeholder="To Date *"
                                            minDate={startDate.Value ?? null}
                                            // maxDate={selectedToDate ?? null}
                                            value={endDate.Value}
                                            dateFormat="dd/mm/yy"
                                            onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                                            invalid={endDate.IsValid}
                                        />
                                        <span className="me-2 text-grey">or</span>
                                        <span>
                                            <div className="p-inputgroup">
                                                <InputText placeholder="Enter Ticket ID"
                                                    value={ticketId.Value}
                                                    onChange={(e) => handleTextChange(e.target.value, TextType.TicketId)}
                                                    onKeyDown={handleNumericKeyPress}
                                                    maxLength={8}
                                                    minLength={6}
                                                />
                                                <span className="p-inputgroup-addon"><img src={ticket} alt="" /></span>
                                            </div>
                                        </span>
                                    </div>
                                    <Button className='primary lg-device' label="Search" onClick={() => handleNavigate('Ticket')} />
                                    <Button className='primary sm-device' label="Search" onClick={() => setReportDialogIsVisible(true)} />
                                </div>

                            </div>
                        </div>

                    </div>

                </div>

                <Dialog header="Search Tickets" visible={reportDialogIsVisible} style={{ width: '80vw' }} onHide={() => setReportDialogIsVisible(false)}>
                    <div className="text-center">
                        <div className="mb-3 form report-form gap-2">
                            <Calendar
                                className="me-3"
                                inputId="startDate"
                                showIcon
                                placeholder={`From Date *`}
                                // minDate={FromMinDateTime}
                                // maxDate={selectedToDate ?? null}
                                value={startDate.Value}
                                dateFormat="dd/mm/yy"
                                onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                invalid={startDate.IsValid}
                            />
                            <Calendar
                                className="me-3"
                                inputId="endDate"
                                showIcon
                                placeholder="To Date *"
                                minDate={startDate.Value ?? null}
                                // maxDate={selectedToDate ?? null}
                                value={endDate.Value}
                                dateFormat="dd/mm/yy"
                                onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                                invalid={endDate.IsValid}
                            />
                            <span>or</span>
                            <div className="p-inputgroup">
                                <InputText placeholder="Enter Ticket ID"
                                    value={ticketId.Value}
                                    onChange={(e) => handleTextChange(e.target.value, TextType.TicketId)}
                                    onKeyDown={handleNumericKeyPress}
                                    maxLength={8}
                                    minLength={6}
                                />
                                <span className="p-inputgroup-addon"><img src={ticket} alt="" /></span>
                            </div>
                        </div>
                        <Button className='primary sm-device' label="Search" onClick={() => handleNavigate('Ticket')} />
                    </div>
                </Dialog>

                <div className="col-md-3 mb-2">
                    <div className="user-section flex-column justify-content-between">
                        {
                            !isMobile &&
                            (
                                <SidePanelComponent
                                    customerDetails={customerDetails}
                                    userName={customerDetails?.[0]?.customerName}
                                    userEmail={customerDetails?.[0]?.customerEmail}
                                    userMobile={customerDetails?.[0]?.customerPhone}
                                    officeName={customerDetails?.[0]?.customerAddress}
                                />
                            )
                        }
                    </div>
                </div>

            </div>
        </div>
    );
};

export default DashboardPage;