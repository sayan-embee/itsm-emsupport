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

import { ThemeContext } from "../../App";
import { useCallback, useContext, useEffect, useState } from "react";

import { getDepartmentMstAPI, getReportAPI, getReportOnmobileAPI, getReportFileAPI, getReportInExcelAPI, getReportSectionMstAPI, getUserAccessAPI } from "../../apis/APIList";
import { IDataTableSkeleton, IDropdownOption } from "../Interfaces";
import { HTTP_CODES, NO_DATA_AVAILABLE } from "../Constants";
import { IsNullOrEmpty } from "../../helpers/CommonMethod";
import Loader from "../common/Loader";
import { useAuth } from "../auth/AuthProvider";
import ErrorComponent from "../common/ErrorComponent";
import NorthStarAlert, { NorthStar_Alert_TYPES } from "../common/NorthStartAlert";
import ReportSectionSkeleton from "../skeleton/ReportSectionSkeleton";

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


const ReportTabComponent: React.FC = () => {
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
                "monthlyReportTab": true
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
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
    }, [teamsSSOToken]);



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
            console.log("getReportSectionMstAPI 1: ", selectedCompany);
            const response = await getReportSectionMstAPI(true, selectedCompany?.code);
            console.log("getReportSectionMstAPI: ", response);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));

            if (response?.data) {
                console.log("getReportSectionMstAPI data-response: ", response.data);
                setReportSectionList(response.data);

                const selectedReportSectionList: any[] = response.data?.filter((e: any) => !e.IsOptional);
                setSelectedReportSectionList(selectedReportSectionList);
            }
        }
        catch (error) {
            console.error("Error at getReportSection():", error);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
    }, [selectedReportType, selectedCompany]);



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
                let response;
                console.log("getReport onmobile: ", selectedCompany?.code, selectedCompany?.code?.toString());
                if (selectedCompany?.code?.toString() === "27000586401") {
                     // OnMobile DepartmentId
                    response = await getReportOnmobileAPI(jsonBody);
                    console.log("getReportOnmobileAPI: ", response);
                } else {
                    response = await getReportAPI(jsonBody);
                    console.log("getReportAPI: ", response);
                }
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

    //#endregion



    //#region Handlers

    const handleDropdownChange = async (e: any, type: DropdownType) => {
        console.log("handleChange: ", e);
         console.log("handleChange company: ", e);
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
                    console.log("handleChange company: 1", e, selectedCompany);
                    setCompany(e);
                    console.log("handleChange company: ", e, selectedCompany);
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
                    setFromDate(dayjs(e).startOf('month').toDate());
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

                setAlertVisible({
                    visible: true,
                    message: 'Presentation file generated successfully.',
                    type: NorthStar_Alert_TYPES.SUCCESS
                });

                await new Promise((resolve) => setTimeout(resolve, 1000));

                setAlertVisible({
                    visible: true,
                    message: 'Trying to download the file.',
                    type: NorthStar_Alert_TYPES.INFO
                });

                await new Promise((resolve) => setTimeout(resolve, 1000));

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
                        message: 'File downloaded successfully.',
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
        if (selectedReportType) {
            getDepartment();

        }
        if (selectedReportType && selectedCompany) {
            console.log("useEffect getReportSection: ", selectedCompany);
            getReportSection();
        }
    }, [selectedReportType, selectedCompany, getDepartment, getReportSection]);

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
        return (
            <div className={themeClass}>
                <div className="content-area flex-fill">
                    <div className="title-bar">
                        <div className="d-flex justify-content-between align-items-center">
                            <h5>Monthly Report</h5>
                        </div>
                    </div>
                    <div className="page-content-wrapper">
                        <div className="container-fluid mt-2">

                            <div className="mb-2">

                                <Toast ref={toast} />

                                <div className="row-container pb-2 container-fluid">

                                    {
                                        alertVisible.visible &&
                                        (
                                            <NorthStarAlert
                                                message={alertVisible.message}
                                                type={alertVisible.type}
                                            />
                                        )
                                    }

                                    <div className="row mb-1 align-items-end">
                                        <div className="col-md-3 mb-2">
                                            <label className="form-label">Report Type</label>
                                            {/* <Dropdown
                placeholder="Select type"
                value={selectedReportType}
                items={ReportTypeOptions}
                onChange={(event, e) => handleDropdownChange(e, DropdownType.ReportType)}
            /> */}
                                            <Dropdown
                                                className="full_width"
                                                value={selectedReportType}
                                                options={ReportTypeOptions}
                                                onChange={(e) => handleDropdownChange(e.value, DropdownType.ReportType)}
                                                optionLabel="name"
                                                placeholder="Select type"
                                                showClear
                                                invalid={invalidState.reportType}
                                            />
                                            {
                                                invalidState.reportType &&
                                                (
                                                    <Message className="custom-error-message" severity="error" text="required" />
                                                )
                                            }
                                        </div>

                                        <div className="col-md-3 mb-2">
                                            <label className="form-label">Company</label>
                                            {/* <Dropdown
                loading={componentLoading}
                loadingMessage={componentLoadingMsg}
                placeholder="Type Company Name"
                value={selectedCompany}
                items={companyOptionList}
                onChange={(event, e) => handleDropdownChange(e, DropdownType.Company)}
                search
                noResultsMessage={NO_DATA_AVAILABLE}
            /> */}
                                            <Dropdown
                                                className="full_width"
                                                value={selectedCompany}
                                                options={companyOptionList}
                                                onChange={(e) => handleDropdownChange(e.value, DropdownType.Company)}
                                                optionLabel="name"
                                                placeholder="Enter company name"
                                                showClear
                                                loading={componentLoading.companyLoading}
                                                emptyMessage={NO_DATA_AVAILABLE}
                                                filter
                                                filterBy="name"
                                                invalid={invalidState.company}
                                            />
                                            {
                                                invalidState.company &&
                                                (
                                                    <Message className="custom-error-message" severity="error" text="required" />
                                                )
                                            }
                                        </div>

                                        <div className="col-md-3 mb-2">
                                            <label className="form-label">Select Month & Year</label>
                                            {/* <Calendar
                className='full_width'
                placeholder="Select a date"
                showIcon
                // minDate={FromMinDateTime}
                // maxDate={selectedToDate ?? null}
                value={selectedFromDate}
                dateFormat="dd/mm/yy"
                onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                invalid={invalidState.fromDate}
            /> */}
                                            <Calendar value={selectedFromDate}
                                                placeholder="Select a month"
                                                showIcon
                                                onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                                view="month"
                                                dateFormat="mm/yy"
                                                maxDate={dayjs().endOf('month').toDate()}
                                            />
                                            {
                                                invalidState.fromDate &&
                                                (
                                                    <Message className="custom-error-message" severity="error" text="required" />
                                                )
                                            }
                                        </div>

                                        {/* <div className="col-md-2">
            <label className="form-label">To Date</label>
            <Calendar
                className='full_width'
                placeholder="Select a date"
                showIcon
                minDate={selectedFromDate ?? null}
                // maxDate={FromMaxDateTime}
                value={selectedToDate}
                dateFormat="dd/mm/yy"
                onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                invalid={invalidState.toDate}
            />
            {
                invalidState.toDate &&
                (
                    <Message className="custom-error-message" severity="error" text="required" />
                )
            }
        </div> */}

                                        <div className="col-md-2 mt-1 mb-2">
                                            <Button
                                                // className="btn btn-primary btn-download"
                                                className="primary-fill px-3"
                                                icon="pi pi-download"
                                                label="Download"
                                                loading={componentLoading.downloadLoading}
                                                disabled={componentLoading.downloadLoading}
                                                onClick={handleDownload}
                                            />
                                        </div>

                                    </div>

                                    <Divider className="m-0" />

                                </div>

                                {
                                    selectedReportType?.code === 1 &&
                                    (
                                        <div className="container-fluid mt-2">

                                            <div className="d-flex justify-content-between align-items-center mb-3">
                                                <h6 className="m-0">Select Report Sections:</h6>
                                                <div className="d-flex align-items-center pe-3">
                                                    <label className="me-2 m-0">Select All</label>
                                                    <Checkbox
                                                        inputId="selectAll"
                                                        value={selectedReportSectionList}
                                                        onChange={() => {
                                                            if (reportSectionList.every(reportSection =>
                                                                selectedReportSectionList.some(selectedSection => selectedSection.Code === reportSection.Code)
                                                            )) {
                                                                setSelectedReportSectionList([]);
                                                            } else {
                                                                setSelectedReportSectionList(reportSectionList);
                                                            }
                                                        }}
                                                        checked={reportSectionList.every(reportSection =>
                                                            selectedReportSectionList.some(selectedSection => selectedSection.Code === reportSection.Code)
                                                        )}
                                                    />
                                                </div>
                                            </div>

                                            {componentLoading.sectionLoading && <ReportSectionSkeleton />}

                                            {
                                                !componentLoading.sectionLoading &&
                                                (
                                                    <div className="row">
                                                        {
                                                            reportSectionList.map((m, index) => {
                                                                return [
                                                                    <div key={`report-section-${index}`} className="col-md-4 mb-2">
                                                                        <Card>
                                                                            <div className="d-flex justify-content-between align-items-center">
                                                                                <label htmlFor={m.Code}>{index + 1}. {m.Name}</label>
                                                                                <Checkbox
                                                                                    className="ms-2"
                                                                                    inputId={m.Code}
                                                                                    value={m}
                                                                                    onChange={handleOnCheckboxChange}
                                                                                    checked={selectedReportSectionList.some((e) => e.Code === m.Code)}
                                                                                />
                                                                            </div>
                                                                        </Card>
                                                                    </div>
                                                                ]
                                                            })
                                                        }
                                                    </div>
                                                )
                                            }

                                        </div>
                                    )
                                }
                            </div>

                        </div>
                    </div>
                </div>
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

export default ReportTabComponent;