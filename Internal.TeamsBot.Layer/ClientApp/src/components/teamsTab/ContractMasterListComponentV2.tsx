// import { ComponentEventHandler, Datepicker, Dropdown, Input } from "@fluentui/react-northstar";
import "../../../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../../../node_modules/primereact/resources/primereact.min.css";
import "../../../node_modules/primeicons/primeicons.css";
import "./../../App.scss";

import React, { useRef } from "react";
import dayjs from "dayjs";
import { Button } from "primereact/button";
import { Dropdown } from "primereact/dropdown";
import { Calendar } from "primereact/calendar";
import { Message } from "primereact/message";
import { Card } from 'primereact/card';
import { Checkbox } from 'primereact/checkbox';
import { Divider } from 'primereact/divider';
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import { FilterMatchMode } from 'primereact/api';
import { DataTable, DataTableFilterMeta } from 'primereact/datatable';
import { Column } from "primereact/column";
import { debounce } from "lodash";
import { ThemeContext } from "../../App";
import { useCallback, useContext, useEffect, useState } from "react";
import { useHistory } from "react-router-dom";

import { getContractMstAPI, getDepartmentMstAPI, getReportAPI, getReportFileAPI, getReportInExcelAPI, getReportSectionMstAPI, getUserAccessAPI, saveContractMstAPI } from "../../apis/APIList";
import { IDataTableSkeleton, IDropdownOption, IState } from "../Interfaces";
import { HTTP_CODES, NO_DATA_AVAILABLE } from "../Constants";
import { IsNullOrEmpty } from "../../helpers/CommonMethod";
import Loader from "../common/Loader";
import { useAuth } from "../auth/AuthProvider";
import ErrorComponent from "../common/ErrorComponent";
import NorthStarAlert, { CommonMessages, NorthStar_Alert_TYPES } from "../common/NorthStartAlert";
import DataTableSkeleton from "../skeleton/DatatableSkeleton";
import { Tooltip } from "primereact/tooltip";
import { IconField } from 'primereact/iconfield';
import { InputIcon } from 'primereact/inputicon';
import { ScrollPanel } from 'primereact/scrollpanel';
import { motion } from "framer-motion";
import ContractMasterListSkeleton from "../skeleton/ContractMasterListSkeleton";
import { InputText } from "primereact/inputtext";
import { AutoComplete } from "primereact/autocomplete";
import { MultiSelect, MultiSelectChangeEvent } from "primereact/multiselect";
import { Dialog } from "primereact/dialog";
import { ToggleButton } from "primereact/togglebutton";
import { InputSwitch } from "primereact/inputswitch";
import { useAlertDialog } from "../common/AlertDialogProvider";

enum DropdownType {
    Month = "Month",
    Year = "Year",
    ReportType = "ReportType",
    Company = "Company",
    Status = "Status",
}

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

const ReportTypeOptions = [
    { code: 1, name: "PowerPoint" },
    { code: 2, name: "Excel" },
];

interface ILoadingState {
    pageLoading: boolean;
    companyLoading: boolean;
    downloadLoading: boolean;
    sectionLoading: boolean;
}

interface IInvalidState {
    fromDate: boolean;
    toDate: boolean;
    reportType: boolean;
    company: boolean;
}

interface INorthStarAlert {
    visible: boolean;
    message: string;
    type: string;
}


