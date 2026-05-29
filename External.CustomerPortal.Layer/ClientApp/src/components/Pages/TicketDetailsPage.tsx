//#region Imports

import { createRef, useCallback, useEffect, useReducer, useRef, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { InputText } from "primereact/inputtext";
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import OtpInput from "react-otp-input";
import { Toast } from 'primereact/toast';
import dayjs from "dayjs";
import customParseFormat from 'dayjs/plugin/customParseFormat';
import { v4 as uuidv4, validate } from "uuid";
import { ConfirmDialog } from 'primereact/confirmdialog';
import { Calendar } from "primereact/calendar";
import { Sidebar } from 'primereact/sidebar';
import { Dialog } from 'primereact/dialog';
import { Dropdown, DropdownChangeEvent } from 'primereact/dropdown';
import { IconField } from 'primereact/iconfield';
import { InputIcon } from 'primereact/inputicon';
import { DataTable, DataTablePageEvent, DataTableSortEvent, DataTableFilterEvent, DataTableFilterMeta, DataTableFilterMetaData } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Tooltip } from 'primereact/tooltip';
import { Divider } from 'primereact/divider';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { Chips, ChipsChangeEvent } from 'primereact/chips';

import * as _ from "lodash";
import debounce from 'lodash/debounce';

import "../../App.css";
import "../../../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../../../node_modules/primereact/resources/primereact.min.css";
import "../../../node_modules/primeicons/primeicons.css";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.min.js';
import 'jquery/dist/jquery';

// Images import
import logo from '../../assets/Logo.svg';
import ticket from '../../assets/ticket.svg';
import alltkt from '../../assets/alltkt_lg.svg';
import closetkt from '../../assets/closetkt_lg.svg';
import opentkt from '../../assets/opentkt_lg.svg';

import { HOME_ROUTE_PATH, ROUTE_PATH } from '../../router';
import { useAuthStore } from '../../store/authStore';
import { useAlertDialog } from '../../store/AlertDialogProvider';
import { IDropdownOption, IState } from '../../Interfaces';
import { EMAIL_REGEX } from '../../Constants';
import { GetStatusClassStyle, handleAlphanumericKeyPress, handleEmailKeyPress, handleErrorHelper, handleNumericCommaKeyPress, handleNumericKeyPress, MaskEmail, MaskText, UseRegex } from '../../helpers/CommonMethods';
import OtpTimerComponent from '../common/OtpTimerComponent';
import { GetCustomerDetailsAPI, getNewOTPAPI, getTicketDetailsAPI, logoutAPI, verifyOTPAPI, getTicketConversationsAPI } from '../../apis/APIList';
import { CommonMessage, showErrorToast, showInfoToast, showSuccessToast, showWarningToast, toast, toastMessageType } from '../common/ToastComponent';
import LoaderComponent from '../common/LoaderComponent';
import { v4 } from 'uuid';
import OtpSkeleton from '../skeletons/OtpSkeleton';
import { showConfirmDialog } from '../common/ConfirmDialogComponent';
import SidePanelComponent from './SidePanelComponent';
import HeaderComponent from './HeaderComponent';
import TicketCardsComponent from './TicketCardsComponent';
import { useData } from '../../store/DataProvider';
import DataTableSkeleton from '../skeletons/DatatableSkeleton';
import { FilterMatchMode } from 'primereact/api';
import { BlockUI } from 'primereact/blockui';
import UnauthorizedComponent from '../common/UnauthorizedComponent';
import useIsMobile from '../common/useIsMobile';
import { Checkbox } from 'primereact/checkbox';
import { RadioButton } from 'primereact/radiobutton';
import { TabView, TabPanel } from 'primereact/tabview';


//#endregion

//#region Interfaces

interface LazyTableState {
    first: number;
    rows: number;
    page: number;
    sortField?: string | null;
    sortOrder?: number | null;
    filters?: any;
}

enum TextType {
    TicketId = "TicketId"
}

enum DropdownType {
    Status = "Status",
    Department = "Department",
}

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

interface ILoadingState {
    hasReset: boolean;
    pageLoading: boolean;
    buttonLoading: boolean;
    dataTableLoading: boolean;
}

//#endregion

