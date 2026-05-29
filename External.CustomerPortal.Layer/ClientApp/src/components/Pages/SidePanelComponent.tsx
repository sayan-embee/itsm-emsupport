import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Message } from 'primereact/message';
import { Sidebar } from 'primereact/sidebar';
import dayjs from "dayjs";

// Images
import logo from '../../assets/Logo.svg';
import avater from '../../assets/user_blank.svg';
import office from '../../assets/corporate.svg';
import email from '../../assets/email-icon.svg';
import phone from '../../assets/phone.svg';
import { Button } from 'primereact/button';

import { ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import { ConfirmDialog } from 'primereact/confirmdialog';
import { logoutAPI } from '../../apis/APIList';
import SidePanelSkeleton from '../skeletons/SidePanelSkeleton';
import { useQueryClient } from '@tanstack/react-query';
import { Chip } from 'primereact/chip';
import AppVersion from '../common/AppVersion';

// interface SidePanelComponentProps {
//     userName: string;
//     userEmail: string;
//     userMobile: string;
//     officeName: string;
//     sidebarIsVisible: boolean;
//     setSidebarIsVisible: (visible: boolean) => void;
// }

// const SidePanelComponent: React.FC<SidePanelComponentProps> = ({
//     userName,
//     officeName,
//     userEmail,
//     userMobile,
//     sidebarIsVisible,
//     setSidebarIsVisible,
// }) => {
//     return (
//         <Sidebar visible={sidebarIsVisible} position="right" onHide={() => setSidebarIsVisible(false)}>
//             <div className="user-section-small-device d-flex flex-column justify-content-between">
//                 <div>
//                     <div className="avater">
//                         <img src={avater} alt="Name" />
//                     </div>
//                     <h6>{userName}</h6>
//                     <div className="mb-2 d-flex align-items-center">
//                         <img src={office} alt="" />
//                         <p className="text-xs ms-2 mb-0">{officeName}</p>
//                     </div>
//                     <div className="mb-2 d-flex align-items-center">
//                         <img src={email} alt="" />
//                         <p className="text-xs ms-2 mb-0">{userEmail}</p>
//                     </div>
//                     <div className="mb-2 d-flex align-items-center">
//                         <img src={phone} alt="" />
//                         <p className="text-xs ms-2 mb-0">{userMobile}</p>
//                     </div>
//                     <Button className='signout px-4 mt-3' label="Sign out" />
//                 </div>
//                 <div>
//                     <div className="brand">
//                         <img src={logo} alt="Embee" />
//                     </div>
//                     <small className="text-xs">© {dayjs().format('YYYY')} Embee Software Pvt. Ltd.</small>
//                 </div>
//             </div>
//         </Sidebar>
//     );
// };

// export default SidePanelComponent;

interface SidePanelComponentProps {
    customerDetails: any;
    userName: string;
    userEmail: string;
    userMobile: string;
    officeName: string;
    openfrom?: string; // Optional prop to determine if the component is opened from a specific page
}

const SidePanelComponent: React.FC<SidePanelComponentProps> = ({
    customerDetails,
    userName,
    officeName,
    userEmail,
    userMobile,
    openfrom = 'default', // Default value if not provided
}) => {

    const location = useLocation();
    const navigate = useNavigate();
    const signOut = useAuthStore((state) => state.signOut);

    const queryClient = useQueryClient();

    const [showButton, setShowButton] = useState(false);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const [departmentList, setDepartmentList] = useState<any>([]);

    const isSigningOutRef = useRef(false);

    const handleSignOut = async () => {
        if (isSigningOutRef.current) return; // Already in progress

        isSigningOutRef.current = true;
        const isConfirmed = await showConfirmDialog({
            header: 'Confirmation',
            message: 'Are you sure you want to sign out?',
            icon: 'pi pi-exclamation-triangle',
        });

        if (isConfirmed) {
            const response = await logoutAPI();
            console.log("logoutAPI: ", response);
            // if (response?.data?.status == 1) {

            queryClient.removeQueries(['ticketDetailsForCount']); // Used in TicketsCardComponent
            queryClient.removeQueries(['ticketDetailsForPercent']); // Used in TicketsCardComponent
            queryClient.removeQueries(['directlineToken']); // Used in WebChatComponent

            queryClient.clear();

            signOut();
            navigate(ROUTE_PATH.SIGN_IN);
            // }
            // else {

            // }
        }
        else {
            isSigningOutRef.current = false;
            return;
        }
    };

    const toggleLoading = useCallback(async () => {
        await new Promise((resolve) => setTimeout(resolve, 250));
        if (userName && userEmail) {
            setIsLoading(false);
        }
    }, [userName, userEmail]);

    useEffect(() => {
        const timer = setTimeout(() => {
            setShowButton(true);
        }, 1000);

        return () => clearTimeout(timer);
    }, []);

    useEffect(() => {
        toggleLoading();
    }, [toggleLoading]);

    useEffect(() => {
        if (customerDetails && customerDetails?.length > 0) {
            try {
                const departmentList = customerDetails
                    ?.map((item: any) => item?.department_name)
                    .filter(Boolean) // Removes any undefined or null values
                    .sort((a: any, b: any) => a.localeCompare(b));

                console.log('Department List: ', departmentList);
                setDepartmentList(departmentList);
            }
            catch (error) {
                console.error('Error at SidePanelComponent:', error);
            }
        }
    }, [customerDetails]);

    // const handleTicketLog = async () => {
    //         const isConfirmed = await showConfirmDialog({
    //             header: 'Confirmation',
    //             message: 'Do you want to create a new ticket?',
    //             icon: 'pi pi-exclamation-triangle',
    //         });

    //         if (isConfirmed) {
    //             window.open('https://supporthub.embee.co.in/support/tickets/new', '_blank');
    //         }
    // }

    return (
        <>
            {
                isLoading &&
                (
                    <SidePanelSkeleton />
                )
            }

            {
                !isLoading &&
                (
                    <div>
                        {openfrom !== 'webchat' && <Button className=' px-3 mt-1 w-100 mb-3 ' label="Create Ticket" onClick={async () => {
                            if (showButton) {
                                // await handleTicketLog(); // wait for logging to complete
                               window.open('https://supporthub.embee.co.in/support/tickets/new', '_blank');
                            }
                        }}
                            disabled={!showButton} />}

                        {/* <ConfirmDialog /> */}

                        {/* <div className="avater">
                            <img src={avater} alt="Name" />
                        </div> */}

                        <h6>{userName}</h6>
                        {
                            officeName &&
                            (
                                <div className="mb-2 d-flex align-items-center">
                                    <img src={office} alt="" />
                                    <span className="text-xs ms-2 mb-0">{officeName}</span>
                                </div>
                            )
                        }
                        {
                            userEmail &&
                            (
                                <div className="mb-2 d-flex align-items-center">
                                    <img src={email} alt="" />
                                    <span className="text-xs ms-2 mb-0">{userEmail}</span>
                                </div>
                            )
                        }
                        {
                            userMobile &&
                            (
                                <div className="mb-2 d-flex align-items-center">
                                    <img src={phone} alt="" />
                                    <span className="text-xs ms-2 mb-0">{userMobile}</span>
                                </div>
                            )
                        }
                        {
                            departmentList &&
                            departmentList?.length > 0 &&
                            (

                                <ul className='custom-list border-top border-bottom'>
                                    {
                                        departmentList
                                            .map((e: any, index: number) => (
                                                //<Chip key={index} label={e.department_name} />
                                                <li key={`dept-${index}`}>{e}</li>
                                            ))
                                    }
                                </ul>
                            )
                        }
                        <Button className='signout px-3 mt-3' icon="pi pi-sign-out" label="Sign out" onClick={() => handleSignOut()} disabled={!showButton} />
                    </div>
                )
            }

            {/* <div>
                <div className="brand">
                    <img src={logo} alt="Embee" />
                </div>
                <small className="text-xs">© {dayjs().format('YYYY')} Embee Software Pvt. Ltd.</small>
            </div> */}

            <div>
                <div className="brand">
                    <img src={logo} alt="Embee" className="h-6" />
                </div>
                <div className="d-flex align-items-center justify-content-between">
                    <small className="text-xs">
                        © {dayjs().format('YYYY')} Embee Software Pvt. Ltd.
                    </small>
                    <div className='app-version-wrapper'>
                        <AppVersion />
                    </div>
                </div>
            </div>
        </>
    );
};

export default SidePanelComponent;