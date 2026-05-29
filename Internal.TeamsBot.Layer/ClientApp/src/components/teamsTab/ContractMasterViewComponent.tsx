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
import { DataTable } from 'primereact/datatable';
import { Column } from "primereact/column";

import { ThemeContext } from "../../App";
import { useCallback, useContext, useEffect, useState } from "react";
import { useHistory, useLocation } from "react-router-dom";

import { getContractMstAPI, getDepartmentMstAPI, getReportAPI, getReportFileAPI, getReportInExcelAPI, getReportSectionMstAPI, getUserAccessAPI } from "../../apis/APIList";
import { IDataTableSkeleton, IDropdownOption } from "../Interfaces";
import { HTTP_CODES, NO_DATA_AVAILABLE } from "../Constants";
import { IsNullOrEmpty } from "../../helpers/CommonMethod";
import Loader from "../common/Loader";
import { useAuth } from "../auth/AuthProvider";
import ErrorComponent from "../common/ErrorComponent";
import NorthStarAlert, { CommonMessages, NorthStar_Alert_TYPES } from "../common/NorthStartAlert";
import DataTableSkeleton from "../skeleton/DatatableSkeleton";
import { Avatar } from "primereact/avatar";
import { Tooltip } from "primereact/tooltip";

// Images import
import contractIcon from '../../assets/contract.svg';
import ContractMasterCreateSkeleton from "../skeleton/ContractMasterCreateSkeleton";
import ContractMasterViewSkeleton from "../skeleton/ContractMasterViewSkeleton";

enum DropdownType {
    Month = "Month",
    Year = "Year",
    ReportType = "ReportType",
    Company = "Company",
}

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

const ReportTypeOptions = [
    { code: 1, name: "PowerPoint" },
    { code: 2, name: "Excel" },
];

interface LocationState {
    selectedId?: number;
}

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


