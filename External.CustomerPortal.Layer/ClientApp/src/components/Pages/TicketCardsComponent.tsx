import React, { useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Message } from 'primereact/message';
import { Sidebar } from 'primereact/sidebar';
import dayjs from "dayjs";
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
import { useQuery } from '@tanstack/react-query';

// Images
import logo from '../../assets/Logo.svg';
import avater from '../../assets/user_blank.svg';
import office from '../../assets/corporate.svg';
import email from '../../assets/email-icon.svg';
import phone from '../../assets/phone.svg';
import alltkt from '../../assets/alltkt_lg.svg';
import closetkt from '../../assets/closetkt_lg.svg';
import opentkt from '../../assets/opentkt_lg.svg';
import resonse from '../../assets/resonse-icon.svg';
import resolution from '../../assets/resolution-icon.svg';

import { HOME_ROUTE_PATH, HOME_ROUTES, ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import { ConfirmDialog } from 'primereact/confirmdialog';
import { getTicketDetailsAPI } from '../../apis/APIList';
import { handleErrorHelper } from '../../helpers/CommonMethods';
import { showErrorToast, toastMessageType } from '../common/ToastComponent';
import { useData } from '../../store/DataProvider';
import Carousel from 'react-multi-carousel';
import 'react-multi-carousel/lib/styles.css';
import TicketCountsSkeleton from '../skeletons/TicketCountsSkeleton';

interface TicketCardsComponentProps {

}

interface ILoadingState {
    pageLoading: boolean;
    cardLoading: boolean;
    buttonLoading: boolean;
}

interface ICardState {
    TotalTickets: number;
    ClosedTickets: number;
    OpenTickets: number;
    ResponseViolated: number;
    ResolutionViolated: number;
}

interface IPercentCardState {
    TotalTicketsPercent: {
        value: string;
        class: string
    };
    ClosedTicketsPercent: {
        value: string;
        class: string
    };
    OpenTicketsPercent: {
        value: string;
        class: string
    };
    ResponseViolatedPercent: {
        value: string;
        class: string
    };
    ResolutionViolatedPercent: {
        value: string;
        class: string
    };
}

const TicketCardsComponent: React.FC<TicketCardsComponentProps> = () => {

    const toastRef = useRef<Toast>(null);

    const { data } = useData();
    const location = useLocation();
    const navigate = useNavigate();

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: false,
        cardLoading: true,
        buttonLoading: false
    });

    //#region API

    const getTicketDetailsForCount = async (): Promise<ICardState> => {
        try {
            if (!data || data.length === 0) {
                return {
                    TotalTickets: 0,
                    ClosedTickets: 0,
                    OpenTickets: 0,
                    ResponseViolated: 0,
                    ResolutionViolated: 0
                };
            }

            const dept_Id_List = data.map((e: any) => e.department_id).join(',');
            const jsonBody = {
                "transactionType": 'Count',
                "departmentId_List": dept_Id_List,
                // "departmentId_List": '27000586401,27001473235',
                "fromDate": dayjs().startOf('month').format("DD-MM-YYYY"),
                "toDate": dayjs().format("DD-MM-YYYY"),
            };

            console.log("getTicketDetailsForCount-JSON: ", jsonBody);
            const response = await getTicketDetailsAPI(jsonBody);
            console.log("getTicketDetailsForCount-count: ", response);

            if (!response || response.status >= 400 || !response.data) {
                throw new Error(response?.data?.message);
            }

            const { ticketList } = response.data;
            const [{ TotalTickets, ClosedTickets, OpenTickets, ResponseViolated, ResolutionViolated }] = ticketList;

            return {
                TotalTickets,
                ClosedTickets,
                OpenTickets,
                ResponseViolated,
                ResolutionViolated
            };
        }
        catch (error) {
            handleErrorHelper("getTicketDetailsForCount", error);
            return {
                TotalTickets: 0,
                ClosedTickets: 0,
                OpenTickets: 0,
                ResponseViolated: 0,
                ResolutionViolated: 0
            };
        }
    };

    const getTicketDetailsForPercent = async (): Promise<IPercentCardState> => {
        try {
            if (!data || data.length === 0) {
                return {
                    TotalTicketsPercent: {
                        value: '0%',
                        class: ''
                    },
                    ClosedTicketsPercent: {
                        value: '0%',
                        class: ''
                    },
                    OpenTicketsPercent: {
                        value: '0%',
                        class: ''
                    },
                    ResponseViolatedPercent: {
                        value: '0%',
                        class: ''
                    },
                    ResolutionViolatedPercent: {
                        value: '0%',
                        class: ''
                    }
                };
            }

            const dept_Id_List = data.map((e: any) => e.department_id).join(',');
            const jsonBody = {
                "transactionType": 'PercentageChange',
                "departmentId_List": dept_Id_List,
                // "departmentId_List": '27000586401,27001473235',
                "fromDate": dayjs().startOf('month').format("DD-MM-YYYY"),
                "toDate": dayjs().format("DD-MM-YYYY"),
            };

            const response = await getTicketDetailsAPI(jsonBody);
            console.log("getTicketDetailsForPercent-count: ", response);

            if (!response || response.status >= 400 || !response.data) {
                throw new Error(response?.data?.message);
            }

            const { ticketList } = response.data;
            const [{ TotalTicketsPercentageChange, ClosedTicketsPercentageChange, OpenTicketsPercentageChange, ResponseViolatedPercentageChange, ResolutionViolatedPercentageChange }] = ticketList;

            return {
                TotalTicketsPercent: {
                    value: TotalTicketsPercentageChange >= 0 ? `+${TotalTicketsPercentageChange}%` : `${TotalTicketsPercentageChange}%`,
                    class: TotalTicketsPercentageChange >= 0 ? 'pi pi-arrow-up-right' : 'pi pi-arrow-down-left'
                },
                ClosedTicketsPercent: {
                    value: ClosedTicketsPercentageChange >= 0 ? `+${ClosedTicketsPercentageChange}%` : `${ClosedTicketsPercentageChange}%`,
                    class: ClosedTicketsPercentageChange >= 0 ? 'pi pi-arrow-up-right' : 'pi pi-arrow-down-left'
                },
                OpenTicketsPercent: {
                    value: OpenTicketsPercentageChange >= 0 ? `+${OpenTicketsPercentageChange}%` : `${OpenTicketsPercentageChange}%`,
                    class: OpenTicketsPercentageChange >= 0 ? 'pi pi-arrow-up-right' : 'pi pi-arrow-down-left'
                },
                ResponseViolatedPercent: {
                    value: ResponseViolatedPercentageChange >= 0 ? `+${ResponseViolatedPercentageChange}%` : `${ResponseViolatedPercentageChange}%`,
                    class: ResponseViolatedPercentageChange >= 0 ? 'pi pi-arrow-up-right' : 'pi pi-arrow-down-left'
                },
                ResolutionViolatedPercent: {
                    value: ResolutionViolatedPercentageChange >= 0 ? `+${ResolutionViolatedPercentageChange}%` : `${ResolutionViolatedPercentageChange}%`,
                    class: ResolutionViolatedPercentageChange >= 0 ? 'pi pi-arrow-up-right' : 'pi pi-arrow-down-left'
                }
            }
        }
        catch (error) {
            handleErrorHelper("getTicketDetailsForPercent", error);
            return {
                TotalTicketsPercent: {
                    value: '0%',
                    class: ''
                },
                ClosedTicketsPercent: {
                    value: '0%',
                    class: ''
                },
                OpenTicketsPercent: {
                    value: '0%',
                    class: ''
                },
                ResponseViolatedPercent: {
                    value: '0%',
                    class: ''
                },
                ResolutionViolatedPercent: {
                    value: '0%',
                    class: ''
                }
            };
        }
    };

    //#endregion


    //#region UseQuery

    const {
        data: ticketDetails,
        isLoading,
        isError,
        error,
        refetch
    } = useQuery<ICardState>({
        queryKey: ['ticketDetailsForCount', data],
        queryFn: getTicketDetailsForCount,
        enabled: !!data,                         // Only run when `data` is available
        refetchOnWindowFocus: true,              // Revalidate on window focus
        staleTime: 10 * 60 * 1000,               // Data stays fresh for 10 minutes
        cacheTime: 30 * 60 * 1000,               // Cache lasts for 30 minutes
        retry: false
    });

    // const {
    //     data: ticketDetailsPercent,
    //     isLoading: isLoadingPercent,
    //     isError: isErrorPercent,
    //     error: errorPercent,
    //     refetch: refetchPercent
    // } = useQuery<IPercentCardState>({
    //     queryKey: ['ticketDetailsForPercent', data],
    //     queryFn: getTicketDetailsForPercent,
    //     enabled: !!data,                            // Only run when `data` is available
    //     refetchOnWindowFocus: true,                  // Revalidate on window focus
    //     staleTime: 10 * 60 * 1000,                   // Data stays fresh for 10 minutes
    //     cacheTime: 30 * 60 * 1000,                   // Cache lasts for 30 minutes
    //     retry: false
    // });


    //#endregion



    //#region Helpers

    const handleNavigate = async (statusId?: number, type?: string) => {
        if (statusId) {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.TICKETS + `?sid=${statusId}`);
        }
        else if (type) {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.TICKETS + `?type=${type}`);
        }
        else {
            navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.TICKETS);
        }
    };

    //#endregion

    //#region Effect

    useEffect(() => {
        console.log('cardcomponent-customer-data: ', data);
        if (data) {
            refetch();        // Refetch count details
            //refetchPercent(); // Refetch percentage details
        }
    }, [data]);



    //#endregion

    const responsive: any = {
        desktop: {
            breakpoint: {
                max: 3000,
                min: 1024
            },
            items: 6,
            slidesToSlide: 1,
            partialVisibilityGutter: 40,
            visible: 6,
        },
        mobile: {
            breakpoint: {
                max: 464,
                min: 0
            },
            items: 1,
            slidesToSlide: 2,
            partialVisibilityGutter: 30
        },
        tablet: {
            breakpoint: {
                max: 1024,
                min: 200
            },
            items: 1,
            slidesToSlide: 1,
            partialVisibilityGutter: 30
        }
    };

    if (isLoading) {
        return (
            <TicketCountsSkeleton />
        );
    }

    if (isError) {
        // const errorMessage = (error as Error).message;
        // showErrorToast(toastRef, toastMessageType.Error, errorMessage);
        navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
    }

    return (
        <>
            <Toast ref={toastRef} />
            <div className='mb-3 mb-lg-0'>
                <Carousel className="dash-carousel" responsive={responsive} showDots={true} infinite={true} autoPlay={true} containerClass="carousel-with-custom-dots" autoPlaySpeed={3000} removeArrowOnDeviceType={["tablet", "mobile"]}>
                    <div className='item'>
                        <div className="dash-stats-card alltkt">
                            <div>
                                <div className="d-flex align-items-start">
                                    <img className="icon" src={alltkt} alt="" />
                                    <div>
                                        <h2 className="text-blue m-0">{ticketDetails?.TotalTickets}</h2>
                                    </div>
                                </div>
                                <p className="mb-2 mt-2">All Tickets This Month</p>
                            </div>
                            <div className='d-flex justify-content-between'>
                                {/* <div className="d-flex align-items-center text-xs status-move">
                                    <i className={ticketDetailsPercent?.TotalTicketsPercent.class}></i>
                                    <span className="text-xs">{ticketDetailsPercent?.TotalTicketsPercent.value} this week</span>
                                </div> */}
                                <div className="d-flex align-items-end">
                                    <Button icon="pi pi-arrow-right" rounded onClick={() => handleNavigate()} />
                                </div>
                            </div>

                        </div>
                    </div>
                    <div className='item'>
                        <div className="dash-stats-card closetkt">
                            <div>
                                <div className="d-flex align-items-start">
                                    <img className="icon" src={closetkt} alt="" />
                                    <div>
                                        <h2 className="text-blue m-0">{ticketDetails?.ClosedTickets}</h2>
                                    </div>
                                </div>
                                <p className="mb-2 mt-2">Closed/ Resolved Tickets This Month</p>
                            </div>
                            <div className='d-flex justify-content-between'>
                                {/* <div className="d-flex align-items-center text-xs status-move">
                                    <i className={ticketDetailsPercent?.ClosedTicketsPercent.class}></i>
                                    <span className="text-xs">{ticketDetailsPercent?.ClosedTicketsPercent.value} this week</span>
                                </div> */}
                                <div className="d-flex align-items-end">
                                    <Button icon="pi pi-arrow-right" rounded onClick={() => handleNavigate(5)} />
                                </div>
                            </div>

                        </div>
                    </div>
                    <div className='item'>
                        <div className="dash-stats-card opentkt">
                            <div>
                                <div className="d-flex align-items-start">
                                    <img className="icon" src={opentkt} alt="" />
                                    <div>
                                        <h2 className="text-blue m-0">{ticketDetails?.OpenTickets}</h2>
                                    </div>
                                </div>
                                <p className="mb-2 mt-2">Active Tickets This Month</p>
                            </div>
                            <div className='d-flex justify-content-between'>
                                {/* <div className="d-flex align-items-center text-xs status-move">
                                    <i className={ticketDetailsPercent?.OpenTicketsPercent.class}></i>
                                    <span className="text-xs">{ticketDetailsPercent?.OpenTicketsPercent.value} this week</span>
                                </div> */}
                                <div className="d-flex align-items-end">
                                    <Button icon="pi pi-arrow-right" rounded onClick={() => handleNavigate(2)} />
                                </div>
                            </div>

                        </div>
                    </div>
                    {/* <div className='item'>
                        <div className="dash-stats-card response">
                            <div>
                                <div className="d-flex align-items-start">
                                    <img className="icon" src={resonse} alt="" />
                                    <div>
                                        <h2 className="text-blue m-0">{ticketDetails?.ResponseViolated}</h2>
                                    </div>
                                </div>
                                <p className="mb-2 mt-2">SLA Violated (Response)</p>
                            </div>
                            <div className="d-flex justify-content-between">
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className={ticketDetailsPercent?.ResponseViolatedPercent.class}></i>
                                    <span className="text-xs">{ticketDetailsPercent?.ResponseViolatedPercent.value} this week</span>
                                </div>
                                <div className="d-flex align-items-end">
                                    <Button icon="pi pi-arrow-right" rounded onClick={() => handleNavigate(0, 'SLA_Violated_Response')} />
                                </div>
                            </div>
                        </div>
                    </div> */}
                    {/* <div className='item'>
                        <div className="dash-stats-card resolution">
                            <div>
                                <div className="d-flex align-items-start">
                                    <img className="icon" src={resolution} alt="" />
                                    <div>
                                        <h2 className="text-blue m-0">{ticketDetails?.ResolutionViolated}</h2>
                                    </div>
                                </div>
                                <p className="mb-2 mt-2">SLA Violated (Resolution)</p>
                            </div>
                            <div className="d-flex justify-content-between">
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className={ticketDetailsPercent?.ResolutionViolatedPercent.class}></i>
                                    <span className="text-xs">{ticketDetailsPercent?.ResolutionViolatedPercent.value} this week</span>
                                </div>
                                <div className="d-flex align-items-end">
                                    <Button icon="pi pi-arrow-right" rounded onClick={() => handleNavigate(0, 'SLA_Violated_Resolution')} />
                                </div>
                            </div>

                        </div>
                    </div> */}
                </Carousel>
            </div>
            {/* <div className="row mb-2">
                <div className="col-md-4 mb-2">
                    <div className="dash-stats-card d-flex justify-content-between alltkt">
                        <div className="d-flex align-items-start">
                            <img className="icon" src={alltkt} alt="" />
                            <div>
                                <h2 className="text-blue m-0">{ticketCards.TotalTickets}</h2>
                                <p className="mb-2">All Ticket</p>
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className="pi pi-arrow-up-right"></i>
                                    <span className="text-xs">+1.01% this week</span>
                                </div>
                            </div>
                        </div>
                        <div className="d-flex align-items-end">
                            <Button icon="pi pi-arrow-right" rounded onClick={handleNavigate} />
                        </div>
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="dash-stats-card d-flex justify-content-between closetkt">
                        <div className="d-flex align-items-start">
                            <img className="icon" src={closetkt} alt="" />
                            <div>
                                <h2 className="text-blue m-0">{ticketCards.ClosedTickets}</h2>
                                <p className="mb-2">Closed Ticket</p>
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className="pi pi-arrow-up-right"></i>
                                    <span className="text-xs">+1.01% this week</span>
                                </div> 
                            </div>
                        </div>
                        <div className="d-flex align-items-end">
                            <Button icon="pi pi-arrow-right" rounded onClick={handleNavigate} />
                        </div>
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="dash-stats-card d-flex justify-content-between opentkt">
                        <div className="d-flex align-items-start">
                            <img className="icon" src={opentkt} alt="" />
                            <div>
                                <h2 className="text-blue m-0">{ticketCards.OpenTickets}</h2>
                                <p className="mb-2">Open Ticket</p>
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className="pi pi-arrow-down-left"></i>
                                    <span className="text-xs">+1.01% this week</span>
                                </div>
                            </div>
                        </div>
                        <div className="d-flex align-items-end">
                            <Button icon="pi pi-arrow-right" rounded onClick={handleNavigate} />
                        </div>
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="dash-stats-card d-flex justify-content-between opentkt">
                        <div className="d-flex align-items-start">
                            <img className="icon" src={opentkt} alt="" />
                            <div>
                                <h2 className="text-blue m-0">{ticketCards.ResponseViolated}</h2>
                                <p className="mb-2">SLA Violated (Response)</p>
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className="pi pi-arrow-down-left"></i>
                                    <span className="text-xs">+1.01% this week</span>
                                </div>
                            </div>
                        </div>
                        <div className="d-flex align-items-end">
                            <Button icon="pi pi-arrow-right" rounded onClick={handleNavigate} />
                        </div>
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="dash-stats-card d-flex justify-content-between opentkt">
                        <div className="d-flex align-items-start">
                            <img className="icon" src={opentkt} alt="" />
                            <div>
                                <h2 className="text-blue m-0">{ticketCards.ResolutionViolated}</h2>
                                <p className="mb-2">SLA Violated (Resolution)</p>
                                <div className="d-flex align-items-center text-xs status-move">
                                    <i className="pi pi-arrow-down-left"></i>
                                    <span className="text-xs">+1.01% this week</span>
                                </div>
                            </div>
                        </div>
                        <div className="d-flex align-items-end">
                            <Button icon="pi pi-arrow-right" rounded onClick={handleNavigate} />
                        </div>
                    </div>
                </div>
            </div> */}

        </>
    );
};

export default TicketCardsComponent;