const TicketDetailsPage = () => {
    dayjs.extend(customParseFormat);
    const isMountedRef = useRef(true);
    const toastRef = useRef<Toast>(null);
    const intervalRef = useRef<NodeJS.Timeout | null>(null);
    const { showDialog, hideDialog } = useAlertDialog();

    const { data } = useData();

    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const query_statusId = queryParams.get('sid'); // Status Id
    const query_type = queryParams.get('type'); // Type

    const navigate = useNavigate();

    const { startDateProp, endDateProp, ticketIdProp } = location.state || {};

    const isMobile = useIsMobile();

    //#region State
    const [isJwtValid, setIsJwtValid] = useState(true);
    // const setSignedInVerified = useAuthStore((state) => state.signIn);
    // const setCaptchaVerified = useAuthStore((state) => state.setCaptchaVerified);
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const isCaptchaVerified = useAuthStore((state) => state.isCaptchaVerified);
    const isOtpVerified = useAuthStore((state) => state.isOtpVerified);
    const isSignedIn = useAuthStore((state) => state.isSignedIn);
    const signOut = useAuthStore((state) => state.signOut);

    const [rowClick, setRowClick] = useState<boolean>(false);

    const [filters, setFilters] = useState<DataTableFilterMeta>({
        global: { value: null, matchMode: FilterMatchMode.STARTS_WITH }
    });
    // const [globalFilterValue, setGlobalFilterValue] = useState<string>('');

    const [totalRecords, setTotalRecords] = useState(0);
    const DEFAULT_LAZY_STATE: LazyTableState = {
        first: 0,
        rows: 10,
        page: 1,
        sortField: null,
        sortOrder: null,
        filters: {}
    };
    const [lazyState, setLazyState] = useState<LazyTableState>(DEFAULT_LAZY_STATE);

    const statusOptions: IDropdownOption[] = [
        { code: -1, name: 'All Tickets', info: alltkt },
        { code: 5, name: 'Closed/ Resolved Tickets', info: closetkt },
        { code: 2, name: 'Active Tickets', info: opentkt }
    ];

    const [statusOptionList, setStatusOptionList] = useState<IDropdownOption[] | []>(statusOptions);
    const [selectedStatus, setSelectedStatus] = useState<IDropdownOption | null>(
        (query_statusId && parseInt(query_statusId) > 0)
            ? (statusOptions.filter((e: any) => e.code === parseInt(query_statusId))?.[0])
            : statusOptions[0]
    );

    const [departmentOptionList, setDepartmentOptionList] = useState<IDropdownOption[] | []>([]);
    const [selectedDepartment, setSelectedDepartment] = useState<IDropdownOption[]>([]);

    const [ticketDumpList, setTicketDumpList] = useState<any[]>([]);
    const [ticketList, setTicketList] = useState<any[]>([]);
    const [selectedTicketList, setSelectedTicketList] = useState<any[]>([]);

    const [email, setEmail] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: false,
        ErrorMessage: ''
    });

    const [startDate, setStartDate] = useState<IState>({
        Value: startDateProp ? startDateProp : dayjs().startOf('month').toDate(),
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [endDate, setEndDate] = useState<IState>({
        Value: endDateProp ? endDateProp : dayjs().toDate(),
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [ticketId, setTicketId] = useState<IState>({
        Value: ticketIdProp ? [ticketIdProp] : [],
        IsRequired: false,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        hasReset: false,
        pageLoading: true,
        buttonLoading: false,
        dataTableLoading: true
    });

    const DataTableSkeletonOptions = {
        columns: ['Ticket ID', 'Status', 'Created Date', 'Subject', 'Company'],
        rows: 0
    }
    const [visible2, setVisible2] = useState<boolean>(false);

    const [hasFiltered, setHasFiltered] = useState(false);

    const [showExportDialog, setShowExportDialog] = useState(false);
    const [selectedExportFields, setSelectedExportFields] = useState<string[]>([]);
    const allExportFields = [
        'Ticket Id',
        'Status',
        'Created Time',
        'Type',
        'Category',
        'Subject',
        'Priority',
        'Sub-Category',
        'Requester Name',
        'Requester Email',
        'Company',
        'Closed Time',
        'First Response Status',
        'Resolution Status',
        'Resolution Remarks',
        'First Response Time (in Hrs)',
        'Last Updated Time'
    ];

    //#endregion
    const [selectedTicket, setSelectedTicket] = useState<any | null>(null);
    const [ticketDialogVisible, setTicketDialogVisible] = useState<boolean>(false);
    const [conversations, setConversations] = useState<any[]>([]);
    const [loadingConversations, setLoadingConversations] = useState(false);

    const handleTicketClick = async (ticket: any) => {
        console.log('Selected Ticket: ', ticket);

        setSelectedTicket({
            ticketId: ticket.ticketId,
            createdDate: ticket.createdDate || '-',
            subject: ticket.subject || '-',
            departmentName: ticket.departmentName || '-',
            type: ticket.type || '-',
            category: ticket.category || '-',
            subCategory: ticket.subCategory || '-',
            priority: ticket.priority || '-',
            requesterName: ticket.requesterName || '-',
            requesterEmail: ticket.requesterEmail || '-',
            requesterMobile: ticket.requesterMobile || '-',
            resolutionStatus: ticket.resolutionStatus || '-',
            firstResponseTimeInSecs: formatTimeFromSeconds(ticket.firstResponseTimeInSecs),
            resolutionTimeInSecs: formatTimeFromSeconds(ticket.resolutionTimeInSecs),
            onRoasterEngineer: ticket.rosterEngineer || '-',
            StatusName: ticket.status?.trim() || '-',
            resolutionRemarks: ticket.resolutionRemarks || '-',
            tenant: ticket.tenant || '-',
            responseStatus: ticket.responseStatus || '-',
            firstResponseTime: ticket.firstResponseTime || '-',
            resolutionTime: ticket.resolutionTime || '-'
        });

        setTicketDialogVisible(true);
        try {
            setLoadingConversations(true);
            const response = await getTicketConversationsAPI(ticket.ticketId);
            if (response?.data?.conversations) {
                setConversations(response.data.conversations);
            } else {
                setConversations([]);
            }
        } catch (err) {
            console.error("Failed to fetch conversations", err);
            setConversations([]);
        } finally {
            setLoadingConversations(false);
        }
    };
    const formatTimeFromSeconds = (seconds: number | null | undefined): string => {
        if (seconds === null || seconds === undefined || isNaN(seconds)) return '-';
        const hrs = Math.floor(seconds / 3600);
        const mins = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;
        return `${hrs.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    };


    //#region API

    const getTicketDetails = async (isExcelDownload: boolean = false): Promise<void> => {
        try {
            if (!data || data.length === 0) {
                setComponentLoading({ ...componentLoading, pageLoading: false, buttonLoading: false, dataTableLoading: false });
                return;
            }

            // const dept_Id_List = data.map((e: any) => e.department_id).join(',');
            const dept_Id_List = selectedDepartment && selectedDepartment?.length > 0
                ? selectedDepartment.map((e) => e.code).join(',')
                : null;

            const selectedStatusId = selectedStatus?.code > 0 ? selectedStatus?.code : null;

            const ticket_Id_List = ticketId?.Value?.length > 0
                ? ticketId?.Value.map((e: any) => e.trim()).join(',')
                : null;

            let startDate_Value = null;
            let endDate_Value = null;

            if (ticket_Id_List) {
                startDate_Value = null;
                endDate_Value = null;
            }
            else {
                startDate_Value = startDate.Value ? dayjs(startDate.Value).format("DD-MM-YYYY") : null;
                endDate_Value = endDate.Value ? dayjs(endDate.Value).format("DD-MM-YYYY") : null;
            }

            if (isExcelDownload) {
                setComponentLoading({ ...componentLoading, buttonLoading: true });

                const jsonBody = {
                    "pageNumber": 0,
                    "pageSize": totalRecords,
                    "transactionType": 'List',
                    // "departmentId_List": '27000586401,27001473235', // For Testing
                    "departmentId_List": dept_Id_List,
                    "ticketId_List": ticket_Id_List,
                    "fromDate": startDate_Value,
                    "toDate": endDate_Value,
                    "statusId": selectedStatusId
                }

                console.log('getTicketDetails-JSON: ', jsonBody);
                const response = await getTicketDetailsAPI(jsonBody);
                console.log("getTicketDetailsAPI-List: ", response);

                if (!response || response.status >= 400 || !response.data) {
                    throw new Error(response?.data?.message ?? 'Error fetching ticket details.');
                }

                const { ticketList, totalCount } = response.data;

                let filteredList = ticketList;

                if (query_type) {
                    if (query_type === 'SLA_Violated_Response') {
                        filteredList = ticketList.filter((e: any) => e.ResponseStatus === 'SLA Violated');
                    }
                    else if (query_type === 'SLA_Violated_Resolution') {
                        filteredList = ticketList.filter((e: any) => e.ResolutionStatus === 'SLA Violated');
                    }
                }

                if (filteredList && filteredList?.length > 0) {
                    const dataList = filteredList.map((e: any) => {

                        let createdDate = e.created_at_display;
                        if (createdDate) {
                            const date = dayjs(createdDate, "DD-MM-YYYY HH:mm:ss");
                            createdDate = date.format("DD/MM/YYYY HH:mm:ss");
                        }

                        let closedDate = e.closed_at_display;
                        if (closedDate) {
                            const date = dayjs(closedDate, "DD-MM-YYYY HH:mm:ss");
                            closedDate = date.format("DD/MM/YYYY HH:mm:ss");
                        }

                        let first_response_time_in_HR = e.first_resp_time_in_secs;

                        if (typeof first_response_time_in_HR === "number" && first_response_time_in_HR >= 0) {
                            const hours = Math.floor(first_response_time_in_HR / 3600);
                            const minutes = Math.floor((first_response_time_in_HR % 3600) / 60);
                            const seconds = first_response_time_in_HR % 60;

                            first_response_time_in_HR = `${hours.toString().padStart(2, "0")}:${minutes.toString().padStart(2, "0")}:${seconds.toString().padStart(2, "0")}`;
                        }

                        let lastUpdateTime = e.status_updated_at_display;
                        if (lastUpdateTime) {
                            const date = dayjs(lastUpdateTime, "DD-MM-YYYY HH:mm:ss");
                            lastUpdateTime = date.format("DD/MM/YYYY HH:mm:ss");
                        }

                        return {
                            'Ticket Id': e.id,
                            'Status': e.StatusName ?? '',
                            'Created Time': createdDate,
                            'Type': e.type ?? '',
                            'Category': e.category ?? '',
                            'Subject': e.subject ?? '',
                            'Priority': e.priorityname ?? '',
                            'Sub-Category': e.sub_category ?? '',
                            'Requester Name': e.RequesterName ?? '',
                            'Requester Email': e.RequesterEmail ?? '',
                            'Company': e.name,
                            'Closed Time': closedDate,
                            'First Response Status': e.ResponseStatus ?? '',
                            'Resolution Status': e.ResolutionStatus ?? '',
                            'Resolution Remarks': e.resolution_remarks ?? '',
                            'First Response Time (in Hrs)': first_response_time_in_HR,
                            'Last Updated Time': lastUpdateTime
                        }
                    });

                    await exportExcel(dataList);
                }
                else {
                    showInfoToast(toastRef, toastMessageType.Info, 'No data available.');
                }
            }
            else {
                setComponentLoading({ ...componentLoading, pageLoading: false, dataTableLoading: true });

                const jsonBody = {
                    "pageNumber": lazyState.page,
                    "pageSize": lazyState.rows,
                    "transactionType": 'List',
                    // "departmentId_List": '27000586401,27001473235', // For Testing
                    "departmentId_List": dept_Id_List,
                    "ticketId_List": ticket_Id_List,
                    "fromDate": startDate_Value,
                    // "fromDate": '01-01-2024', // For Testing
                    "toDate": endDate_Value,
                    "statusId": selectedStatusId
                }

                console.log('getTicketDetails-JSON: ', jsonBody);
                const response = await getTicketDetailsAPI(jsonBody);
                console.log("getTicketDetailsAPI-List: ", response);

                if (!response || response.status >= 400 || !response.data) {
                    throw new Error(response?.data?.message ?? 'Error fetching ticket details.');
                }

                const { ticketList, totalCount } = response.data;

                let filteredList = ticketList;

                if (query_type) {
                    if (query_type === 'SLA_Violated_Response') {
                        filteredList = ticketList.filter((e: any) => e.ResponseStatus === 'SLA Violated');
                    }
                    else if (query_type === 'SLA_Violated_Resolution') {
                        filteredList = ticketList.filter((e: any) => e.ResolutionStatus === 'SLA Violated');
                    }
                }

                const dataList = filteredList.map((e: any) => {
                    let createdDate = e.created_at_display;
                    if (createdDate) {
                        const date = dayjs(createdDate, "DD-MM-YYYY HH:mm:ss");
                        createdDate = date.format("DD/MM/YYYY HH:mm:ss");
                    }

                    const statusClass = GetStatusClassStyle(e.status ?? 0);

                    return {
                        ticketId: e.id,
                        ticketId_Formatted: (<span className="tktid">{e.id}</span>),
                        departmentId: e.department_id,
                        departmentName: e.name,
                        createdDate: createdDate,
                        subject: e.subject ?? '-',
                        statusId: e.status,
                        status: e.StatusName ?? '-',
                        type: e.type ?? '-',
                        status_Formatted: (<span className={`status ${statusClass} m-0`}>{e.StatusName}</span>),
                        priority: e.priorityname ?? '-',
                        category: e.category ?? '-',
                        subCategory: e.sub_category ?? '-',
                        requesterName: e.RequesterName ?? '-',
                        requesterEmail: e.RequesterEmail ?? '-',
                        requesterMobile: e.RequesterMobile ?? '-',
                        tenant: e.tenant ?? '-',
                        resolutionRemarks: e.resolution_remarks ?? '-',
                        rosterEngineer: e.on_roaster_engineer ?? '-',
                        responseStatus: e.ResponseStatus ?? '-',
                        resolutionStatus: e.ResolutionStatus ?? '-',
                        firstResponseTimeInSecs: e.first_resp_time_in_secs ?? 0,
                        resolutionTimeInSecs: e.resolution_time_in_secs ?? 0,
                        firstResponseTime: e.first_responded_at_display ?? '-',
                        resolutionTime: e.resolved_at_display ?? '-',


                    }
                });

                setTicketList(dataList);
                setTicketDumpList(dataList);
                setTotalRecords(totalCount);
            }
        }
        catch (error: any) {
            const { statusCode, errorMessage } = handleErrorHelper("getTicketDetails", error);
            if (statusCode === 401 || statusCode === 403) {
                setIsJwtValid(false);
                // navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
            }
            else {
                showErrorToast(toastRef, toastMessageType.Error, errorMessage);
            }
        }
        finally {
            setComponentLoading({ ...componentLoading, hasReset: false, pageLoading: false, buttonLoading: false, dataTableLoading: false });
        }
    };

    //#endregion


    //#region Helper Methods

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true) => {
        showDialog(header, content, isClosable);
    };

    const handleSignOut = useCallback(async () => {
        await logoutAPI();
        signOut();
        navigate(ROUTE_PATH.SIGN_IN);
    }, [signOut, navigate]);

    const handleNavigate = async () => {
        navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.DASHBOARD);
    };

    const handleFilter = async () => {
        setVisible2(false);

        const isValid = await validateFields('Filter');
        if (!isValid) {
            return;
        }
        else {
            setLazyState((prevState) => ({
                ...prevState,
                first: 0,
                page: 1
            }));
            setHasFiltered(true);
        }
    };

    // const handleOnChangeFilter = (paramItem: IDropdownOption) => {

    //     if (paramItem && ticketDumpList && ticketDumpList.length > 0) {
    //         if (paramItem.code === -1 || ticketDumpList.length >= 1000) {
    //             getTicketDetails(paramItem);
    //         }
    //         else {
    //             setComponentLoading({ ...componentLoading, pageLoading: false, dataTableLoading: true });

    //             const filteredList = ticketDumpList.filter((e: any) => e.statusId === paramItem.code);
    //             console.log('filteredList: ', filteredList);
    //             setTicketList(filteredList);

    //             setComponentLoading({ ...componentLoading, pageLoading: false, dataTableLoading: false });
    //         }
    //     }

    // };

    const getDepartmentDetails = () => {
        if (!data || (data && data?.length === 0)) {
            return;
        }

        // Create a new Map to store unique department entries
        const departmentMap = new Map<number, { department_id: number, department_name: string }>();

        // Iterate through the data array
        data.forEach((e: any) => {
            // Step 3: Add to Map using department_id as the key to ensure uniqueness
            departmentMap.set(e.department_id, {
                department_id: e.department_id,
                department_name: e.department_name
            });
        });

        // Convert the Map values to an array
        const uniqueDepartments = Array.from(departmentMap.values());
        console.log('uniqueDepartments: ', uniqueDepartments);

        let dataList = uniqueDepartments.map((e: any) => {
            return { code: e.department_id, name: e.department_name, info: '' }
        });

        // if (dataList?.length > 1) {
        //     dataList.unshift({
        //         code: -1,
        //         name: 'All Companies',
        //         info: ''
        //     });
        // }

        setSelectedDepartment(dataList);
        setDepartmentOptionList(dataList);
    }

    const saveAsExcelFile = async (buffer: any, fileName: string) => {
        import('file-saver').then((FileSaver) => {
            let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
            let EXCEL_EXTENSION = '.xlsx';
            const data: Blob = new Blob([buffer], {
                type: EXCEL_TYPE
            });
            FileSaver.default.saveAs(data, fileName + '_Export_' + dayjs().format('DDMMYYYY_HHmmss') + EXCEL_EXTENSION);
        });
    };

    const exportExcel = async (dataList: any[]) => {
        import('xlsx').then(async (xlsx) => {
            const worksheet = xlsx.utils.json_to_sheet(dataList);
            const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
            const excelBuffer = xlsx.write(workbook, {
                bookType: 'xlsx',
                type: 'array'
            });
            await saveAsExcelFile(excelBuffer, 'Tickets');
            setComponentLoading({ ...componentLoading, buttonLoading: false });
            showSuccessToast(toastRef, 'Success', 'Tickets exported successfully.');
        });
    };

    //#endregion


    //#region OnChange Methods

    const handleDropdownChange = (e: any, type: DropdownType) => {
        console.log("handleDropdownChange: ", e);
        setHasFiltered(true);

        switch (type) {
            case DropdownType.Status:
                {
                    setSelectedStatus(e);
                    setLazyState((prevState) => ({
                        ...prevState,
                        first: 0,
                        page: 1
                    }));
                    break;
                }

            default:
                break;
        }
    };

    const handleMultiSelectChange = (e: any, type: DropdownType) => {
        console.log("handleMultiSelectChange: ", e);
        setHasFiltered(true);

        switch (type) {
            case DropdownType.Department:
                {
                    setSelectedDepartment(e);
                    break;
                }

            default:
                break;
        }
    }

    const handleDateChange = (e: any, type: DateType) => {
        console.log("handleDateChange: ", e);
        setHasFiltered(true);

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

    const handleTextChange = (e: any, type: TextType) => {
        console.log("handleTextChange: ", e);
        setHasFiltered(true);

        switch (type) {
            case TextType.TicketId:
                {
                    setTicketId({ ...ticketId, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    const handleChipChange = (e: any, type: TextType) => {
        console.log("handleChipChange: ", e);
        setHasFiltered(true);

        switch (type) {
            case TextType.TicketId:
                {
                    setTicketId({ ...ticketId, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    const handlePaste = (event: React.ClipboardEvent<HTMLDivElement>) => {
        event.preventDefault();  // Prevent the default paste behavior

        // Get the pasted content
        const pastedText = event.clipboardData.getData('text');

        // Remove all spaces from the pasted text
        const textWithoutSpaces = pastedText.replace(/\s+/g, '');

        // Insert the cleaned text into the Chips component's internal div
        const currentValue = ticketId.Value || '';  // Get current value from the Chips component
        handleChipChange(currentValue + textWithoutSpaces, TextType.TicketId);  // Update the value with cleaned text
    };

    const handleReset = () => {
        // setGlobalFilterValue('');
        // setGlobalFilterChange('');

        setStartDate({ ...startDate, Value: dayjs().startOf('month').toDate(), IsValid: true, ErrorMessage: '' });
        setEndDate({ ...endDate, Value: dayjs().toDate(), IsValid: true, ErrorMessage: '' });

        setSelectedStatus(statusOptions[0]);
        setSelectedDepartment(departmentOptionList);

        setTicketId({ ...ticketId, Value: [], IsValid: true, ErrorMessage: '' });


        const params = new URLSearchParams(location.search);
        let paramsDeleted = false;

        if (params.has("type")) {
            params.delete("type");
            paramsDeleted = true;
        }
        if (params.has("sid")) {
            params.delete("sid");
            paramsDeleted = true;
        }

        if (paramsDeleted) {
            navigate({ search: params.toString() }, { replace: true });
        }

        setComponentLoading({ ...componentLoading, hasReset: true });

        setHasFiltered(false);

        setLazyState((prevState) => ({
            ...prevState,
            first: 0,
            page: 1
        }));
    };

    // const handleOnSubmit = async () => {
    //     const isValid = await validateFields('VerifyOTP');
    //     if (!isValid) {
    //         return;
    //     }

    // };

    //#endregion


    //#region Validation Methods

    const validateFields = async (type: 'Filter') => {
        let result = true;
        switch (type) {
            case 'Filter': {

                if (!startDate.Value) {
                    setStartDate({ ...startDate, IsValid: false, ErrorMessage: 'Start date is required' });
                    showWarningToast(toastRef, toastMessageType.Warn, 'Start date is required');
                    result = false;
                } else {
                    setStartDate({ ...startDate, IsValid: true, ErrorMessage: '' });
                }

                if (!endDate.Value) {
                    setEndDate({ ...endDate, IsValid: false, ErrorMessage: 'End date is required' });
                    showWarningToast(toastRef, toastMessageType.Warn, 'End date is required');
                    result = false;
                } else {
                    setEndDate({ ...endDate, IsValid: true, ErrorMessage: '' });
                }

                break;
            }

            default: break;
        }

        return result;
    };

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
        if (hasHydrated && (!data)) {
            navigate(ROUTE_PATH.HOME);
        }
    }, [hasHydrated, data, navigate]);


    useEffect(() => {
        isMountedRef.current = true;
        if (isMountedRef.current) {
            console.log('ticket-customer-data: ', data);
            getDepartmentDetails();
        }
    }, []);


    useEffect(() => {
        if (departmentOptionList?.length === 0) {
            return;
        }
        getTicketDetails();
    }, [lazyState, departmentOptionList]);


    // useEffect(() => {
    //     if (componentLoading.hasReset) {
    //         getTicketDetails();
    //     }
    // }, [componentLoading.hasReset]);

    // useEffect(() => {
    //     if (!isJwtValid) {
    //         navigate(ROUTE_PATH.HOME + HOME_ROUTE_PATH.SESSION_EXPIRED);
    //     }
    // }, [isJwtValid]);

    //#endregion


    //#region Render Helpers

    // const setGlobalFilterChange = useCallback(
    //     debounce((value: string) => {
    //         setComponentLoading(prevState => ({ ...prevState, dataTableLoading: true }));

    //         let _filters = { ...filters };
    //         (_filters['global'] as DataTableFilterMetaData).value = value;

    //         setFilters(_filters);

    //         setComponentLoading(prevState => ({ ...prevState, dataTableLoading: false }));
    //     }, 300), // Reduced debounce time for smoother experience
    //     [filters]
    // );

    // const onGlobalFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    //     const value = e.target.value;
    //     setGlobalFilterValue(value);

    //     // Trigger debounced filter change
    //     setGlobalFilterChange(value);
    // };

    const truncatedRow = (rowData: any, type: 'Subject') => {
        if (type === 'Subject') {
            const subject = rowData.subject;
            if (subject.length > 100) {
                const truncatedSubject = subject.length > 100 ? subject.substring(0, 50) + '...' : subject;
                const tooltipId = `subject-tooltip-${rowData.ticketId}`;
                return (
                    <>
                        <span id={tooltipId}>{truncatedSubject}</span>
                        <Tooltip target={`#${tooltipId}`} content={subject} position="top" style={{ maxWidth: '25rem' }} />
                    </>
                );
            }
            else {
                return subject;
            }
        }
    };

    const renderTooltipInRow = (rowData: any, type: 'requester') => {
        if (type === 'requester') {
            const requesterEmail = rowData.requesterEmail;
            const tooltipId = `req-email-tooltip-${rowData.ticketId}`;
            return (
                <>
                    <span id={tooltipId}>{rowData.requesterName}</span>
                    <Tooltip target={`#${tooltipId}`} content={requesterEmail} position="left" style={{ maxWidth: '25rem' }} />
                </>
            );
        }
    };

    const statusDropdownValueTemplate = (e: IDropdownOption | null, props: any) => {
        if (e) {
            return (
                <div className="flex align-items-center">
                    <img src={e.info} alt='' style={{ width: '18px', marginRight: '8px' }} />
                    <span>{e.name}</span>
                </div>
            );
        }
        return <span>{props.placeholder}</span>;
    };

    const statusDropdownItemTemplate = (e: IDropdownOption) => {
        return (
            <div className="flex align-items-center">
                <img src={e.info} alt='' style={{ width: '18px', marginRight: '8px' }} />
                <span>{e.name}</span>
            </div>
        );
    };

    const onPage = (event: DataTablePageEvent) => {
        console.log('onPage: ', event);
        const currentPage = event.page !== undefined
            ? event.page + 1
            : Math.floor(event.first / event.rows) + 1;

        setLazyState({
            ...lazyState,
            first: event.first,
            rows: event.rows,
            page: currentPage
        });
    };

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

    const handleExportWithSelection = async () => {

        const jsonBody = {
            "pageNumber": 0,
            "pageSize": totalRecords,
            "transactionType": 'List',
            "departmentId_List": selectedDepartment?.map((e) => e.code).join(',') || null,
            "ticketId_List": ticketId?.Value?.length > 0 ? ticketId?.Value.map((e: any) => e.trim()).join(',') : null,
            "fromDate": startDate.Value ? dayjs(startDate.Value).format("DD-MM-YYYY") : null,
            "toDate": endDate.Value ? dayjs(endDate.Value).format("DD-MM-YYYY") : null,
            "statusId": selectedStatus?.code > 0 ? selectedStatus?.code : null
        };

        const response = await getTicketDetailsAPI(jsonBody);
        const { ticketList } = response.data;

        if (ticketList && ticketList.length > 0) {
            setComponentLoading({ ...componentLoading, buttonLoading: true });
            const fullData = ticketList.map((e: any) => ({
                'Ticket Id': e.id,
                'Status': e.StatusName ?? '',
                'Created Time': e.created_at_display,
                'Type': e.type ?? '',
                'Category': e.category ?? '',
                'Subject': e.subject ?? '',
                'Priority': e.priorityname ?? '',
                'Sub-Category': e.sub_category ?? '',
                'Requester Name': e.RequesterName ?? '',
                'Requester Email': e.RequesterEmail ?? '',
                'Company': e.name,
                'Closed Time': e.closed_at_display,
                'First Response Status': e.ResponseStatus ?? '',
                'Resolution Status': e.ResolutionStatus ?? '',
                'Resolution Remarks': e.resolution_remarks ?? '',
                'First Response Time (in Hrs)': e.first_resp_time_in_secs,
                'Last Updated Time': e.status_updated_at_display
            }));

            const filteredData = fullData.map((row: any) => {
                const newRow: any = {};
                selectedExportFields.forEach(field => {
                    newRow[field] = row[field];
                });
                return newRow;
            });

            await exportExcel(filteredData);
        } else {
            //setComponentLoading({ ...componentLoading, buttonLoading: false });
            showInfoToast(toastRef, toastMessageType.Info, 'No data available.');
        }
    };

   
  const extractSenderFromBody = (html: string | null): string | null => {
  if (!html) return null;

  const normalizedHtml = html.replace(/\u00A0/g, ' ');

  if (typeof document !== 'undefined') {
    try {
      const container = document.createElement('div');
      container.innerHTML = normalizedHtml;
      const possibleLabels = container.querySelectorAll('strong, b, span, div, p, td');

      for (let i = 0; i < possibleLabels.length; i++) {
        const el = possibleLabels[i] as HTMLElement;
        const txt = (el.textContent || '').trim().replace(/\s+/g, ' ');

        // Detect proper "From:" label (not inside a word)
        if (/^From:?$/i.test(txt) || /\bFrom:?\b/i.test(txt)) {
          let next = el.nextSibling;
          while (next) {
            if (next.nodeType === Node.TEXT_NODE) {
              const t = (next.textContent || '').trim();
              if (t) return t.replace(/\s*(Sent:.*)$/i, '').trim();
            }
            if (next.nodeType === Node.ELEMENT_NODE) {
              const t = (next as Element).textContent || '';
              const tTrim = t.trim();
              if (tTrim) return tTrim.replace(/\s*(Sent:.*)$/i, '').trim();
            }
            next = next.nextSibling;
          }
        }

        const inlineMatch = (el.innerText || '').match(
          /(?:^|\s)From[:\s]*([^\n\r]+?)(?:Sent[:\s]|To[:\s]|Subject[:\s]|$)/i
        );
        if (inlineMatch && inlineMatch[1]) {
          return inlineMatch[1].trim();
        }
      }

      // Fallback: search whole text content but require "From:" (not inside another word)
      const allText = container.textContent || '';
      const match = allText.match(/\bFrom[:\s]+([\s\S]*?)(?:Sent[:\s]|To[:\s]|Subject[:\s]|$)/i);
      if (match && match[1]) {
        return match[1].trim();
      }
    } catch (e) {
      // ignore and fallback to regex on raw HTML
    }
  }

  // Final fallback: only match if there's an actual "From:" token
  const stripped = normalizedHtml.replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').trim();
  if (!/\bFrom[:\s]/i.test(stripped)) return null; // ✅ ensure "From" exists as standalone word

  const m = stripped.match(/\bFrom[:\s]+([\s\S]*?)(?:Sent[:\s]|To[:\s]|Subject[:\s]|$)/i);
  return m && m[1] ? m[1].trim() : null;
};


    return (
        <div className="container-fluid px-md-5 px-2 mb-2">

            {componentLoading.buttonLoading && <LoaderComponent />}

            <BlockUI className="transparent-blockui" blocked={componentLoading.dataTableLoading || componentLoading.buttonLoading}>
                <div className="list-page-header mb-3 d-none d-md-flex align-items-center justify-content-between">

                    <Toast ref={toastRef} />

                    <div className="d-flex align-items-center">
                        <Button className="back-btn" icon="pi pi-arrow-left" rounded aria-label="Back" onClick={handleNavigate} />
                        {
                            departmentOptionList.length > 1 &&
                            (
                                <MultiSelect
                                    value={selectedDepartment}
                                    onChange={(e: MultiSelectChangeEvent) => handleMultiSelectChange(e.value, DropdownType.Department)}
                                    options={departmentOptionList}
                                    optionLabel="name"
                                    filter
                                    filterBy="name"
                                    placeholder="Select company"
                                    maxSelectedLabels={1}
                                />
                            )
                        }
                        <Dropdown
                            className='me-2'
                            value={selectedStatus}
                            onChange={(e: DropdownChangeEvent) => handleDropdownChange(e.value, DropdownType.Status)}
                            options={statusOptionList}
                            optionLabel="name"
                            placeholder="Select status"
                            valueTemplate={statusDropdownValueTemplate}
                            itemTemplate={statusDropdownItemTemplate}
                        />
                        <div className="report-form">
                            {
                                ticketId.Value?.length === 0 &&
                                (
                                    <>
                                        <Calendar className="me-1" inputId="startDate" showIcon placeholder={`From date *`}
                                            // minDate={FromMinDateTime}
                                            // maxDate={selectedToDate ?? null}
                                            value={startDate.Value}
                                            dateFormat="dd/mm/yy"
                                            onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                            invalid={startDate.IsValid}
                                        />
                                        {/* {
                                            !startDate.IsValid && startDate.ErrorMessage &&
                                            <small className="text-danger">{startDate.ErrorMessage}</small>
                                        } */}
                                        <Calendar className="me-2" inputId="endDate" showIcon placeholder="To date *"
                                            minDate={startDate.Value ?? null}
                                            // maxDate={selectedToDate ?? null}
                                            value={endDate.Value}
                                            dateFormat="dd/mm/yy"
                                            onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                                            invalid={endDate.IsValid}
                                        />
                                        {/* {
                                            !endDate.IsValid && endDate.ErrorMessage &&
                                            <small className="text-danger">{endDate.ErrorMessage}</small>
                                        } */}
                                    </>
                                )
                            }

                            <Divider layout="vertical" align="center">
                                {
                                    ticketId.Value?.length === 0 &&
                                    (
                                        <span className="p-tag" style={{ fontSize: '0.5rem' }}>OR</span>
                                    )
                                }
                            </Divider>

                            <IconField className='me-1' iconPosition="left">
                                <InputIcon className="pi pi-search"></InputIcon>
                                {/* <InputText placeholder="Enter ticket ID"
                                value={ticketId.Value}
                                onChange={(e) => handleTextChange(e, TextType.TicketId)}
                                onKeyDown={handleNumericKeyPress}
                                maxLength={8}
                                minLength={6}
                            /> */}
                                <Chips
                                    value={ticketId.Value}
                                    placeholder='Ticket ID e.g. 6001, 6002'
                                    onChange={(e: ChipsChangeEvent) => handleChipChange(e.value, TextType.TicketId)}
                                    separator=','
                                    onKeyDown={handleNumericCommaKeyPress}
                                    max={3}
                                    tooltip='Use comma to separate multiple ticket IDs'
                                    tooltipOptions={{ position: 'bottom' }}
                                    addOnBlur
                                    allowDuplicate={false}
                                    onPaste={handlePaste}
                                />
                            </IconField>


                        </div>

                    </div>

                    <div className="d-flex align-items-center">
                        <Button
                            icon={'pi pi-filter'}
                            className={`${hasFiltered ? 'primary' : ''} me-2`}
                            severity={!hasFiltered ? 'secondary' : 'contrast'}
                            tooltip={hasFiltered ? 'Apply Filter' : 'Filter'}
                            tooltipOptions={{ position: 'bottom' }}
                            loading={componentLoading.dataTableLoading}
                            onClick={handleFilter}
                        />

                        <Button className='secondary me-2'
                            icon="pi pi-undo"
                            severity="secondary"
                            tooltip='Reset' tooltipOptions={{ position: 'bottom' }}
                            onClick={handleReset}
                        />
                        {/* <Button className='primary me-3'
                            icon="pi pi-file-excel"
                            loading={componentLoading.buttonLoading}
                            tooltip='Export' tooltipOptions={{ position: 'bottom' }}
                            onClick={() => getTicketDetails(true)}
                        /> */}

                        <Button
                            className='primary me-3'
                            icon="pi pi-download"
                            loading={componentLoading.buttonLoading}
                            tooltip='Export Report'
                            tooltipOptions={{ position: 'bottom' }}
                            onClick={() => {
                                setSelectedExportFields(allExportFields);
                                setShowExportDialog(true);
                            }}
                        />

                    </div>

                </div>
                <div className="list-page-header mb-3 d-flex d-md-none align-items-center justify-content-between">
                    <Button className="back-btn" icon="pi pi-arrow-left" rounded aria-label="Back" onClick={handleNavigate} />
                    <Button icon="pi pi-filter" className='primary' onClick={() => setVisible2(true)} />
                </div>

                <Dialog className="filter-modal" header="Filter" visible={visible2} style={{ width: '95vw' }} onHide={() => setVisible2(false)}>
                    <Toast ref={toastRef} />
                    <div className="list-page-header p-0 mb-3">
                        <div className='pb-3'>
                            {
                                departmentOptionList.length > 1 &&
                                (
                                    <MultiSelect
                                        value={selectedDepartment}
                                        onChange={(e: MultiSelectChangeEvent) => handleMultiSelectChange(e.value, DropdownType.Department)}
                                        options={departmentOptionList}
                                        optionLabel="name"
                                        filter
                                        filterBy="name"
                                        placeholder="Select company"
                                        maxSelectedLabels={1}
                                    />
                                )
                            }
                            <Dropdown
                                className='me-2'
                                value={selectedStatus}
                                onChange={(e: DropdownChangeEvent) => handleDropdownChange(e.value, DropdownType.Status)}
                                options={statusOptionList}
                                optionLabel="name"
                                placeholder="Select status"
                                valueTemplate={statusDropdownValueTemplate}
                                itemTemplate={statusDropdownItemTemplate}
                            />
                            <div className="report-form pt-3">
                                {
                                    ticketId.Value?.length === 0 &&
                                    (
                                        <>
                                            <Calendar className="me-1" inputId="startDate" showIcon placeholder={`From date *`}
                                                // minDate={FromMinDateTime}
                                                // maxDate={selectedToDate ?? null}
                                                value={startDate.Value}
                                                dateFormat="dd/mm/yy"
                                                onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                                invalid={startDate.IsValid}
                                            />
                                            <Calendar inputId="endDate" showIcon placeholder="To date *"
                                                minDate={startDate.Value ?? null}
                                                // maxDate={selectedToDate ?? null}
                                                value={endDate.Value}
                                                dateFormat="dd/mm/yy"
                                                onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                                                invalid={endDate.IsValid}
                                            />
                                        </>
                                    )
                                }
                            </div>

                            <Divider align="center">
                                {
                                    ticketId.Value?.length === 0 &&
                                    (
                                        <span className="p-tag" style={{ fontSize: '0.5rem' }}>OR</span>
                                    )
                                }
                            </Divider>

                            <IconField className='me-1' iconPosition="left">
                                <InputIcon className="pi pi-search"></InputIcon>
                                {/* <InputText placeholder="Enter ticket ID"
                                    value={ticketId.Value}
                                    onChange={(e) => handleTextChange(e, TextType.TicketId)}
                                    onKeyDown={handleNumericKeyPress}
                                    maxLength={8}
                                    minLength={6}
                                /> */}
                                <Chips
                                    value={ticketId.Value}
                                    placeholder='Ticket ID e.g. 6001, 6002'
                                    onChange={(e: ChipsChangeEvent) => handleChipChange(e.value, TextType.TicketId)}
                                    separator=','
                                    onKeyDown={handleNumericCommaKeyPress}
                                    max={3}
                                    tooltip='Use comma to separate multiple ticket IDs'
                                    tooltipOptions={{ position: 'bottom' }}
                                    addOnBlur
                                    allowDuplicate={false}
                                    onPaste={handlePaste}
                                />
                            </IconField>


                        </div>



                        <div className="d-flex align-items-center justify-content-end">
                            <Button
                                icon={'pi pi-filter'}
                                className={`${hasFiltered ? 'primary' : ''} me-2`}
                                severity={!hasFiltered ? 'secondary' : 'contrast'}
                                tooltip={hasFiltered ? 'Apply Filter' : 'Filter'}
                                tooltipOptions={{ position: 'bottom' }}
                                loading={componentLoading.dataTableLoading}
                                onClick={handleFilter}
                            />

                            <Button className='secondary me-2'
                                icon="pi pi-undo"
                                severity="secondary"
                                tooltip='Reset' tooltipOptions={{ position: 'bottom' }}
                                onClick={handleReset}
                            />
                            {/* <Button className='primary me-2'
                                icon="pi pi-file-excel"
                                loading={componentLoading.buttonLoading}
                                tooltip='Export' tooltipOptions={{ position: 'bottom' }}
                                onClick={() => getTicketDetails(true)}
                            /> */}
                            <Button
                                className='primary me-3'
                                icon="pi pi-download"
                                loading={componentLoading.buttonLoading}
                                tooltip='Export Report'
                                tooltipOptions={{ position: 'bottom' }}
                                onClick={() => {
                                    setSelectedExportFields(allExportFields);
                                    setShowExportDialog(true);
                                }}
                            />
                        </div>

                    </div>

                </Dialog>

            </BlockUI>

            <div className="list-wrapper">
                {
                    componentLoading.dataTableLoading &&
                    (
                        <DataTableSkeleton columns={DataTableSkeletonOptions.columns} rows={DataTableSkeletonOptions.rows} />
                    )
                }
                {
                    !componentLoading.dataTableLoading &&
                    (
                        <DataTable
                            dataKey="ticketId"
                            value={ticketList}
                            lazy
                            paginator
                            first={lazyState.first}
                            rows={lazyState.rows}
                            totalRecords={totalRecords}
                            onPage={onPage}
                            currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                            emptyMessage="No data available"
                            tableStyle={{ minWidth: '50rem' }}
                            scrollable
                        >
                            <Column
                                field="ticketId_Formatted"
                                header="Ticket ID"
                                style={{ whiteSpace: 'nowrap', maxWidth: '85px', minWidth: '85px' }}
                                frozen
                                body={(rowData) => (
                                    <span
                                        className="tktid clickable"
                                        style={{ color: '#007bff', cursor: 'pointer' }}
                                        onClick={() => handleTicketClick(rowData)}
                                    >
                                        {rowData.ticketId}
                                    </span>
                                )}
                            />
                            <Column field="status_Formatted" header="Status" style={{ maxWidth: '96px', minWidth: '96px' }} frozen={isMobile ? false : true}></Column>
                            <Column field="createdDate" header="Created Date" style={{ whiteSpace: 'nowrap' }}></Column>
                            <Column field="subject" header="Subject" body={(e) => truncatedRow(e, 'Subject')} style={{ maxWidth: '220px', minWidth: '220px' }}></Column>
                            <Column field="departmentName" header="Company" style={{ whiteSpace: 'nowrap' }}></Column>
                        </DataTable>

                    )
                }
            </div>
            <Dialog
                header={null}
                visible={ticketDialogVisible}
                style={{ width: '850px' }}
                modal
                onHide={() => setTicketDialogVisible(false)}
            >
                {selectedTicket && (
                    <TabView>

                        <TabPanel header="Ticket Details">
                            <div className="ticket-dialog-modern">
                                {selectedTicket && (
                                    <div className="ticket-dialog-modern">


                                        <div className="ticket-header">
                                            <h2>{selectedTicket.subject}</h2>
                                            <div className="ticket-meta">
                                                {selectedTicket.requesterName && selectedTicket.requesterName !== '-' ? (
                                                    <span>
                                                        {selectedTicket.requesterName} reported on{" "}
                                                        <strong>{selectedTicket.createdDate}</strong>
                                                    </span>
                                                ) : (
                                                    <span>
                                                        Reported on <strong>{selectedTicket.createdDate}</strong>
                                                    </span>
                                                )}
                                            </div>
                                        </div>


                                        <div className="ticket-section">
                                            <h4>Description</h4>
                                            <div className="desc-box">
                                                {selectedTicket.resolutionRemarks || selectedTicket.subject}
                                            </div>
                                        </div>
                                        <div className="ticket-section">
                                            <h4>Details</h4>
                                            <div className="details-grid">
                                                <div><label>Ticket ID:</label> {selectedTicket.ticketId}</div>
                                                <div>
                                                    <label>Status:</label>
                                                    <span className={`status-badge ${selectedTicket.StatusName?.toLowerCase().replace(/\s+/g, '-')}`}>
                                                        {selectedTicket.StatusName}
                                                    </span>
                                                </div>
                                                <div>
                                                    <label>Priority:</label>
                                                    <span className={`priority-badge ${selectedTicket.priority?.toLowerCase()}`}>
                                                        {selectedTicket.priority}
                                                    </span>
                                                </div>
                                                <div><label>Type:</label> {selectedTicket.type}</div>
                                                <div><label>Category:</label> {selectedTicket.category}</div>
                                                <div><label>Sub-Category:</label> {selectedTicket.subCategory}</div>
                                                <div><label>Resolution Status:</label> {selectedTicket.resolutionStatus}</div>
                                                <div><label>Roster Engineer:</label> {selectedTicket.onRoasterEngineer}</div>
                                            </div>
                                        </div>
                                        <div className="ticket-section">
                                            <h4>Time Info</h4>
                                            <div className="details-grid">
                                                <div><label>First Response:</label> {selectedTicket.firstResponseTimeInSecs}</div>
                                                <div><label>Resolution Time:</label> {selectedTicket.resolutionTimeInSecs}</div>
                                            </div>
                                        </div>
                                        {selectedTicket.requesterName && selectedTicket.requesterName !== '-' && (
                                            <div className="ticket-section contact-card">
                                                <h4>Contact Information</h4>
                                                <div className="contact-info">
                                                    <div className="avatar">{selectedTicket.requesterName?.charAt(0) || "U"}</div>
                                                    <div>
                                                        <strong>{selectedTicket.requesterName}</strong>
                                                        {selectedTicket.requesterEmail && <div>{selectedTicket.requesterEmail}</div>}
                                                        {selectedTicket.requesterMobile && <div>{selectedTicket.requesterMobile}</div>}
                                                    </div>
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                )}
                            </div>
                        </TabPanel>

                        <TabPanel header="Conversations">
                            {loadingConversations ? (
                                <div className="text-center p-3">
                                    <ProgressSpinner />
                                    <p>Loading conversations...</p>
                                </div>
                            ) : conversations.length === 0 ? (
                                <p className="text-center p-3">No conversations found.</p>
                            ) : (
                                <div className="conversation-container">
                                    {conversations.map((conv) => {

                                        const sender = extractSenderFromBody(conv.body);
                                        let createdAt = dayjs(conv.created_at);

                                        return (
                                            <div key={conv.id} className="conversation-card">
                                               
                                                
                                                    <div className="conversation-header">
                                                      {sender && (  <div className="avatar">{sender.charAt(0).toUpperCase()}</div>)}
                                                        <div className="conversation-meta">
                                                            {sender && (<strong>{sender}</strong>)}
                                                            <div className="conversation-date">
                                                                {sender ? (<>replied on {createdAt.format('DD MMM YYYY, h:mm A')}</>) : (<> {createdAt.format('DD MMM YYYY, h:mm A')}</>)}
                                                            </div>
                                                            {conv.to_emails?.length > 0 && <div><b>To:</b> {conv.to_emails.join(", ")}</div>}
                                                            {conv.cc_emails?.length > 0 && <div><b>Cc:</b> {conv.cc_emails.join(", ")}</div>}
                                                        </div>
                                                    </div>
                                                

                                                <div
                                                    className="conversation-body"
                                                    dangerouslySetInnerHTML={{ __html: conv.body }}
                                                />
                                            </div>
                                        );
                                    })}

                                </div>
                            )}
                        </TabPanel>


                    </TabView>
                )}
            </Dialog>



            <Dialog
                header="Select Fields to Export"
                visible={showExportDialog}
                style={{ width: '50vw' }}
                onHide={() => setShowExportDialog(false)}
                className="export-dialog"
            >
                <div className="header-row">
                    <h3 className="section-title"></h3>
                    <div className="select-all">
                        <Checkbox
                            inputId="selectAll"
                            checked={selectedExportFields.length === allExportFields.length}
                            onChange={(e) => {
                                if (e.checked) {
                                    setSelectedExportFields(allExportFields);
                                } else {
                                    setSelectedExportFields([]);
                                }
                            }}
                        />
                        <label htmlFor="selectAll">Select All</label>
                    </div>
                </div>

                {/* 2-column grid */}
                <div className="fields-grid">
                    {allExportFields.map((field, index) => (
                        <div
                            key={field}
                            className={`field-item ${selectedExportFields.includes(field) ? 'selected' : ''}`}
                            onClick={() => {
                                if (selectedExportFields.includes(field)) {
                                    setSelectedExportFields(selectedExportFields.filter((f) => f !== field));
                                } else {
                                    setSelectedExportFields([...selectedExportFields, field]);
                                }
                            }}
                        >
                            <Checkbox
                                inputId={`field_${index}`}
                                checked={selectedExportFields.includes(field)}
                                onChange={() => {
                                    if (selectedExportFields.includes(field)) {
                                        setSelectedExportFields(selectedExportFields.filter((f) => f !== field));
                                    } else {
                                        setSelectedExportFields([...selectedExportFields, field]);
                                    }
                                }}
                            />
                            <label htmlFor={`field_${index}`}>{index + 1}. {field}</label>
                        </div>
                    ))}
                </div>

                <div className="export-actions">
                    <Button
                        label="Export"
                        icon="pi pi-download"
                        className="p-button-success"
                        onClick={async () => {
                            await handleExportWithSelection();
                            setShowExportDialog(false);
                        }}
                    />
                </div>
            </Dialog>



        </div>
    );
};

export default TicketDetailsPage;