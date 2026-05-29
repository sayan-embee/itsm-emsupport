import React, { useCallback, useContext, useEffect, useRef, useState } from "react";
import { v4 as uuidv4, validate } from "uuid";
import { Button } from "primereact/button";
import { TabMenu } from 'primereact/tabmenu';
import { MenuItem } from 'primereact/menuitem';

import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';

import { getCustomerWiseMasterDataAPI, getDirectlineTokenAPI } from "../../apis/APIList";
import { DirectLine } from "botframework-directlinejs";
import ReactWebChat from "botframework-webchat";
import { StyleOptions } from 'botframework-webchat';
import { useQuery } from '@tanstack/react-query';

//Images
import virtualbotsm from '../../assets/virtual-bot-sm.png';
import chatIcon from '../../assets/chat-icon.svg';
import botIcon from '../../assets/chatIcon.svg';
import avater from '../../assets/user_blank.svg';

import { useAuthStore } from "../../store/authStore";
import { useData } from '../../store/DataProvider';
import UnauthorizedComponent from "../common/UnauthorizedComponent";
import SidePanelComponent from "./SidePanelComponent";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { ROUTE_PATH, HOME_ROUTE_PATH } from "../../router";
import { handleErrorHelper } from "../../helpers/CommonMethods";
import { get, set } from "lodash";
import { Divider } from "primereact/divider";
import LoaderComponent from "../common/LoaderComponent";
import { IState } from "../../Interfaces";
import { useAlertDialog } from "../../store/AlertDialogProvider";
import { ConfirmDialog } from "primereact/confirmdialog";
import { Toast } from "primereact/toast";
import { error } from "console";
import { Message } from "primereact/message";
import { showConfirmDialog } from "../common/ConfirmDialogComponent";
import useIsMobile from "../common/useIsMobile";

interface IDirectLineToken {
    webChatLogId: number | undefined;
    userId: string | undefined;
    token: string | undefined;
    conversationId: string | undefined;
    streamUrl: string | undefined;
    expirationTime: number;
    expirationInLocal?: string;
}

dayjs.extend(utc);

