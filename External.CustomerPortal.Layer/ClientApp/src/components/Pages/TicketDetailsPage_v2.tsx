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
import { OverlayPanel } from 'primereact/overlaypanel';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import { ScrollPanel } from 'primereact/scrollpanel';
import { motion } from "framer-motion";

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
import { GetCustomerDetailsAPI, getNewOTPAPI, getTicketDetailsAPI, logoutAPI, verifyOTPAPI } from '../../apis/APIList';
import { CommonMessage, showErrorToast, showInfoToast, showSuccessToast, showWarningToast, toastMessageType } from '../common/ToastComponent';
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

interface Category {
    name: string;
    key: string;
}

//#endregion

const TicketDetailsPage_v2 = () => {
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
    const [lazyState, setLazyState] = useState<LazyTableState>({
        first: 0,
        rows: 10,
        page: 1,
        sortField: null,
        sortOrder: null,
        filters: {}
    });

    const statusOptions: IDropdownOption[] = [
        { code: -1, name: 'All Tickets', info: alltkt },
        { code: 5, name: 'Closed Tickets', info: closetkt },
        { code: 2, name: 'Open Tickets', info: opentkt }
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
        columns: ['Ticket ID', 'Status', 'Created Date', 'Type', 'Category', 'Subject', 'Priority', 'Sub-Category', 'Requester', 'Company'],
        rows: 5
    }
    const [visible2, setVisible2] = useState<boolean>(false);

    const [hasFiltered, setHasFiltered] = useState(false);

    const [isOpen_CustomizedColumn, set_CustomizedColumn] = useState(false);

    //#endregion






    // #region Filter Sidebar
    const [isOpen, setIsOpen] = useState(false);
    // #region Column Customization
    const ref_CustomizedColumn = useRef<OverlayPanel>(null);
    const [checked, setChecked] = useState<boolean>(false);

    const [selectedOption, setSelectedOption] = useState(null);
    const options = [
        { name: 'All Tickets' },
        { name: 'Closed Tickets' },
        { name: 'Open Tickets' }
    ];

    const categories: Category[] = [
        { name: 'Accounting', key: 'A' },
        { name: 'Marketing', key: 'M' },
        { name: 'Production', key: 'P' },
        { name: 'Research', key: 'R' }
    ];
    const [selectedCategories, setSelectedCategories] = useState<Category[]>([categories[1]]);

    const onCategoryChange = (e: CheckboxChangeEvent) => {
        let _selectedCategories = [...selectedCategories];

        if (e.checked)
            _selectedCategories.push(e.value);
        else
            _selectedCategories = _selectedCategories.filter(category => category.key !== e.value.key);

        setSelectedCategories(_selectedCategories);
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
                    // "departmentId_List": '27000586401,27001473235',
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
                    "departmentId_List": '27000586401,27001473235', // For Testing
                    // "departmentId_List": dept_Id_List,
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
                    let createdDate_display = e.created_at_display;
                    if (createdDate) {
                        const date = dayjs(createdDate, "DD-MM-YYYY HH:mm:ss");
                        createdDate_display = date.format("DD/MM/YYYY HH:mm:ss");
                        createdDate = date.valueOf();
                    }

                    const statusClass = GetStatusClassStyle(e.status ?? 0);

                    return {
                        ticketId: e.id,
                        ticketId_Formatted: (<span className="tktid">{e.id}</span>),
                        departmentId: e.department_id,
                        departmentName: e.name,
                        createdDate: createdDate,
                        createdDate_display: createdDate_display,
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

    const toggleCustomizedColumn = (e: any) => {
        set_CustomizedColumn(!isOpen_CustomizedColumn);
        ref_CustomizedColumn.current?.toggle(e);
    }

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
        const isValid = await validateFields('Filter');
        if (!isValid) {
            return;
        }
        else {
            getTicketDetails();
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


    useEffect(() => {
        if (componentLoading.hasReset) {
            getTicketDetails();
        }
    }, [componentLoading.hasReset]);

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

    const renderTooltipInRow = (rowData: any, type: 'Requester') => {
        if (type === 'Requester') {
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

    const datatableBodyTemplate = (rowData: any, type: 'Status' | 'TicketId') => {
        if (type === 'Status') {
            const statusClass = GetStatusClassStyle(rowData.statusId ?? 0);
            return <span className={`status ${statusClass} m-0`}>{rowData.status}</span>;
        }

        if (type === 'TicketId') {
            return <span className="tktid">{rowData.ticketId}</span>;
        }
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

    return (
        <div>

            <motion.div className="mt-4 main-content"
                animate={{ marginRight: isOpen ? 300 : 0, width: isOpen ? "calc(100% - 300px)" : "100%" }}
                transition={{ duration: 0.1, ease: "easeInOut" }}
            >
                <div className="container-fluid px-md-5 px-4 mb-2">

                    {componentLoading.buttonLoading && <LoaderComponent />}

                    <BlockUI className="transparent-blockui" blocked={componentLoading.dataTableLoading || componentLoading.buttonLoading}>
                        <div className="list-page-header mb-3 d-none d-md-flex align-items-center justify-content-between">

                            <Toast ref={toastRef} />

                            <div className="d-flex align-items-center">
                                <Button className="back-btn" icon="pi pi-arrow-left" rounded aria-label="Back" onClick={handleNavigate} />
                                <div className="report-form"></div>
                            </div>

                            <div className="d-flex align-items-center">
                                <Button className='me-2'
                                    severity="success"
                                    outlined
                                    icon="pi pi-file-excel"
                                    loading={componentLoading.buttonLoading}
                                    tooltip='Export Tickets' tooltipOptions={{ position: 'top' }}
                                    onClick={() => getTicketDetails(true)}
                                />
                                <Button
                                    className={`${isOpen ? 'primary' : ''}`}
                                    icon='pi pi-filter'
                                    severity="info"
                                    outlined={!isOpen ? true : false}
                                    tooltip={`${isOpen ? 'Collapse Filters' : 'Expand Filters'}`} tooltipOptions={{ position: 'top' }}
                                    onClick={() => setIsOpen(!isOpen)}
                                />
                            </div>

                        </div>
                        <div className="list-page-header mb-3 d-flex d-md-none align-items-center justify-content-between">
                            <Button className="back-btn" icon="pi pi-arrow-left" rounded aria-label="Back" />
                            {/* <Button label="Filter" icon="pi pi-filter" className='primary' onClick={() => setVisible2(true)} /> */}
                            <Button
                                className={`${isOpen ? 'primary' : ''}`}
                                icon='pi pi-filter'
                                severity="info"
                                outlined={!isOpen ? true : false}
                                onClick={() => setIsOpen(!isOpen)}
                            />
                        </div>

                        <Dialog className="filter-modal" header="Filter" visible={visible2} style={{ width: '95vw' }} onHide={() => setVisible2(false)}>
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
                                        icon={hasFiltered ? 'pi pi-filter' : 'pi pi-filter-slash'}
                                        className={`${hasFiltered ? 'primary' : ''} me-2`}
                                        severity={!hasFiltered ? 'secondary' : 'contrast'}
                                        tooltip={hasFiltered ? 'Apply filter' : 'Filter'} tooltipOptions={{ position: 'bottom' }}
                                        loading={componentLoading.dataTableLoading}
                                        onClick={handleFilter}
                                    />

                                    <Button className='secondary me-2'
                                        icon="pi pi-undo"
                                        severity="secondary"
                                        tooltip='Reset' tooltipOptions={{ position: 'bottom' }}
                                        onClick={handleReset}
                                    />
                                    <Button className='primary me-2'
                                        icon="pi pi-file-excel"
                                        loading={componentLoading.buttonLoading}
                                        tooltip='Export' tooltipOptions={{ position: 'bottom' }}
                                        onClick={() => getTicketDetails(true)}
                                    />
                                </div>

                            </div>

                        </Dialog>

                    </BlockUI>

                    <div className="list-wrapper">

                        <Button
                            style={{ background: `${!isOpen_CustomizedColumn ? '' : '#10446f'}` }}
                            className='col-set-btn'
                            outlined={!isOpen_CustomizedColumn ? true : false}
                            icon="pi pi-cog"
                            onClick={(e) => toggleCustomizedColumn(e)}
                        />

                        {/* Choose Columns Section */}
                        <OverlayPanel ref={ref_CustomizedColumn} dismissable={false} style={{ width: "360px" }}>
                            <h6 className="text-blue">Choose columns</h6>
                            <IconField iconPosition="left" className="col-search mb-3">
                                <InputIcon className="pi pi-search"></InputIcon>
                                <InputText className="w-100" placeholder="Search here..." />
                            </IconField>

                            <ScrollPanel style={{ width: '100%', maxHeight: '190px' }}>
                                <div className="row">
                                    <div className="col-md-6">
                                        <div className="d-flex align-items-center mb-2">
                                            <Checkbox onChange={e => setChecked(e.checked ?? false)} checked={checked}></Checkbox>
                                            <label className="ms-1 text-xs">Ticket ID</label>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="d-flex align-items-center mb-2">
                                            <Checkbox onChange={e => setChecked(e.checked ?? false)} checked={checked}></Checkbox>
                                            <label className="ms-1 text-xs">Created Date</label>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="d-flex align-items-center mb-2">
                                            <Checkbox onChange={e => setChecked(e.checked ?? false)} checked={checked}></Checkbox>
                                            <label className="ms-1 text-xs">Subject</label>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="d-flex align-items-center mb-2">
                                            <Checkbox onChange={e => setChecked(e.checked ?? false)} checked={checked}></Checkbox>
                                            <label className="ms-1 text-xs">Status</label>
                                        </div>
                                    </div>
                                </div>
                            </ScrollPanel>

                            <div className="mt-4">
                                <Button className='primary py-2' label="Apply" />
                                <Button className='ms-2 p-2' outlined icon="pi pi-refresh" />
                            </div>
                        </OverlayPanel>

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
                                    // selectionMode={rowClick ? null : 'multiple'}
                                    // selection={selectedTicketList ?? []}
                                    // onSelectionChange={(e: any) => setSelectedTicketList(e.value)}
                                    // lazy
                                    paginator
                                    // first={lazyState.first}
                                    rows={lazyState.rows}
                                    // totalRecords={totalRecords}
                                    // onPage={onPage}
                                    currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                                    paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                                    emptyMessage="No data available"
                                    tableStyle={{ minWidth: '50rem' }}
                                    //size="small"
                                    // filters={filters}
                                    // globalFilterFields={['ticketId']}
                                    scrollable
                                    removableSort
                                    sortField="createdDate" sortOrder={1}
                                    columnResizeMode="expand" resizableColumns
                                    reorderableColumns
                                    showGridlines
                                //scrollHeight="calc(100vh-200px)"
                                >

                                    {/* <Column selectionMode="multiple" headerStyle={{ width: '3rem' }}></Column> */}
                                    <Column field="ticketId" header="Ticket ID" sortable style={{ whiteSpace: 'nowrap', maxWidth: '85px', minWidth: '85px' }} frozen body={(e) => datatableBodyTemplate(e, 'TicketId')}></Column>
                                    <Column field="status" header="Status" sortable style={{ maxWidth: '96px', minWidth: '96px' }} frozen body={(e) => datatableBodyTemplate(e, 'Status')}></Column>
                                    <Column field="createdDate_display" sortable header="Created Date" style={{ whiteSpace: 'nowrap' }}></Column>
                                    <Column field="type" header="Type" sortable style={{ whiteSpace: 'nowrap' }}></Column>
                                    <Column field="category" header="Category" sortable style={{ whiteSpace: 'nowrap' }}></Column>
                                    <Column field="subject" header="Subject" body={(e) => truncatedRow(e, 'Subject')} style={{ maxWidth: '220px', minWidth: '220px' }}></Column>
                                    <Column field="priority" header="Priority" sortable style={{ whiteSpace: 'nowrap' }}></Column>
                                    <Column field="subCategory" header="Sub-Category" sortable style={{ whiteSpace: 'nowrap' }}></Column>
                                    <Column field="requesterName" header="Requester" sortable body={(e) => renderTooltipInRow(e, 'Requester')} style={{ maxWidth: '160px', minWidth: '160px' }}></Column>
                                    <Column field="departmentName" header="Company" sortable style={{ whiteSpace: 'nowrap' }}></Column>
                                </DataTable>
                            )
                        }
                    </div>

                </div>
            </motion.div>

            <motion.div
                initial={{ x: "100%" }}
                animate={{ x: isOpen ? 0 : "100%" }}
                transition={{ duration: 0.5, ease: "easeInOut" }}
                className="sidebar pt-2"
            >
                <div className="d-flex justify-content-between align-items-center mb-2 pb-2 border-bottom">
                    <h6 className="text-blue m-0">Filter</h6>
                    <Button className="p-1" label="Close" severity="secondary" icon="pi pi-times" text onClick={() => setIsOpen(!isOpen)} />
                </div>

                <ScrollPanel style={{ width: '100%', height: 'calc(100vh - 230px)' }}>
                    <div className="row">
                        <div className="col-md-12 mb-2">
                            <label>Ticket Status</label>
                            <Dropdown
                                className='w-100 me-2'
                                value={selectedStatus}
                                onChange={(e: DropdownChangeEvent) => handleDropdownChange(e.value, DropdownType.Status)}
                                options={statusOptionList}
                                optionLabel="name"
                                placeholder="Select status"
                                valueTemplate={statusDropdownValueTemplate}
                                itemTemplate={statusDropdownItemTemplate}
                            />
                        </div>
                        {
                            departmentOptionList.length > 1 &&
                            (
                                <div className="col-md-12 mb-2">
                                    <label>Contracts</label>
                                    <MultiSelect
                                        className='w-100'
                                        value={selectedDepartment}
                                        onChange={(e: MultiSelectChangeEvent) => handleMultiSelectChange(e.value, DropdownType.Department)}
                                        options={departmentOptionList}
                                        optionLabel="name"
                                        filter
                                        filterBy="name"
                                        placeholder="Select company"
                                        maxSelectedLabels={1}
                                    />
                                </div>
                            )
                        }
                        <div className="col-md-12 mb-2">
                            <label>Ticket ID</label>
                            <InputText
                                className="p-inputtext-sm w-100"
                                value={ticketId.Value}
                                onChange={(e) => handleTextChange(e.target.value, TextType.TicketId)}
                                onKeyDown={handleNumericKeyPress}
                                maxLength={10}
                                minLength={6}
                                placeholder='Enter a Ticket ID'
                            />
                            {/* <IconField className='me-1' iconPosition="left">
                                <InputIcon className="pi pi-search"></InputIcon>
                                <Chips
                                    className='w-100'
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
                            </IconField> */}
                        </div>
                        <div className="col-md-12 mb-2">
                            <div className="row g-2">
                                <div className="col-md-6">
                                    <label>From Date</label>
                                    <Calendar className="w-100" inputId="startDate" showIcon placeholder={`From date *`}
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
                                </div>
                                <div className="col-md-6">
                                    <label>To Date</label>
                                    <Calendar className="w-100" inputId="endDate" showIcon placeholder="To date *"
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
                                </div>
                            </div>
                        </div>
                        <div className="col-md-12 mb-2">
                            <label>Other Status</label>
                            <Dropdown value={selectedOption} onChange={(e) => setSelectedOption(e.value)} options={options} optionLabel="name"
                                placeholder="Select" className="w-100" />
                        </div>
                        <div className="col-md-12 mb-2">
                            <label className="mb-2">Issue Type</label>
                            <div className="d-flex flex-column mb-2 gap-2">
                                {categories.map((category) => {
                                    return (
                                        <div key={category.key} className="flex align-items-center">
                                            <Checkbox inputId={category.key} name="category" value={category} onChange={onCategoryChange} checked={selectedCategories.some((item) => item.key === category.key)} />
                                            <label htmlFor={category.key} className="ms-2">
                                                {category.name}
                                            </label>
                                        </div>
                                    );
                                })}
                            </div>
                        </div>
                    </div>
                </ScrollPanel>
                <div className="bottom-action">
                    <Button
                        className='primary py-2 me-1'
                        label="Apply"
                        severity={!hasFiltered ? 'secondary' : 'contrast'}
                        loading={componentLoading.dataTableLoading}
                        onClick={handleFilter}
                    />
                    <Button className='p-2'
                        icon="pi pi-undo"
                        outlined
                        tooltip='Reset' tooltipOptions={{ position: 'top' }}
                        onClick={handleReset}
                    />
                </div>
            </motion.div>

        </div>
    );
};

export default TicketDetailsPage_v2;