const ContractMasterViewComponent: React.FC = () => {
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

    const location = useLocation<LocationState>();
    const { selectedId } = location.state || {};

    //#region State

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

    const [contractMasterList, setContractMasterList] = useState([]);
    const [selectedContractMaster, setSelectedContractMaster] = useState<any>(null);

    const [isAuthorized, setIsAuthorized] = useState<boolean>(true);

    const [invalidState, setInvalidState] = useState<IInvalidState>({
        fromDate: false,
        toDate: false,
        reportType: false,
        company: false
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

                let startDate = null, endDate = null;
                startDate = selectedFromDate ? dayjs(selectedFromDate).format('YYYY-MM-DD') : null;
                endDate = selectedToDate ? dayjs(selectedToDate).format('YYYY-MM-DD') : null;

                startDate = dayjs().startOf('year').format('YYYY-MM-DD');
                endDate = dayjs().endOf('year').format('YYYY-MM-DD');

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

                        const actionItem = (
                            <Button icon="pi pi-eye" rounded text severity="secondary" aria-label="View"
                                onClick={() => getContractMaster(e.id)}
                            />
                        );

                        return {
                            id: e.id,
                            slNo: e.slNo ?? '',
                            contractNo: e.contractNo ?? '',
                            tenantName: e.tenantName ?? '',
                            customerId: e.customerId ?? '',
                            customerName: e.customerName ?? '',
                            departmentName: e.departmentName ?? '',
                            startDate: e.startDate,
                            startDate_formatted: e.startDate ? dayjs(e.startDate).format('DD/MM/YYYY') : '',
                            endDate: e.endDate,
                            endDate_formatted: e.endDate ? dayjs(e.endDate).format('DD/MM/YYYY') : '',
                            acc_ManagerName: e.acc_ManagerName,
                            action: actionItem
                        }
                    });
                    setContractMasterList(dataList);
                }
            }

            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
        catch (error) {
            console.error("Error at getContractMaster():", error);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
            setAlertVisible({
                visible: true,
                message: CommonMessages.Error,
                type: NorthStar_Alert_TYPES.DANGER
            });
            handleRedirect('List');
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

    //#endregion



    //#region Handlers

    const handleRedirect = (type: 'Add' | 'View' | 'List', data?: any) => {
        switch (type) {
            case 'Add': {
                pageHistory.push('/newContract');
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

    // const handleDownload = async () => {
    //     try {
    //         if (!await checkMandatoryFields()) {
    //             return;
    //         }

    //         setComponentLoading((prevState) => ({ ...prevState, downloadLoading: true }));
    //         setAlertVisible({ visible: false, message: '', type: NorthStar_Alert_TYPES.NONE });

    //         const reportUrl = await getReport(selectedReportType?.name.toString() as 'Excel' | 'PowerPoint');
    //         if (!IsNullOrEmpty(reportUrl)) {
    //             const fileName = `${selectedCompany?.name}_${selectedReportType?.code === 1 ? `Presentation.pptx` : `Excel.xlsx`}`;
    //             const url = await getFile(reportUrl);
    //             if (fileName && url) {
    //                 const link = document.createElement('a');
    //                 link.href = url;
    //                 link.download = decodeURIComponent(fileName);
    //                 document.body.appendChild(link);
    //                 link.click();
    //                 document.body.removeChild(link);
    //                 URL.revokeObjectURL(url);

    //                 setAlertVisible({
    //                     visible: true,
    //                     message: 'File generated successfully.',
    //                     type: NorthStar_Alert_TYPES.SUCCESS
    //                 });
    //             }
    //         }
    //         else {
    //             setAlertVisible({
    //                 visible: true,
    //                 message: 'Something went wrong, please try again.',
    //                 type: NorthStar_Alert_TYPES.DANGER
    //             });
    //         }

    //         setComponentLoading((prevState) => ({ ...prevState, downloadLoading: false }));

    //     }
    //     catch (error) {
    //         console.error("Error at handleDownload():", error);
    //         setComponentLoading((prevState) => ({ ...prevState, downloadLoading: false }));
    //         setAlertVisible({
    //             visible: true,
    //             message: 'Something went wrong, please try again.',
    //             type: NorthStar_Alert_TYPES.DANGER
    //         });
    //     }
    // }

    const handleDownload = (url: string, fileName: string) => {
        try {
            const link = document.createElement("a");
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            setAlertVisible({
                visible: true,
                message: 'File downloaded successfully.',
                type: NorthStar_Alert_TYPES.SUCCESS
            });
        }
        catch (err) {
            setAlertVisible({
                visible: true,
                message: 'Something went wrong, please try again.',
                type: NorthStar_Alert_TYPES.DANGER
            });
        }
    };

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
        if (!selectedId) {
            pageHistory.push('/listContract');
        }
        else if (selectedId && userDetails) {
            getContractMaster(selectedId);
        }
    }, [selectedId, userDetails, getContractMaster, pageHistory]);

    //#endregion

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

        const statusCSS = getStatusCSS(selectedContractMaster?.activeStatusId ?? '');

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

                            {/* <div className="col-md-2">
                                <div className="side-nav-bar">
                                    <div className="title-bar">
                                        <h6 className="ms-2">Manage Application</h6>
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

                            <div className="col-md-12">
                                <div className="content-area flex-fill">

                                    <div className="title-bar">
                                        <div className="d-flex align-items-center">
                                            <Button className="back-btn" icon="pi pi-arrow-left" rounded text aria-label="Back"
                                                onClick={() => pageHistory.goBack()}
                                            />
                                            <h5 className="ms-2 mb-0">Contract Details</h5>
                                        </div>
                                    </div>

                                    <div className="page-content-wrapper">
                                        <div className="details">
                                            {/* <div className="card"> */}
                                            <div className="container-fluid mt-2">

                                                {componentLoading.sectionLoading && <ContractMasterViewSkeleton />}

                                                {
                                                    !componentLoading.sectionLoading &&
                                                    (
                                                        <>
                                                            <div className="row">
                                                                <div className="col-md-12">
                                                                    <div className="d-flex align-items-center mb-2">
                                                                        <div className="identity d-flex align-items-center">
                                                                            <div className="icon">
                                                                                <img src={contractIcon} alt="" />
                                                                            </div>
                                                                            <div>
                                                                                <h6 className="fw-semibold m-0 me-1">{selectedContractMaster?.departmentName ?? ''} [Contract No. {selectedContractMaster?.contractNo ?? ''}]</h6>
                                                                                {/* <p className="text-grey"></p> */}
                                                                            </div>
                                                                        </div>
                                                                        {/* <Button icon="pi pi-pencil" rounded text tooltip="Edit" tooltipOptions={{ position: 'top' }} /> */}
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div className="row mb-2 ps-4 ms-1">

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Status</p>
                                                                    <p className={`m-0 fw-semibold ${statusCSS}`} style={{ maxWidth: '180px' }}>{selectedContractMaster?.activeStatus ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Tenant</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.tenantName ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Department Id</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.departmentId ?? '-'}
                                                                    </p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Customer Name</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.customerName ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Contact Person Name</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.contactPersonName ?? '-'}
                                                                    </p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Contact Person Email</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.contactPersonEmail ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Contact Person Mobile</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.contactPersonPhone ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Region</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.regionName ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Account Manager Name</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.acc_ManagerName ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Account Manager Email</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.acc_ManagerEmail ?? '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Start Date</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.startDate ? dayjs(selectedContractMaster?.startDate).format('DD/MM/YYYY') : '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">End Date</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.endDate ? dayjs(selectedContractMaster?.endDate).format('DD/MM/YYYY') : '-'}</p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Category</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.categoryName ?? '-'}
                                                                        {
                                                                            selectedContractMaster?.categoryCode &&
                                                                            ' [' + selectedContractMaster?.categoryCode + ']'
                                                                        }
                                                                    </p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">Sub-Category</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.subCategoryName ?? '-'}
                                                                        {
                                                                            selectedContractMaster?.categoryCode && selectedContractMaster?.subCategoryCode &&
                                                                            ' [' + selectedContractMaster?.categoryCode + selectedContractMaster?.subCategoryCode + ']'
                                                                        }
                                                                    </p>
                                                                </div>

                                                                <div className="col-md-4 mb-3">
                                                                    <p className="text-grey m-0">PO No.</p>
                                                                    <p className="m-0 fw-semibold">{selectedContractMaster?.poNo ?? '-'}</p>
                                                                </div>



                                                                {
                                                                    selectedContractMaster?.fileList?.length > 0 &&
                                                                    (

                                                                        <div className="col-md-12 mb-3">
                                                                            <p className="text-grey mb-1">Attachment(s)</p>
                                                                            <ul className="uploaded-file">
                                                                                {
                                                                                    selectedContractMaster?.fileList.map((f: any, index: number) => {
                                                                                        return (
                                                                                            <li key={`file-${index}`} onClick={() => handleDownload(f.url, f.name)} style={{ cursor: 'pointer' }}>
                                                                                                {f.name} <i className="pi pi-download"></i>
                                                                                            </li>
                                                                                        )
                                                                                    })
                                                                                }
                                                                            </ul>
                                                                        </div>
                                                                    )
                                                                }
                                                            </div>
                                                        </>
                                                    )
                                                }

                                            </div>
                                            {/* </div> */}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div >

            </div >
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

export default ContractMasterViewComponent;