const WebChatPage: React.FC = () => {
    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);

    const location = useLocation();
    const navigate = useNavigate();

    const { data } = useData();
    const { sessionId, sessionExpiresOn } = useAuthStore();

    const { showDialog, hideDialog } = useAlertDialog();

    const isMobile = useIsMobile();
    const isConfirmDialogRef = useRef(false);

    //#region State

    const [directLine, setDirectLine] = useState<DirectLine | null>(null);
    const [tokenExpireTime, setTokenExpireTime] = useState<number | null>(null);

    const [searchParams] = useSearchParams();
    const initialTab = Number(searchParams.get("access")) || 0;

    const [activeTabIndex, setActiveTabIndex] = useState(initialTab);

    const [isUnauthorized, setIsUnauthorized] = useState(initialTab === 1 ? false : true);
    const [isPageLoading, setIsPageLoading] = useState(true);
    const [isChatLoading, setIsChatLoading] = useState(false);

    const [isSidebarVisible, setIsSidebarVisible] = useState(true);
    const [userInitials, setUserInitials] = useState('YOU');
    const [customerDetails, setCustomerDetails] = useState<any>(null);

    const [categoryList, setCategoryList] = useState<any>([]);
    const [subCategoryList, setSubCategoryList] = useState<any>([]);

    const [onboardingCompleted, setOnboardingCompleted] = useState(false);
    const [isWebChatReady, setIsWebChatReady] = useState(false);
    const [isWebChatFailed, setIsWebChatFailed] = useState(false);
    const [isWebChatEnded, setIsWebChatEnded] = useState(false);

    //#endregion

    //#region API

    // const getDirectlineToken_backup = async (): Promise<any> => {
    //     try {
    //         const id = new Date().getTime().toString();
    //         const jsonBody = {
    //             "userId": "dl_" + id,
    //             "userName": id
    //         };
    //         console.log('jsonBody: ', jsonBody);

    //         const response = await getDirectlineTokenAPI(jsonBody);

    //         console.log("getDirectlineTokenAPI: ", response);
    //         if (response && response.data) {

    //             const { token, expires_in, conversationId } = response.data;

    //             setDirectLine(new DirectLine({ token }));

    //             // Dispose of the old DirectLine instance before creating a new one
    //             // setDirectLine((prevDirectLine) => {
    //             //     if (prevDirectLine) {
    //             //         prevDirectLine.end();
    //             //     }
    //             //     return new DirectLine({ token });
    //             // });

    //             // Calculate expiration time in milliseconds
    //             const expirationTime = new Date().getTime() + expires_in * 1000;
    //             setTokenExpireTime(expirationTime);

    //             console.log("DirectLine token set. Conversation ID:", conversationId);
    //         }
    //     } catch (error) {
    //         console.error("Error at getDirectlineToken():", error);
    //     }
    // };


    const getDirectlineToken = async (): Promise<IDirectLineToken> => {
        setIsChatLoading(true);

        setIsWebChatEnded(false);
        setIsWebChatFailed(false);
        setIsWebChatReady(false);
        setOnboardingCompleted(false);

        if (!data || data.length === 0) {
            return {
                webChatLogId: undefined,
                userId: undefined,
                token: undefined,
                conversationId: undefined,
                streamUrl: undefined,
                expirationTime: 0,
                expirationInLocal: ''
            };
        }

        try {
            const newUserId = uuidv4();
            const jsonBody = {
                "userId": "dl_" + newUserId,
                "userName": data?.[0]?.customerName,
                "userEmail": data?.[0]?.customerEmail,
                "sessionId": sessionId,
                // "conversationType": activeTabIndex === 0 ? 'SOP' : (activeTabIndex === 1 ? 'PublicSite' : 'FreshService'),
                "conversationType": 'Miscellaneous',
                // "conversationType": 'SOP',
            };
            console.log('getDirectlineToken-jsonBody: ', jsonBody);

            const response = await getDirectlineTokenAPI(jsonBody);
            console.log("getDirectlineTokenAPI: ", response);

            if (!response || response.status >= 400 || !response.data || !response.data.directLineToken) {
                setIsWebChatFailed(true);
                throw new Error("Failed to fetch DirectLine token");
            }

            const { webChatLogId, userId, directLineToken, expiredOn, conversationId, streamUrl } = response.data;
            console.log("DirectLine token set:", response.data);
            const expirationTime = dayjs.utc(expiredOn).valueOf();
            const localExpiration = dayjs.utc(expirationTime).local().format("hh:mm A");

            setIsChatLoading(false);
            setIsPageLoading(false);

            return {
                webChatLogId: webChatLogId,
                userId: userId,
                token: directLineToken || undefined,
                conversationId: conversationId || undefined,
                streamUrl: streamUrl || undefined,
                expirationTime: expirationTime,
                expirationInLocal: localExpiration
            };
        } catch (error) {
            handleErrorHelper("getDirectlineToken", error);
            setIsChatLoading(false);
            return {
                webChatLogId: undefined,
                userId: undefined,
                token: undefined,
                conversationId: undefined,
                streamUrl: undefined,
                expirationTime: 0,
                expirationInLocal: ''
            };
        }
    };


    const getCustomerWiseMasterData = async (): Promise<any> => {
        try {
            if (!data || data.length === 0) {
                return;
            }

            const dept_Id_List = data.map((e: any) => e.department_id).join(',');
            const crm_Id_List = data.map((e: any) => e.embee_crm_id).join(',');

            const jsonBody = {
                "embee_crm_id_List": crm_Id_List,
                "departmentId_List": dept_Id_List
            };
            console.log('jsonBody: ', jsonBody);

            const response = await getCustomerWiseMasterDataAPI(jsonBody);
            console.log("getCustomerWiseMasterDataAPI: ", response);

            if (response && response.data) {
                const { categoryList, subCategoryList } = response.data;

                if (categoryList?.length > 0 && subCategoryList?.length > 0) {
                    // // Update URL query parameter
                    // const searchParams = new URLSearchParams(location.search);
                    // searchParams.set("access", '0');
                    // navigate({ search: searchParams.toString() }, { replace: true });
                    setIsUnauthorized(false);

                    console.log('categoryList: ', categoryList);
                    console.log('subCategoryList: ', subCategoryList);
                    setCategoryList(categoryList);
                    setSubCategoryList(subCategoryList);

                    setIsWebChatEnded(false);

                    refetchToken();
                }
                else {
                    // Update URL query parameter
                    const searchParams = new URLSearchParams(location.search);
                    searchParams.set("access", '0');
                    navigate({ search: searchParams.toString() }, { replace: true });
                    setIsUnauthorized(true);
                }
            }
            else {
                // Update URL query parameter
                const searchParams = new URLSearchParams(location.search);
                searchParams.set("access", '0');
                navigate({ search: searchParams.toString() }, { replace: true });
                setIsUnauthorized(true);
            }
        } catch (error) {
            console.error("Error at getDirectlineToken():", error);
        }
    };

    // const refreshDirectlineToken = async (): Promise<void> => {
    //     console.log("Refreshing Direct Line token...");
    //     await getDirectlineToken();
    // };

    //#endregion



    //#region UseQuery

    const {
        data: directlineTokenDetails,
        isLoading,
        isError,
        error: tokenError,
        refetch: refetchToken
    } = useQuery<IDirectLineToken, Error>({
        queryKey: ['directlineToken', data],
        queryFn: getDirectlineToken,
        // enabled: !!data, // Only fetch when `data` is available
        enabled: false,
        refetchOnWindowFocus: true,
        staleTime: 10 * 60 * 1000, // Data remains fresh for 10 minutes
        cacheTime: 10 * 60 * 1000, // Cache remains in memory for 10 minutes
        // refetchInterval: (queryData) => {
        //     if (!queryData?.token || !queryData?.expiresInMs) return false;
        //     return Math.max(queryData.expiresInMs - 60 * 1000, 30 * 1000);
        // },
        refetchInterval: (queryData) => {
            if (!queryData?.token || !queryData?.expirationTime) return false;

            const remainingTime = dayjs(queryData.expirationTime).diff(dayjs.utc(), "millisecond") - 5 * 60 * 1000;
            return Math.max(remainingTime, 60 * 1000); // Refresh token 5 min before expiry, with min 60 sec interval
        },
        retry: false
    });

    //#endregion



    //#region Helper Methods

    const handleNavigate = async () => {
        await toggleChat(0);
        await new Promise((resolve) => setTimeout(resolve, 2000));

        navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD);
    };

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    // const handleTabChange = async (index: number) => {
    //     setActiveTabIndex(index);
    //     setDirectLine(null);

    //     // Update URL query parameter
    //     const searchParams = new URLSearchParams(location.search);
    //     searchParams.set("type", index.toString());
    //     navigate({ search: searchParams.toString() }, { replace: true });

    //     await new Promise((resolve) => setTimeout(resolve, 1000));
    //     await refetchToken();
    // };

    const toggleChat = async (index: number) => {

        isConfirmDialogRef.current = true;
        const message = index === 0 ? 'Do you want to end the chat session?' : 'Do you want to start a new chat session?';

        const isConfirmed = await showConfirmDialog({
            header: 'Confirmation',
            message: message,
            icon: 'pi pi-exclamation-triangle',
        });

        if (isConfirmed) {
            setIsChatLoading(true);

            if (index === 0) {
                await new Promise((resolve) => setTimeout(resolve, 2000));
                setIsWebChatEnded(true);
            }
            if (index === 1) {
                await new Promise((resolve) => setTimeout(resolve, 2000));
                // await refetchToken();
                window.location.reload();
            }

            setIsChatLoading(false);
            isConfirmDialogRef.current = false;
        }
        else {
            isConfirmDialogRef.current = false;
            return;
        }
    };


    //#endregion

    useEffect(() => {
        return () => {
            isMountedRef.current = false;
            setDirectLine(null);
            setIsWebChatEnded(false);
        };
    }, []);

    //#region Effects


    useEffect(() => {
        isMountedRef.current = true;
        if (isMountedRef.current) {
            console.log('webchat-customer-data: ', data);

            setCustomerDetails(data);
            const userInitials = data?.[0]?.customerName.split(' ').map((name: string) => name.charAt(0)).join('');
            setUserInitials(userInitials);

            getCustomerWiseMasterData();
        }
    }, []);


    useEffect(() => {
        if (!isMountedRef.current) return;
        if (directlineTokenDetails?.token) {
            // setDirectLine(prevDirectLine => {
            //     prevDirectLine?.end();
            //     return new DirectLine({ token: directlineTokenDetails?.token });
            // });
            setDirectLine(new DirectLine({ token: directlineTokenDetails?.token }));
        }
    }, [directlineTokenDetails?.token]);


    useEffect(() => {
        if (!isMountedRef.current) return;
        if (!directLine) return;

        const messageSubscription = directLine.activity$
            .filter(activity => activity.type === 'message')
            .subscribe(activity => {
                console.log('Received activity:', activity);
            });

        const connectionSubscription = directLine.connectionStatus$
            .subscribe(status => {
                console.log('Connection status:', status);

                if (status === 0) console.log('Uninitialized');
                else if (status === 1) console.log('Connecting...');
                else if (status === 2) console.log('Connected to conversation:', directlineTokenDetails?.conversationId);
                else if (status === 3) console.log('Expired...');
                else if (status === 4) console.log('Failed...');
                else if (status === 5) {
                    // alert('Chat Ended...2, Navigate to Home');
                    console.log('Ended...');
                    window.location.replace(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD);
                    // navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD, { replace: true, state: {} });
                }
            });

        return () => {
            messageSubscription.unsubscribe();
            connectionSubscription.unsubscribe();
        };
    }, [directLine]);



    useEffect(() => {
        if (!isMountedRef.current) return;
        if (!directLine) return;

        const subscription1 = directLine.activity$.subscribe(activity => {
            if (activity.type === "event" && activity.name === "onboardingComplete") {
                console.log("OnMembersAddedAsync finished execution:", activity.value);
                setOnboardingCompleted(true);
            }
        });
        const subscription2 = directLine.activity$.subscribe(activity => {
            if (activity.type === "event" && activity.name === "endChat") {
                // alert('Chat Ended...1');

                setIsWebChatReady(false);
                setOnboardingCompleted(false);
                setIsWebChatFailed(false);
                setIsWebChatEnded(true);
            }
        });
        return () => {
            subscription1.unsubscribe();
            subscription2.unsubscribe();
        };
    }, [directLine]);


    useEffect(() => {
        if (!isMountedRef.current) return;
        if (isWebChatEnded && directlineTokenDetails?.userId && directLine) {
            directLine.postActivity({
                type: "event",
                name: "webchat/end",
                value: {
                    // conversationType: activeTabIndex === 0 ? 'SOP' : (activeTabIndex === 1 ? 'PublicSite' : 'FreshService'),
                    conversationType: 'Miscellaneous',
                    // conversationType: 'SOP',


                    webChatLogId: directlineTokenDetails?.webChatLogId,

                    userId: directlineTokenDetails?.userId,
                    userName: data?.[0]?.customerName,
                    userEmail: data?.[0]?.customerEmail,

                    // categoryList: categoryList,
                    // subCategoryList: subCategoryList,

                    // selectedCategory: categoryList?.length > 0 ? categoryList[0] : null,
                    // selectedSubCategory: subCategoryList?.length > 0 ? subCategoryList[0] : null
                },
                from: {
                    id: directlineTokenDetails?.userId,
                    name: data?.[0]?.customerName
                }
            }).subscribe(
                id => {
                    console.log("User data sent to bot", id);
                    setDirectLine(null);
                },
                error => console.error("Error sending user data", error)
            );
        }
    }, [isWebChatEnded, directLine, directlineTokenDetails, data]);


    useEffect(() => {
        if (!isMountedRef.current) return;
        if (onboardingCompleted && directlineTokenDetails?.userId && directLine && data?.[0]?.customerName && data?.[0]?.customerEmail) {
            if (directLine) {
                directLine.postActivity({
                    type: "event",
                    name: "webchat/join",
                    value: {
                        // conversationType: activeTabIndex === 0 ? 'SOP' : (activeTabIndex === 1 ? 'PublicSite' : 'FreshService'),
                        conversationType: 'Miscellaneous',
                        // conversationType: 'SOP',

                        webChatLogId: directlineTokenDetails?.webChatLogId,

                        userId: directlineTokenDetails?.userId,
                        userName: data?.[0]?.customerName,
                        userEmail: data?.[0]?.customerEmail,

                        categoryList: categoryList,
                        subCategoryList: subCategoryList,

                        selectedCategory: categoryList?.length === 1 ? categoryList[0] : null,
                        selectedSubCategory: subCategoryList?.length === 1 ? subCategoryList[0] : null
                    },
                    from: {
                        id: directlineTokenDetails?.userId,
                        name: data?.[0]?.customerName
                    }
                }).subscribe(
                    id => {
                        console.log("User data sent to bot", id);
                        setIsWebChatReady(true);
                    },
                    error => { console.error("Error sending user data", error); setIsWebChatFailed(true); }
                );
            }
        }
    }, [onboardingCompleted, directLine, directlineTokenDetails, data, categoryList, subCategoryList]);


    useEffect(() => {
        if (!isMountedRef.current) return;
        if (directLine && !isLoading && (isError || isWebChatFailed)) {
            setIsWebChatReady(false);
            setOnboardingCompleted(false);
        }
    }, [directLine, isLoading, isError, isWebChatFailed]);

    useEffect(() => {
        if (!isMountedRef.current) return;
        if (isWebChatFailed) {
            setIsWebChatReady(false);
            setOnboardingCompleted(false);

            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD, { replace: true, state: { errorMsg: "Something went wrong. Unable to start chat. Please try again." } });
        }
    }, [isWebChatFailed, navigate]);

    // useEffect(() => {
    //     // Setup interval to refresh token before it expires
    //     if (tokenExpireTime) {
    //         const refreshInterval = setInterval(() => {
    //             const currentTime = new Date().getTime();

    //             // Refresh the token 60 seconds before it expires
    //             if (tokenExpireTime - currentTime <= 60 * 1000) {
    //                 refreshDirectlineToken();
    //             }
    //         }, 30 * 1000); // Check every 30 seconds

    //         return () => clearInterval(refreshInterval);
    //     }
    // }, [tokenExpireTime]);

    //#endregion

    // const items: MenuItem[] = [
    //     { label: 'SOP', icon: 'pi pi-home' },
    //     { label: 'Web', icon: 'pi pi-chart-line' },
    //     { label: 'Freshservice', icon: 'pi pi-list' }
    // ];

    const styleOptions: StyleOptions = {
        hideUploadButton: true,
        hideScrollToEndButton: false,
        bubbleBorderRadius: 12,
        // bubbleMaxWidth: 800,
        // bubbleMinWidth: 800,
        bubbleMaxWidth: undefined,
        bubbleMinWidth: undefined,
        suggestedActionBorderRadius: 6,
        emojiSet: false,
        backgroundColor: '#F5F5F5',
        avatarSize: 32,
        botAvatarInitials: '',
        userAvatarInitials: userInitials,
        bubbleBackground: '#FFFFFF',
        bubbleTextColor: '#000000',
        bubbleFromUserBackground: '#d2e7f9',
        bubbleFromUserTextColor: '#000000',
        richCardWrapTitle: true,
        timestampFormat: 'relative',
        botAvatarBackgroundColor: '#10446f',
        botAvatarImage: botIcon
        // bubbleTextSize: '14px',
        // suggestedActionTextSize: '14px',
    };

    if (isUnauthorized) {
        return (
            <UnauthorizedComponent message="You are not currently associated with an active contract. Please contact the administrator." severity="warn" redirect={true} />
        );
    }

    return (
        <div className="container-fluid px-md-5 px-2 mb-2">

            <Toast ref={toastRef} />

            {isPageLoading && <LoaderComponent />}

            <div className="row">
                <div className={`col-md-${isSidebarVisible ? '9' : '12'}`}>

                    {/* <div className="page-header d-flex align-items-center">
                        <Button className="back-btn" icon="pi pi-arrow-left" rounded aria-label="Back" onClick={handleNavigate} />
                        <div className="app-icon">
                            <img src={virtualbotsm} alt="" />
                        </div>
                        <h6 className="mb-0">Self-Help Bot</h6>
                    </div> */}

                    {/* <div className="row">
                        <div className={`col-md-${isSidebarVisible ? '9' : '12'}`}>
                            <TabMenu model={items} activeIndex={activeTabIndex} onTabChange={(e) => handleTabChange(e.index)} />
                        </div>
                    </div> */}

                    <div className="chat-wrapper">
                        <div className="top-part d-flex justify-content-between align-items-center">
                            <div className="d-flex align-items-center">
                                <img src={chatIcon} alt="" />
                                <p className="text-xs mb-0 ms-2">Chat
                                    {/* <small>{isWebChatReady && directlineTokenDetails?.expirationInLocal ? `(Session automatically ends on ${directlineTokenDetails?.expirationInLocal})` : ''}</small> */}
                                </p>
                            </div>
                            <div className="user-info d-flex align-items-center">

                                {
                                    directLine && isWebChatReady && !isWebChatEnded && (
                                        <div className="d-flex flex-column justify-content-center align-items-center">
                                            <Button className="btn btn-danger" onClick={() => toggleChat(0)}
                                                label='End Chat'
                                                loading={isLoading}
                                            />
                                        </div>
                                    )
                                }
                                {/* {
                                    !directLine && !isWebChatReady && isWebChatEnded && (
                                        <div className="d-flex flex-column justify-content-center align-items-center me-2">
                                            <Button className="btn btn-outline-primary" onClick={() => toggleChat(1)}
                                                label='New Chat'
                                                loading={isLoading}
                                            />
                                        </div>
                                    )
                                } */}

                                {/* <Divider layout="vertical">
                                </Divider>

                                <div className="me-2 d-flex align-items-center">
                                    <small className="text-xs me-1">{customerDetails?.[0]?.customerName}</small>
                                    <div className="avater">
                                        <img src={avater} alt="Name" />
                                    </div>
                                </div> */}
                                {/* <div>
                                    <Button icon="pi pi-arrow-up-right-and-arrow-down-left-from-center" rounded text onClick={() => setIsSidebarVisible(!isSidebarVisible)}
                                        tooltip={isSidebarVisible ? 'Expand' : 'Collapse'} tooltipOptions={{ position: 'top' }}
                                    />
                                </div> */}
                            </div>
                        </div>

                        <div className="body-part" style={{ userSelect: 'none' }}>

                            {
                                (isChatLoading || isLoading) && (
                                    <LoaderComponent />
                                )
                            }

                            {/* Failed to get token */}
                            {/* {
                                !isLoading && (isError || isWebChatFailed || isWebChatEnded) && (
                                    <div className="d-flex flex-column justify-content-center align-items-center">
                                        <Button className="primary" onClick={() => refetchToken()}
                                            label={isLoading ? '' : 'Reconnect'}
                                            loading={isLoading}
                                        />
                                    </div>
                                )
                            } */}

                            {/* {
                                !isWebChatReady && !isWebChatEnded &&
                                (
                                    <div className="d-flex flex-column justify-content-center align-items-center webchatOverlay">
                                        <div className="message-container">
                                            <Message severity={"info"} text={`Initiating chat session`}
                                                icon={<i className="pi pi-spin pi-cog me-1"></i>} />
                                        </div>
                                    </div>
                                )
                            } */}


                            {
                                isWebChatEnded && (
                                    <div className="d-flex flex-column justify-content-center align-items-center webchatOverlay">
                                        {/* <p>Existing chat has ended. Click 'New Chat' to begin a new conversation.</p> */}
                                        <Message severity={"success"} text={`Session has ended. Please do not hesitate to contact us anytime. Have a great day!`} />
                                        {/* <p>Session has ended. Please do not hesitate to contact us anytime. Have a great day!</p> */}
                                    </div>
                                )
                            }



                            {/* Token is available */}
                            {
                                directLine && (
                                    <ReactWebChat
                                        directLine={directLine}
                                        userID={directlineTokenDetails?.userId}
                                        username={data?.[0]?.customerName}
                                        styleOptions={styleOptions}
                                        disabled={!isWebChatReady}
                                    />
                                )
                            }

                        </div>
                    </div>

                </div>

                <div className="col-md-3" style={{ display: isSidebarVisible ? 'block' : 'none' }}>
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
                                    openfrom="webchat"
                                />
                            )
                        }
                    </div>
                </div>

            </div>
        </div>
    );
};

export default WebChatPage;