const ContractMasterListComponentV2: React.FC = () => {
    const theme = useContext(ThemeContext);
    const { teamsSSOToken, teamsSSOError, teamsSSOUser } = useAuth();

    const themeClass = theme?.siteVariables?.colors?.brand === "#333"
        ? 'dark-theme'
        : theme?.siteVariables?.colors?.brand === "#000"
            ? 'contrast-theme'
            : 'light-theme';
    // console.log("theme: ", theme);
    // console.log("themeClass: ", themeClass);

    const toast = useRef<Toast>(null);
    const pageHistory = useHistory();

    const { showDialog, hideDialog } = useAlertDialog();

    //#region State

    const [hasFiltered, setHasFiltered] = useState(false);
    const [extendDialogIsVisible, setExtendDialogIsVisible] = useState(false);
    const [isOpen, setIsOpen] = useState(false);

    const [selectedMonth, setMonth] = useState<number | undefined>(undefined);
    const [selectedYear, setYear] = useState<number | undefined>(undefined);

    const [selectedFromDate, setFromDate] = useState<any | null>(null);
    const [selectedToDate, setToDate] = useState<any | null>(null);

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: true,
        companyLoading: false,
        downloadLoading: false,
        sectionLoading: false
    });

    const [alertVisible, setAlertVisible] = useState<INorthStarAlert>({
        visible: false,
        message: '',
        type: NorthStar_Alert_TYPES.NONE
    });

    const [selectedReportType, setReportType] = useState<IDropdownOption | undefined>(undefined);

    const [userDetails, setUserDetails] = useState<any>(null);
    const [selectedCompany, setCompany] = useState<IDropdownOption | null>(null);
    const [companyOptionList, setCompanyOptionList] = useState<IDropdownOption[] | []>([]);
    const [reportSectionList, setReportSectionList] = useState<any[]>([]);
    const [selectedReportSectionList, setSelectedReportSectionList] = useState<any[]>([]);

    const [contractMasterList, setContractMasterList] = useState<any[]>([]);
    const [contractMasterDumpList, setContractMasterDumpList] = useState<any[]>([]);

    const [selectedContractMaster, setSelectedContractMaster] = useState(null);
    const [extendSupport, setExtendSupport] = useState(false);

    const [extendSupportButton, setExtendSupportButton] = useState<IState>({
        Value: null,
        IsRequired: false,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });


    const [selectedStatus, setSelectedStatus] = useState<IDropdownOption[] | []>([]);
    const [statusOptionList, setStatusOptionList] = useState<IDropdownOption[] | []>([]);

    const [isAuthorized, setIsAuthorized] = useState<boolean>(true);

    const [invalidState, setInvalidState] = useState<IInvalidState>({
        fromDate: false,
        toDate: false,
        reportType: false,
        company: false
    });

    const [searchTerm, setSearchTerm] = useState('');
    const [globalFilterValue, setGlobalFilterValue] = useState<string>('');

    const searchableFields = [
        "contractNo",
        "tenantName",
        "customerName",
        "departmentName",
        "categoryName",
        "subCategoryName",
        "acc_ManagerName",
        "acc_ManagerEmail"
    ];
    const [suggestion, setSuggestion] = useState<string[]>([]);
    const [allSuggestion, setAllSuggestion] = useState<string[]>([]);

    const [filters, setFilters] = useState<DataTableFilterMeta>({
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        slNo: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        contractNo: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        tenantName: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        customerName: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        departmentName: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        startDate_formatted: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        endDate_formatted: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        acc_ManagerName: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        acc_ManagerEmail: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        activeStatus: { value: null, matchMode: FilterMatchMode.EQUALS },
        categoryName: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
        subCategoryName: { value: null, matchMode: FilterMatchMode.STARTS_WITH }
    });

    //#endregion



    //#region API

    const getPageAccess = useCallback(async (): Promise<void> => {
        try {
            const jsonBody = {
                "userEmail": teamsSSOToken?.upn,
                "teamsTab": true,
                "contractTab": true
            }
            const response = await getUserAccessAPI(jsonBody);
            console.log("getUserAccessAPI: ", response);
            if (response?.data) {
                console.log("getUserAccessAPI data-response: ", response.data);
                setUserDetails(response.data);
            }
            else {
                setIsAuthorized(false);
            }
        }
        catch (error) {
            console.error("Error at getPageAccess():", error);
            setComponentLoading((prevState) => ({ ...prevState, pageLoading: false }));
        }
    }, [teamsSSOToken]);


    const getContractMaster = useCallback(async (Id: number = 0): Promise<void> => {
        try {
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: true }));
            if (Id > 0) {
                const jsonBody = {
                    "id": Id,
                    "contractNo": null,
                    "tenantId": null,
                    "createdByEmail": null,
                    "startDate": null,
                    "endDate": null
                }
                const response = await getContractMstAPI(jsonBody);
                console.log("getContractMstAPI: ", response);

                if (response.data) {
                    console.log("getContractMstAPI data-response: ", response.data);
                    setSelectedContractMaster(response.data);
                }
            }
            else {
                setSearchTerm('');

                let startDate = null, endDate = null;
                startDate = selectedFromDate ? dayjs(selectedFromDate).format('YYYY-MM-DD') : null;
                endDate = selectedToDate ? dayjs(selectedToDate).format('YYYY-MM-DD') : null;

                // startDate = dayjs().startOf('year').format('YYYY-MM-DD');
                // endDate = dayjs().endOf('year').format('YYYY-MM-DD');

                const jsonBody = {
                    "id": Id,
                    "contractNo": null,
                    "tenantId": null,
                    "createdByEmail": null,
                    "startDate": startDate,
                    "endDate": endDate
                }
                const response = await getContractMstAPI(jsonBody);
                console.log("getContractMstAPI: ", response);

                if (response.data) {
                    console.log("getContractMstAPI data-response: ", response.data);
                    const dataList = response.data.map((e: any) => {

                        // const actionItem = (
                        //     <>
                        //         {/* <Button icon="pi pi-eye" rounded text severity="secondary" aria-label="View"
                        //             tooltip="View details"
                        //             tooltipOptions={{ position: 'left' }}
                        //             onClick={() => handleRedirect('View', e.id)}
                        //         /> */}
                        //         <Button icon="pi pi-copy" rounded text severity="secondary" aria-label="Copy"
                        //             tooltip="Copy details to add new contract"
                        //             tooltipOptions={{ position: 'left' }}
                        //             onClick={() => handleRedirect('Add', e.id)}
                        //         />
                        //     </>
                        // );

                        let actionItem = null;

                        if (e.activeStatusId && (e.activeStatusId === 3)) {
                            actionItem = (
                                <>
                                    <Button className="btn-large-icon"
                                        icon="pi pi-copy" rounded text severity="secondary" aria-label="Copy"
                                        tooltip="Copy details to add new contract"
                                        tooltipOptions={{ position: 'left' }}
                                        onClick={() => handleRedirect('Add', e.id)}
                                    />
                                </>
                            );
                        }

                        const statusCSS = getStatusCSS(e.activeStatusId ?? '');

                        return {
                            id: e.id,
                            slNo: e.slNo ?? '',
                            contractNo: e.contractNo ?? '',
                            contractNo_Formatted: (<span style={{ cursor: 'pointer', fontWeight: 'bold', color: '#4E5FBF' }} onClick={() => handleRedirect('View', e.id)}>{e.contractNo}</span>),
                            tenantName: e.tenantName ?? '',
                            customerId: e.customerId ?? '',
                            customerName: e.customerName ?? '',
                            departmentName: e.departmentName ?? '',
                            startDate: e.startDate,
                            startDate_formatted: e.startDate ? dayjs(e.startDate).format('DD/MM/YYYY') : '',
                            endDate: e.endDate,
                            endDate_formatted: e.endDate ? dayjs(e.endDate).format('DD/MM/YYYY') : '',
                            acc_ManagerName: e.acc_ManagerName,
                            acc_ManagerEmail: e.acc_ManagerEmail,
                            extendSupport: e?.extendSupport ?? null,
                            activeStatusId: e.activeStatusId ?? null,
                            activeStatus: e.activeStatus ?? '',
                            activeStatus_formatted: (<p className={statusCSS} >{e.activeStatus ?? ''}</p>),
                            // categoryName: e?.categoryName ? e?.categoryName + ' (' + e?.categoryCode + ')' : '',
                            categoryName: e?.categoryName ? e?.categoryName : '',
                            subCategoryName: e?.subCategoryName ? e?.subCategoryName + ' [' + e?.categoryCode + e?.subCategoryCode + ']' : '',
                            action: actionItem
                        }
                    });
                    setContractMasterList(dataList);
                    setContractMasterDumpList(dataList);

                    //For Status
                    if (dataList?.length > 0) {
                        const uniqueStatus = new Set<string>();

                        dataList.forEach((item: any) => {
                            uniqueStatus.add(item.activeStatus);
                        });

                        const statusList = Array.from(uniqueStatus).map((status) => ({ code: status, name: status }));
                        const sorted_statusList = statusList.sort((a, b) => a.name.localeCompare(b.name));
                        setStatusOptionList(sorted_statusList);
                    }

                    // For Suggestion
                    if (dataList?.length > 0) {
                        const uniqueSuggestions = new Set<string>();

                        dataList.forEach((item: any) => {
                            searchableFields.forEach((field) => {
                                if (item[field]) {
                                    uniqueSuggestions.add(item[field]);
                                }
                            });
                        });
                        setAllSuggestion(Array.from(uniqueSuggestions));
                    }
                    else {
                        setAllSuggestion([]);
                    }

                }
            }

            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
        catch (error) {
            console.error("Error at getContractMaster():", error);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
    }, []);


    const getDepartment = useCallback(async (): Promise<void> => {
        try {
            if (!selectedReportType) return;
            setComponentLoading((prevState) => ({ ...prevState, companyLoading: true }));
            const jsonBody = {
                "name": '',
                "id": 0,
                "ReportType": selectedReportType.name,
                "active": true
            }
            const response = await getDepartmentMstAPI(jsonBody);
            console.log("getDepartmentMstAPI: ", response);
            setComponentLoading((prevState) => ({ ...prevState, companyLoading: false }));

            if (response.data) {
                console.log("getDepartmentMstAPI data-response: ", response.data);
                const optionList: IDropdownOption[] = response.data.map((e: any) => {
                    return {
                        code: e.id,
                        name: e.name
                    }
                });
                setCompanyOptionList(optionList);
            }
        }
        catch (error) {
            console.error("Error at getDepartment():", error);
            setComponentLoading((prevState) => ({ ...prevState, companyLoading: false }));
        }
    }, [selectedReportType]);



    const getReportSection = useCallback(async (): Promise<void> => {
        try {
            if (!selectedReportType) return;
            if (selectedReportType?.code !== 1) return;
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: true }));
            const response = await getReportSectionMstAPI(true);
            console.log("getReportSectionMstAPI: ", response);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));

            if (response?.data) {
                console.log("getReportSectionMstAPI data-response: ", response.data);
                setReportSectionList(response.data);
                setSelectedReportSectionList(response.data);
            }
        }
        catch (error) {
            console.error("Error at getReportSection():", error);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
    }, [selectedReportType]);



    const getReport = async (type: 'Excel' | 'PowerPoint'): Promise<string> => {
        try {

            if (type === 'PowerPoint') {

                const slideConfigList_Obj = selectedReportSectionList.map((e: any) => {
                    return {
                        slideCode: e.Code,
                        slideName: e.Name,
                        columns: '',
                        customColumns: '',
                        sortOrder: e.SortOrder
                    }
                });

                const jsonBody = {
                    "departmentId": selectedCompany?.code,
                    "start_date": dayjs(selectedFromDate).format('DD/MM/YYYY'),
                    "end_date": dayjs(selectedToDate).format('DD/MM/YYYY'),
                    "slideCodeList": selectedReportSectionList.map((e: any) => e.Code).join(','),
                    "slideConfigList": slideConfigList_Obj
                }
                const response = await getReportAPI(jsonBody);
                console.log("getReportAPI: ", response);
                if (response.status === HTTP_CODES.OK) {
                    return response.data?.message ?? '';
                }
            }
            else if (type === 'Excel') {
                const jsonBody = {
                    "departmentId": selectedCompany?.code,
                    "start_date": dayjs(selectedFromDate).format('DD/MM/YYYY'),
                    "end_date": dayjs(selectedToDate).format('DD/MM/YYYY')
                }
                const response = await getReportInExcelAPI(jsonBody);
                console.log("getReportInExcelAPI: ", response);
                if (response.status === HTTP_CODES.OK) {
                    return response.data?.message ?? '';
                }
            }

            return '';
        }
        catch (error) {
            console.error("Error at getReport():", error);
            return '';
        }
    }

    const getFile = async (e: any): Promise<string> => {
        try {
            if (!IsNullOrEmpty(e)) {
                const response = await getReportFileAPI(e);
                console.log("getReportFileAPI: ", response);
                if (response?.status === HTTP_CODES.OK) {

                    // const contentDisposition = response.headers['content-disposition'];

                    // const fileName = contentDisposition
                    //     ?.split('filename*=UTF-8\'\'')[1]
                    //     ?.replace(/"/g, '') || 'download.pptx';

                    const blob = new Blob([response.data], { type: response.headers['content-type'] });
                    const url = URL.createObjectURL(blob);
                    return url;
                }
            }
            return '';
        }
        catch (error) {
            console.error("Error at getReport():", error);
            return '';
        }
    }

    //#endregion



    //#region Helpers

    const setDefaultValues = () => {
        const fromDate = dayjs().add(-1, 'month').startOf('month').toDate();
        const toDate = dayjs().add(-1, 'month').endOf('month').toDate();

        setFromDate(fromDate);
        setToDate(toDate);
    }

    const getStatusCSS = (e: number | null) => {
        if (e === 2) {
            return 'active';
        }
        else if (e === 3) {
            return 'inactive';
        }
        else if (e === 4) {
            return 'extend';
        }
        else if (e === 1) {
            return 'upcoming';
        }
        else return '';
    }

    const resetValues = (type: 'Invalid-State') => {
        if (type === 'Invalid-State') {
            setInvalidState({
                fromDate: false,
                toDate: false,
                reportType: false,
                company: false
            });
        }
    }

    const checkMandatoryFields = async () => {
        let invalidState: IInvalidState = {
            fromDate: false,
            toDate: false,
            reportType: false,
            company: false
        };

        if (!selectedReportType) invalidState.reportType = true;
        if (!selectedCompany) invalidState.company = true;
        if (!selectedFromDate) invalidState.fromDate = true;
        if (!selectedToDate) invalidState.toDate = true;

        setInvalidState(invalidState);

        if (Object.values(invalidState).some((state) => state)) {
            return false;
        }

        return true;
    }

    //#endregion



    //#region Handlers

    const handleRedirect = (type: 'Add' | 'View' | 'List', data?: any) => {
        switch (type) {
            case 'Add': {
                // pageHistory.push('/newContract');
                pageHistory.push({
                    pathname: '/newContract',
                    state: { selectedId: data }
                });
                break;
            }

            case 'View': {
                pageHistory.push({
                    pathname: '/viewContract',
                    state: { selectedId: data }
                });
                break;
            }

            case 'List': {
                pageHistory.push('/listContract');
                break;
            }
            default: break;
        }
    }

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true, redirect: string = '', history: any = undefined) => {
        showDialog(header, content, isClosable, redirect, history);
    };

    const handleDropdownChange = async (e: any, type: DropdownType) => {
        console.log("handleChange: ", e);
        resetValues('Invalid-State');

        switch (type) {
            case DropdownType.ReportType:
                {
                    setReportType(e);
                    setCompany(null);
                    setCompanyOptionList([]);

                    break;
                }

            case DropdownType.Company:
                {
                    setCompany(e);
                    break;
                }

            case DropdownType.Month:
                {
                    setMonth(e);
                    break;
                }

            case DropdownType.Year:
                {
                    setYear(e);
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
            case DropdownType.Status:
                {
                    setSelectedStatus(e);

                    if (e?.length > 0) {
                        const selectedStatus = e.map((f: { code: any; name: any; }) => f.code);
                        const filteredContractMasterList = contractMasterDumpList.filter(f => selectedStatus.includes(f.activeStatus));
                        setContractMasterList(filteredContractMasterList);
                    }
                    else {
                        setContractMasterList(contractMasterDumpList);
                    }

                    break;
                }

            default:
                break;
        }
    }

    const handleOnCheckboxChange = async (e: any) => {
        console.log("handleOnCheckboxChange: ", e);

        let _selectedReportSectionList = [...selectedReportSectionList];
        console.log("_selectedReportSectionList: ", _selectedReportSectionList);

        if (e.checked) {
            _selectedReportSectionList.push(e.value);
        }
        else {
            _selectedReportSectionList = selectedReportSectionList.filter(f => f.Code !== e.value.Code);
        }
        setSelectedReportSectionList(_selectedReportSectionList);
    };

    const handleInputSwitch = (rowData: any, newValue: boolean) => {

        // Update local state
        const updatedData = contractMasterList.map((item) =>
            item.id === rowData.id ? { ...item, extendSupport: newValue } : item
        );
        setContractMasterList(updatedData);

        // Send update to backend
        handleSubmit('ExtendSupport', rowData, newValue);
    };

    const handleDateChange = async (e: any, type: DateType) => {
        console.log("handleDateChange: ", e);
        resetValues('Invalid-State');

        const MAX_DAY_DIFF = 30;

        switch (type) {
            case DateType.FromDate:
                {
                    setFromDate(e);
                    setToDate(dayjs(e).endOf('month').toDate());
                    break;
                }

            case DateType.ToDate:
                {
                    if (dayjs(e).diff(dayjs(selectedFromDate), 'day') > MAX_DAY_DIFF) {
                        setToDate(dayjs(selectedFromDate).add(MAX_DAY_DIFF, 'day').toDate());
                    }
                    else {
                        setToDate(e);
                    }
                    break;
                }

            default:
                break;
        }
    }

    const handleDownload = async () => {
        try {
            if (!await checkMandatoryFields()) {
                return;
            }

            setComponentLoading((prevState) => ({ ...prevState, downloadLoading: true }));
            setAlertVisible({ visible: false, message: '', type: NorthStar_Alert_TYPES.NONE });

            const reportUrl = await getReport(selectedReportType?.name.toString() as 'Excel' | 'PowerPoint');
            if (!IsNullOrEmpty(reportUrl)) {
                const fileName = `${selectedCompany?.name}_${selectedReportType?.code === 1 ? `Presentation.pptx` : `Excel.xlsx`}`;
                const url = await getFile(reportUrl);
                if (fileName && url) {
                    const link = document.createElement('a');
                    link.href = url;
                    link.download = decodeURIComponent(fileName);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    URL.revokeObjectURL(url);

                    setAlertVisible({
                        visible: true,
                        message: 'File generated successfully.',
                        type: NorthStar_Alert_TYPES.SUCCESS
                    });
                }
            }
            else {
                setAlertVisible({
                    visible: true,
                    message: 'Something went wrong, please try again.',
                    type: NorthStar_Alert_TYPES.DANGER
                });
            }

            setComponentLoading((prevState) => ({ ...prevState, downloadLoading: false }));

        }
        catch (error) {
            console.error("Error at handleDownload():", error);
            setComponentLoading((prevState) => ({ ...prevState, downloadLoading: false }));
            setAlertVisible({
                visible: true,
                message: 'Something went wrong, please try again.',
                type: NorthStar_Alert_TYPES.DANGER
            });
        }
    }

    const onGlobalFilterChange = useCallback(
        debounce((e: React.ChangeEvent<HTMLInputElement>) => {
            const value = e.target.value.toLowerCase().trim();
            let _filters = { ...filters };

            if (value?.toLowerCase() === "active" || value?.toLowerCase() === "inactive") {
                _filters["global"] = { value, matchMode: FilterMatchMode.EQUALS };
                _filters["activeStatus"] = { value, matchMode: FilterMatchMode.EQUALS };
            } else {
                _filters["global"] = { value, matchMode: FilterMatchMode.CONTAINS };
            }

            setFilters(_filters);
            setGlobalFilterValue(value);
        }, 300),
        [filters]
    );


    const onSearch = (event: { query: string }) => {
        if (allSuggestion.length > 0) {
            const query = event.query.toLowerCase().trim();

            const filteredSuggestions = allSuggestion.filter((item) => {
                const lowerItem = item.toLowerCase();

                // Exact match for "active" avoiding "inactive"
                if (query === "active") {
                    return lowerItem === "active"; // Only exact match
                }

                return lowerItem.includes(query);
            });

            setSuggestion(filteredSuggestions);
        }
    };

    const handleAutoInputChange = (e: any) => {
        setSearchTerm(e.value);
        onGlobalFilterChange(e);
    };

    const handleAutoInputReset = () => {
        setSearchTerm('');

        // Synthetic input event
        const syntheticEvent = {
            target: { value: '' }
        } as React.ChangeEvent<HTMLInputElement>;

        onGlobalFilterChange(syntheticEvent);
    };

    const handleSelectSuggestion = (e: any) => {
        setSearchTerm(e.value);

        // Synthetic input event
        const syntheticEvent = {
            target: { value: e.value }
        } as React.ChangeEvent<HTMLInputElement>;

        onGlobalFilterChange(syntheticEvent);
    };

    const handleSubmit = async (transactionType = 'ExtendSupport', rowData: any, newValue: boolean) => {
        try {
            if (transactionType === 'ExtendSupport') {
                if (!rowData) {
                    setAlertVisible({
                        visible: true,
                        message: CommonMessages.Error,
                        type: NorthStar_Alert_TYPES.DANGER
                    });
                    return;
                }

                setExtendSupportButton({ ...extendSupportButton, IsLoading: true, IsDisabled: true });

                const formData = new FormData();

                const jsonBody = {
                    "transactionType": "E",
                    "id": rowData.id,
                    "extendSupport": newValue ?? false,
                    "modifiedByName": userDetails?.userName ?? null,
                    "modifiedEmail": userDetails?.userEmail ?? null,
                    "fileList": null
                }

                formData.append("eventData", JSON.stringify(jsonBody));

                console.log("saveContractMstAPI-jsonBody: ", jsonBody);
                const response = await saveContractMstAPI(formData);
                console.log("saveContractMstAPI: ", response);

                if (response.data && response.data?.status == 1) {
                    console.log("saveContractMstAPI data-response: ", response.data);
                    // handleShowAlert('Success', response.data?.message, true, '/listContract', pageHistory);
                    setAlertVisible({
                        visible: true,
                        message: response.data?.message,
                        type: NorthStar_Alert_TYPES.SUCCESS
                    });
                }
                else if (response.data && response.data?.status == 0 && response.data?.message) {
                    setAlertVisible({
                        visible: true,
                        message: response.data?.message,
                        type: NorthStar_Alert_TYPES.DANGER
                    });
                }
                else {
                    setAlertVisible({
                        visible: true,
                        message: CommonMessages.Error,
                        type: NorthStar_Alert_TYPES.DANGER
                    });
                }

                setExtendSupportButton({ ...extendSupportButton, IsLoading: true, IsDisabled: false });
            }
        }
        catch (error) {
            console.error("Error at handleSubmit():", error);
            setExtendSupportButton({ ...extendSupportButton, IsLoading: false, IsDisabled: false });
            setAlertVisible({
                visible: true,
                message: CommonMessages.Error,
                type: NorthStar_Alert_TYPES.DANGER
            });
        }
        finally {
            setExtendSupportButton({ ...extendSupportButton, IsLoading: false, IsDisabled: false });
            await getContractMaster();
        }
    }

    //#endregion

    //#region Effects

    useEffect(() => {
        if (teamsSSOToken) {
            console.log("teamsSSOToken: ", teamsSSOToken);
            getPageAccess();
            setDefaultValues();
            setComponentLoading((prevState) => ({ ...prevState, pageLoading: false }));
        }
    }, [teamsSSOToken, getPageAccess]);

    useEffect(() => {
        if (userDetails) {
            getContractMaster();
        }
    }, [userDetails, getContractMaster]);

    //#endregion

    const formattedRow = (rowData: any, type: 'acc_ManagerEmail') => {
        if (type === 'acc_ManagerEmail') {
            const name = rowData.acc_ManagerName;
            const subject = rowData.acc_ManagerEmail;
            const tooltipId = `accManagerEmail-tooltip-${rowData.id}`;
            return (
                <>
                    <span id={tooltipId} style={{ cursor: 'pointer' }}>{name}</span>
                    <Tooltip target={`#${tooltipId}`} content={subject} position="top" />
                </>
            );
        }
    };

    const extendSupportTemplate = (rowData: any) => {
        if (rowData.activeStatusId && (rowData.activeStatusId === 3 || rowData.activeStatusId === 4)) {
            return (
                <InputSwitch
                    checked={rowData.extendSupport}
                    onChange={(e) => handleInputSwitch(rowData, e.value)}
                    className="small-switch"
                    tooltip={`Extend support${rowData.extendSupport ? ' is active' : ' is inactive'}`}
                    tooltipOptions={{ position: 'left' }}
                    disabled={extendSupportButton.IsDisabled}
                />
            );
        }
        else {
            return null;
        }
    };

    if (teamsSSOError) {
        return (
            <div className="d-flex justify-content-center align-items-center vh-100">
                <Message severity="error" text={teamsSSOError} />
            </div>
        );
    }

    else if (!isAuthorized) {
        return (
            <ErrorComponent message="You are not authorized to access this page" severity="error" />
        )
    }

    else if (userDetails) {

        const DataTableSkeletonOptions = {
            columns: ['Sl. No.', 'Contract No.', 'Status', 'Tenant', 'Customer', 'Department', 'Category', 'Sub-Category', 'Start', 'End', 'Account Manager'],
            rows: 0
        }

        return (
            <div className={themeClass}>

                <div className="d-flex align-items-stretch">
                    <div className="container-fluid p-0">

                        <Toast ref={toast} />

                        {
                            alertVisible.visible &&
                            (
                                <NorthStarAlert
                                    message={alertVisible.message}
                                    type={alertVisible.type}
                                />
                            )
                        }

                        <div className="row g-0">

                            {/* Side Panel */}
                            {/* <div className="col-md-2">
                                <div className="side-nav-bar">
                                    <div className="title-bar">
                                        <h6 className="m-0">Manage Application</h6>
                                    </div>
                                    <div className="nav-bar">
                                        <ul className="nav">
                                            <li className="active">Manage Contact</li>
                                            <li>Manage Contact</li>
                                            <li>Manage Contact</li>
                                        </ul>
                                    </div>
                                </div>
                            </div> */}

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
                                            <div className="col-md-12">Menu 1</div>
                                        </div><div className="col-md-12 mb-2">
                                            <div className="col-md-12">Menu 2</div>
                                        </div>
                                    </div>
                                </ScrollPanel>
                                <div className="bottom-action">
                                </div>
                            </motion.div>

                            <div className="col-md-12">
                                <div className="content-area flex-fill">

                                    {
                                        extendSupportButton.IsLoading && <Loader />
                                    }

                                    <div className="title-bar">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <Button
                                                className={`${isOpen ? 'primary' : ''}`}
                                                icon='pi pi-bars'
                                                severity="info"
                                                outlined={!isOpen ? true : false}
                                                tooltip={`${isOpen ? 'Collapse Menu' : 'Expand Menu'}`} tooltipOptions={{ position: 'top' }}
                                                onClick={() => setIsOpen(!isOpen)}
                                            />
                                            <h5>Manage Contracts</h5>
                                            {/* <Button className="primary-fill px-3 me-2" label="Add New" icon="pi pi-plus" onClick={() => handleRedirect('Add')} /> */}
                                            <div className="d-flex justify-content-end">
                                                <MultiSelect
                                                    className="me-2"
                                                    value={selectedStatus}
                                                    onChange={(e: MultiSelectChangeEvent) => handleMultiSelectChange(e.value, DropdownType.Status)}
                                                    options={statusOptionList}
                                                    optionLabel="name"
                                                    filter
                                                    filterBy="name"
                                                    placeholder="Select Status"
                                                    clearIcon
                                                    maxSelectedLabels={1}
                                                />
                                                {/* <IconField iconPosition="left" className="search-field">
                                                    <InputIcon className="pi pi-search" />
                                                    <InputText value={searchTerm} onChange={onSearchChange} onInput={onGlobalFilterChange} placeholder="Keyword Search" />
                                                </IconField> */}
                                                <div className="p-inputgroup flex-1">
                                                    <AutoComplete
                                                        value={searchTerm}
                                                        suggestions={suggestion}
                                                        completeMethod={onSearch}
                                                        onChange={handleAutoInputChange}
                                                        onSelect={handleSelectSuggestion}
                                                        placeholder="Keyword Search"
                                                        className="p-inputtext-sm"
                                                    />
                                                    <Button
                                                        icon={searchTerm ? "pi pi-times" : "pi pi-search"} className={searchTerm ? "p-button-danger" : "p-button-secondary"}
                                                        onClick={() => {
                                                            if (!searchTerm) return;
                                                            handleAutoInputReset();
                                                        }}
                                                        type="button"
                                                    />
                                                </div>
                                                <Button className="primary-fill px-3 ms-3 add-btn" label="Add New" icon="pi pi-plus" onClick={() => handleRedirect('Add')} />
                                            </div>

                                        </div>
                                    </div>

                                    <motion.div className="main-content"
                                        animate={{ marginRight: isOpen ? 300 : 0, width: isOpen ? "calc(100% - 300px)" : "100%" }}
                                        transition={{ duration: 0.1, ease: "easeInOut" }}
                                    >
                                        <div className="page-content-wrapper">
                                            <div className="container-fluid mt-2">

                                                {
                                                    componentLoading.sectionLoading &&
                                                    (
                                                        <>
                                                            <DataTableSkeleton columns={DataTableSkeletonOptions.columns} rows={DataTableSkeletonOptions.rows} />
                                                            <ContractMasterListSkeleton />
                                                        </>
                                                    )
                                                }

                                                {
                                                    !componentLoading.sectionLoading &&
                                                    (
                                                        <DataTable dataKey="id" value={contractMasterList}
                                                            paginator rows={25} rowsPerPageOptions={[25, 50, 75]}
                                                            currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                                                            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                                                            emptyMessage="No data available"
                                                            tableStyle={{ minWidth: '50rem' }}
                                                            scrollable
                                                            // filters={filters}
                                                            // filterDisplay="row"
                                                            globalFilter={globalFilterValue}
                                                            globalFilterFields={['slNo', 'contractNo', 'activeStatus', 'tenantName', 'customerName', 'departmentName', 'categoryName', 'subCategoryName', 'startDate_formatted', 'endDate_formatted', 'acc_ManagerName', 'acc_ManagerEmail']}
                                                        >
                                                            <Column field="slNo" header="Sl. No." frozen style={{ whiteSpace: 'nowrap', maxWidth: '50px', minWidth: '50px' }}></Column>
                                                            <Column field="contractNo_Formatted" header="Contract No." frozen style={{ whiteSpace: 'nowrap', maxWidth: '145px', minWidth: '145px' }}></Column>
                                                            <Column field="activeStatus_formatted" header="Status" style={{ maxWidth: '120px', minWidth: '75px' }}></Column>
                                                            <Column field="tenantName" header="Tenant" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            {/* <Column field="customerId" header="Customer Id"></Column> */}
                                                            <Column field="customerName" header="Customer" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="departmentName" header="Department" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="categoryName" header="Category" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="subCategoryName" header="Sub-Category" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="startDate_formatted" header="Start" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="endDate_formatted" header="End" style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="acc_ManagerName" header="Account Manager" body={(e) => formattedRow(e, 'acc_ManagerEmail')} style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="extendSupport" header="Extend Support" body={extendSupportTemplate} style={{ whiteSpace: 'nowrap' }}></Column>
                                                            <Column field="action" header="Action" style={{ whiteSpace: 'nowrap' }}></Column>
                                                        </DataTable>
                                                    )
                                                }

                                            </div>
                                        </div>
                                    </motion.div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <Dialog header="Extend Support" visible={extendDialogIsVisible} style={{ width: '80vw' }} onHide={() => setExtendDialogIsVisible(false)}>
                    <div className="text-center">
                        <div className="row">
                            <div className="col-md-4">
                                <p>Extend Support</p>
                            </div>
                            <div className="col-md-2">
                                <ToggleButton checked={extendSupport} onChange={(e) => setExtendSupport(e.value)} className="w-8rem" />
                            </div>
                        </div>
                    </div>
                </Dialog>

            </div>
        );
    }

    else {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <Loader />
            </div>
        )
    }
};

export default ContractMasterListComponentV2;