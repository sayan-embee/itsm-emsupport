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
import { BlockUI } from 'primereact/blockui';
import { useDropzone } from 'react-dropzone';
import { v4 as uuidv4 } from "uuid";

import { ThemeContext } from "../../App";
import { useCallback, useContext, useEffect, useState } from "react";
import { useHistory, useLocation } from "react-router-dom";

import { getContractMstAPI, getDepartmentMstAPI, getMasterDataAPI, getReportAPI, getReportFileAPI, getReportInExcelAPI, getReportSectionMstAPI, getUserAccessAPI, saveContractMstAPI } from "../../apis/APIList";
import { IDataTableSkeleton, IDropdownOption, IState } from "../Interfaces";
import { HTTP_CODES, NO_DATA_AVAILABLE } from "../Constants";
import { handleEmailKeyPress, handleMobileNumberKeyPress, handleTextKeyPress, IsNullOrEmpty, UseRegex } from "../../helpers/CommonMethod";
import Loader from "../common/Loader";
import { useAuth } from "../auth/AuthProvider";
import ErrorComponent from "../common/ErrorComponent";
import NorthStarAlert, { CommonMessages, NorthStar_Alert_TYPES } from "../common/NorthStartAlert";
import DataTableSkeleton from "../skeleton/DatatableSkeleton";
import { Avatar } from "primereact/avatar";
import { Tooltip } from "primereact/tooltip";
import { InputText } from "primereact/inputtext";
import { EMAIL_REGEX } from "../Constants";
import ContractMasterCreateSkeleton from "../skeleton/ContractMasterCreateSkeleton";
import { useAlertDialog } from "../common/AlertDialogProvider";

// Images import
// import contractIcon from './Assets/contract.svg';

enum DropdownType {
    Tenant = 'Tenant',
    CustomerName = 'CustomerName',
    ContractName = 'ContractName',
    Category = 'Category',
    SubCategory = 'SubCategory',
    Region = 'Region',
}

enum DateType {
    FromDate = "FromDate",
    ToDate = "ToDate",
}

enum TextType {
    acc_ManagerName = "acc_ManagerName",
    acc_ManagerEmail = "acc_ManagerEmail",
    po_No = 'po_No'
}

interface LocationState {
    selectedId?: number;
}

interface ILoadingState {
    pageLoading: boolean;
    sectionLoading: boolean;
    customerLoading: boolean;
    contractLoading: boolean;
    subCategoryLoading: boolean;
}

interface INorthStarAlert {
    visible: boolean;
    message: string;
    type: string;
}

interface IMasterData {
    TenantList: any[];
    CategoryList: any[];
    SubCategoryList: any[];
    RegionList: any[];
    CustomerList: any[];
    DepartmentList: any[];
}


const ContractMasterCreateComponent: React.FC = () => {
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

    const { showDialog, hideDialog } = useAlertDialog();

    //#region State

    const [isAuthorized, setIsAuthorized] = useState<boolean>(true);

    const [selectedMonth, setMonth] = useState<number | undefined>(undefined);
    const [selectedYear, setYear] = useState<number | undefined>(undefined);

    const [selectedFromDate, setFromDate] = useState<any | null>(null);
    const [selectedToDate, setToDate] = useState<any | null>(null);

    const [componentLoading, setComponentLoading] = useState<ILoadingState>({
        pageLoading: true,
        sectionLoading: false,
        customerLoading: false,
        contractLoading: false,
        subCategoryLoading: false
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

    const [masterDataList, setMasterDataList] = useState<IMasterData>({
        TenantList: [],
        CategoryList: [],
        SubCategoryList: [],
        RegionList: [],
        CustomerList: [],
        DepartmentList: []
    });
    const [contractMasterList, setContractMasterList] = useState([]);
    const [selectedContractMaster, setSelectedContractMaster] = useState<any>(null);

    const [tenantList, setTenantList] = useState<IDropdownOption[]>([]);
    const [selectedTenant, setSelectedTenant] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [customerList, setCustomerList] = useState<IDropdownOption[]>([]);
    const [selectedCustomer, setSelectedCustomer] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [contractList, setContractList] = useState<IDropdownOption[]>([]);
    const [selectedContract, setSelectedContract] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [categoryList, setCategoryList] = useState<IDropdownOption[]>([]);
    const [selectedCategory, setSelectedCategory] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [subCategoryList, setSubCategoryList] = useState<IDropdownOption[]>([]);
    const [selectedSubCategory, setSelectedSubCategory] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [startDate, setStartDate] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [endDate, setEndDate] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [regionList, setRegionList] = useState<any[]>([]);
    const [selectedRegion, setSelectedRegion] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [acc_ManagerName, setAccManagerName] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [acc_ManagerEmail, setAccManagerEmail] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [poNo, setPoNo] = useState<IState>({
        Value: '',
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
    });

    const [newFileList, setNewFileList] = useState<any[]>([]);

    const [submitButton, setSubmitButton] = useState<IState>({
        Value: null,
        IsRequired: true,
        IsValid: true,
        IsDisabled: false,
        IsLoading: false,
        ErrorMessage: ''
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

    const getMasterData = async (): Promise<void> => {
        try {
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: true }));
            const response = await getMasterDataAPI();
            console.log("getMasterDataAPI: ", response);
            if (response.data) {
                console.log("getMasterDataAPI data-response: ", response.data);
                let tenantList = response.data?.TenantList || [];
                let regionList = response.data?.RegionList || [];
                let categoryList = response.data?.CategoryList || [];
                let subCategoryList = response.data?.SubCategoryList || [];

                let customerList = response.data?.CustomerList || [];
                let departmentList = response.data?.DepartmentList || [];

                let tenantListDropDown: IDropdownOption[] = tenantList.map((item: any) => { return { code: item.id, name: item.tenantName } });
                let regionListDropDown: IDropdownOption[] = regionList.map((item: any) => { return { code: item.id, name: item.regionName } });
                let categoryListDropDown: IDropdownOption[] = categoryList.map((item: any) => { return { code: item.id, name: (item.categoryName + " (" + item.categoryCode + ")") } });
                // let subCategoryListDropDown: IDropdownOption[] = subCategoryList.map((item: any) => { return { code: item.id, name: item.subCategoryName } });

                setTenantList(tenantListDropDown);
                setRegionList(regionListDropDown);
                setCategoryList(categoryListDropDown);
                // setSubCategoryList(subCategoryListDropDown);

                setMasterDataList({
                    TenantList: tenantList,
                    RegionList: regionList,
                    CategoryList: categoryList,
                    SubCategoryList: subCategoryList,
                    CustomerList: customerList,
                    DepartmentList: departmentList
                });
            }

            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
        }
        catch (error) {
            console.error("Error at getMasterData():", error);
            setComponentLoading((prevState) => ({ ...prevState, sectionLoading: false }));
            setAlertVisible({
                visible: true,
                message: CommonMessages.Error,
                type: NorthStar_Alert_TYPES.DANGER
            });
        }
    };


    const getContractMaster = async (Id: number = 0): Promise<void> => {
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

                    const e = response.data;

                    if (e?.tenantId && e?.tenantName) {
                        // handleDropdownChange({ code: e?.tenantId, name: e?.tenantName }, DropdownType.Tenant);
                        setSelectedTenant({ ...selectedTenant, Value: { code: e?.tenantId, name: e?.tenantName }, IsValid: true, ErrorMessage: '' });

                        if (masterDataList?.CustomerList?.length > 0) {
                            setComponentLoading({ ...componentLoading, customerLoading: true });
                            const customerListDropDown: IDropdownOption[] = masterDataList.CustomerList
                                .filter(f => f.tenant && (f.tenant).trim() === (e.tenantName).trim())
                                .map((item: any) => { return { code: item.embee_crm_id, name: item.sap_customer_name, info: item.tenant } });
                            console.log('filtered customerListDropDown: ', customerListDropDown);
                            setCustomerList(customerListDropDown);
                            setComponentLoading({ ...componentLoading, customerLoading: false });
                        }

                        // Customer Name
                        if (e?.customerId && e?.customerName) {
                            // handleDropdownChange({ code: e?.customerId, name: e?.customerName, info: e.tenantName }, DropdownType.CustomerName);

                            setSelectedCustomer({ ...selectedCustomer, Value: { code: e.customerId, name: e.customerName, info: e.tenantName }, IsValid: true, ErrorMessage: '' });

                            if (masterDataList?.DepartmentList?.length > 0) {
                                setComponentLoading({ ...componentLoading, contractLoading: true });
                                const contractListDropDown: IDropdownOption[] = masterDataList.DepartmentList
                                    .filter(f => f.tenant && (f.tenant).trim() === (e.tenantName).trim() && f.embee_crm_id && (f.embee_crm_id).trim() === (e.customerId).trim())
                                    .map((item: any) => { return { code: item.departmentId, name: item.departmentName } });
                                console.log('filtered contractListDropDown: ', contractListDropDown);
                                setContractList(contractListDropDown);
                                setComponentLoading({ ...componentLoading, contractLoading: false });
                            }

                            // Contract / Department Name
                            if (e?.departmentId && e?.departmentName) {
                                // handleDropdownChange({ code: e?.departmentId, name: e?.departmentName }, DropdownType.ContractName);
                                setSelectedContract({ ...selectedContract, Value: { code: e.departmentId, name: e.departmentName }, IsValid: true, ErrorMessage: '' });
                            }
                        }

                        // Category
                        if (e?.categoryId && e?.categoryName) {
                            // handleDropdownChange({ code: e?.categoryId, name: e?.categoryName }, DropdownType.Category);
                            const selectedCategory = categoryList.find(f => f.code === e.categoryId);
                            if (selectedCategory) {
                                setSelectedCategory({ ...selectedCategory, Value: selectedCategory, IsValid: true, ErrorMessage: '', IsRequired: true });

                                if (masterDataList?.SubCategoryList?.length > 0) {
                                    setComponentLoading({ ...componentLoading, subCategoryLoading: true });
                                    const subCategoryListDropDown: IDropdownOption[] = masterDataList.SubCategoryList.filter(f => f.categoryId === e.categoryId).map((item: any) => { return { code: item.id, name: (item.subCategoryName + ' (' + item.subCategoryCode + ')') } });
                                    setSubCategoryList(subCategoryListDropDown);
                                    setComponentLoading({ ...componentLoading, subCategoryLoading: false });


                                    // Sub-Category
                                    if (e?.subCategoryId && e?.subCategoryName) {
                                        // handleDropdownChange({ code: e?.subCategoryId, name: e?.subCategoryName }, DropdownType.SubCategory);
                                        const selectedSubCategory = subCategoryListDropDown.find(f => f.code === e.subCategoryId);
                                        if (selectedSubCategory) {
                                            setSelectedSubCategory({ ...selectedSubCategory, Value: selectedSubCategory, IsValid: true, ErrorMessage: '', IsRequired: true });
                                        }
                                    }
                                }
                            }
                        }

                        if (e?.acc_ManagerName) {
                            setAccManagerName({ ...acc_ManagerName, Value: e.acc_ManagerName, IsValid: true, ErrorMessage: '' });
                        }
                        if (e?.acc_ManagerEmail) {
                            setAccManagerEmail({ ...acc_ManagerEmail, Value: e.acc_ManagerEmail, IsValid: true, ErrorMessage: '' });
                        }

                        if (e?.regionId && e?.regionName) {
                            // handleDropdownChange({ code: e?.regionId, name: e?.regionName }, DropdownType.Region);
                            setSelectedRegion({ ...selectedRegion, Value: { code: e?.regionId, name: e?.regionName }, IsValid: true, ErrorMessage: '' });
                        }

                        if (e?.startDate && dayjs(e?.startDate).isValid()) {
                            setStartDate({ ...startDate, Value: dayjs(e?.startDate).add(1, 'year').toDate(), IsValid: true, ErrorMessage: '' });
                        }

                        if (e?.endDate && dayjs(e?.endDate).isValid()) {
                            setEndDate({ ...endDate, Value: dayjs(e?.endDate).add(1, 'year').toDate(), IsValid: true, ErrorMessage: '' });
                        }
                    }
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
                            <Button icon="pi pi-eye" rounded text severity="secondary" aria-label="View" />
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
    };


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

    const validateEmail = async () => {
        if (acc_ManagerEmail.Value === '') {
            // setAccManagerEmail({ ...acc_ManagerEmail, IsValid: false, ErrorMessage: 'Enter an email' });
        } else if (!UseRegex(EMAIL_REGEX, acc_ManagerEmail.Value)) {
            setAccManagerEmail({ ...acc_ManagerEmail, IsValid: false, ErrorMessage: 'Please enter a valid email address' });
        } else {
            setAccManagerEmail({ ...acc_ManagerEmail, IsValid: true, ErrorMessage: '' });
        }
    };

    const setDefaultValues = () => {
        const fromDate = dayjs().add(-1, 'month').startOf('month').toDate();
        const toDate = dayjs().add(-1, 'month').endOf('month').toDate();

        setFromDate(fromDate);
        setToDate(toDate);
    }

    const resetValues = () => {
        window.location.reload();
    }

    const checkMandatoryFields = async (): Promise<boolean> => {
        let returnValue = true;

        if (!selectedTenant.Value || selectedTenant.Value === null) {
            setSelectedTenant({ ...selectedTenant, IsValid: false, ErrorMessage: 'Select a tenant' });
            returnValue = false;
        }

        if (!selectedCustomer.Value || selectedCustomer.Value === null) {
            setSelectedCustomer({ ...selectedCustomer, IsValid: false, ErrorMessage: 'Select a customer' });
            returnValue = false;
        }

        if (!selectedContract.Value || selectedContract.Value === null) {
            setSelectedContract({ ...selectedContract, IsValid: false, ErrorMessage: 'Select a contract' });
            returnValue = false;
        }

        if (!selectedCategory.Value || selectedCategory.Value === null) {
            setSelectedCategory({ ...selectedCategory, IsValid: false, ErrorMessage: 'Select a category' });
            returnValue = false;
        }

        if (!selectedSubCategory.Value || selectedSubCategory.Value === null) {
            setSelectedSubCategory({ ...selectedSubCategory, IsValid: false, ErrorMessage: 'Select a sub-category' });
            returnValue = false;
        }

        if (!startDate.Value || startDate.Value === null) {
            setStartDate({ ...startDate, IsValid: false, ErrorMessage: 'Select a start date' });
            returnValue = false;
        }

        if (!endDate.Value || endDate.Value === null) {
            setEndDate({ ...endDate, IsValid: false, ErrorMessage: 'Select an end date' });
            returnValue = false;
        }

        // if (IsNullOrEmpty(acc_ManagerName.Value)) {
        //     setAccManagerName({ ...acc_ManagerName, IsValid: false, ErrorMessage: 'Enter an account manager name' });
        //     returnValue = false;
        // }

        // if (IsNullOrEmpty(acc_ManagerEmail.Value)) {
        //     setAccManagerEmail({ ...acc_ManagerEmail, IsValid: false, ErrorMessage: 'Enter an account manager email' });
        //     returnValue = false;
        // }

        return returnValue;
    }

    //#endregion



    //#region Handlers

    const handleShowAlert = (header: string, content: string, isClosable: boolean = true, redirect: string = '', history: any = undefined) => {
        showDialog(header, content, isClosable, redirect, history);
    };


    //#region File Upload

    // Dropzone configuration
    const { acceptedFiles, getRootProps, getInputProps } = useDropzone({
        accept: {
            'application/pdf': ['.pdf'],
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
            'application/msword': ['.doc'],
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx'], // Accept .docx
            'image/png': ['.png'], // Accept .png
            'image/jpeg': ['.jpg', '.jpeg'], // Accept .jpg and .jpeg
            'image/gif': ['.gif'], // Accept .gif
            'text/csv': ['.csv'], // Accept .csv files
        },
        onDrop: (acceptedFiles: any) => {
            const newFiles = acceptedFiles.map((file: { file: any; name: any; size: any; type: any; }) => ({
                file: file,
                name: file.name,
                size: file.size,
                type: file.type,
                internalName: uuidv4()
            }));
            setNewFileList(prevFiles => [...prevFiles, ...newFiles]);
        }
    });

    const removeFile = (internalName: string) => {
        setNewFileList(prevFiles => prevFiles.filter(file => file.internalName !== internalName));
    };

    const files = newFileList.map(file => (
        <li key={file.name}>
            {file.name} <i className="pi pi-times" onClick={() => removeFile(file.internalName)}></i>
        </li>
    ));

    //#endregion


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

        switch (type) {
            case DropdownType.Tenant:
                {
                    setSelectedTenant({ ...selectedTenant, Value: e ?? null, IsValid: true, ErrorMessage: '' });

                    if (e?.code && masterDataList?.CustomerList?.length > 0) {
                        setComponentLoading({ ...componentLoading, customerLoading: true });
                        const customerListDropDown: IDropdownOption[] = masterDataList.CustomerList
                            .filter(f => f.tenant && (f.tenant).trim() === (e.name).trim())
                            .map((item: any) => { return { code: item.embee_crm_id, name: item.sap_customer_name, info: item.tenant } });
                        console.log('filtered customerListDropDown: ', customerListDropDown);
                        setCustomerList(customerListDropDown);
                        await new Promise((resolve) => setTimeout(resolve, 2000));
                        setComponentLoading({ ...componentLoading, customerLoading: false });
                    }
                    else {
                        setCustomerList([]);
                    }
                    setSelectedCustomer({ ...selectedCustomer, Value: null, IsValid: true, ErrorMessage: '' });

                    setContractList([]);
                    setSelectedContract({ ...selectedContract, Value: null, IsValid: true, ErrorMessage: '' });

                    break;
                }

            case DropdownType.CustomerName:
                {
                    setSelectedCustomer({ ...selectedCustomer, Value: e ?? null, IsValid: true, ErrorMessage: '' });

                    if (e?.code && masterDataList?.DepartmentList?.length > 0) {
                        setComponentLoading({ ...componentLoading, contractLoading: true });
                        const contractListDropDown: IDropdownOption[] = masterDataList.DepartmentList
                            .filter(f => f.tenant && (f.tenant).trim() === (e.info).trim() && f.embee_crm_id && (f.embee_crm_id).trim() === (e.code).trim())
                            .map((item: any) => { return { code: item.departmentId, name: item.departmentName } });
                        console.log('filtered contractListDropDown: ', contractListDropDown);
                        setContractList(contractListDropDown);
                        await new Promise((resolve) => setTimeout(resolve, 2000));
                        setComponentLoading({ ...componentLoading, contractLoading: false });
                    }
                    else {
                        setContractList([]);
                    }
                    setSelectedContract({ ...selectedContract, Value: null, IsValid: true, ErrorMessage: '' });

                    break;
                }

            case DropdownType.ContractName:
                {
                    setSelectedContract({ ...selectedContract, Value: e ?? null, IsValid: true, ErrorMessage: '' });
                    break;
                }

            case DropdownType.Category:
                {
                    setSelectedCategory({ ...selectedCategory, Value: e ?? null, IsValid: true, ErrorMessage: '' });

                    if (e?.code && masterDataList?.SubCategoryList?.length > 0) {
                        setComponentLoading({ ...componentLoading, subCategoryLoading: true });
                        const subCategoryListDropDown: IDropdownOption[] = masterDataList.SubCategoryList.filter(f => f.categoryId === e.code).map((item: any) => { return { code: item.id, name: (item.subCategoryName + ' (' + item.subCategoryCode + ')') } });
                        setSubCategoryList(subCategoryListDropDown);
                        await new Promise((resolve) => setTimeout(resolve, 2000));
                        setComponentLoading({ ...componentLoading, subCategoryLoading: false });
                    }
                    else {
                        setSubCategoryList([]);
                    }
                    setSelectedSubCategory({ ...selectedSubCategory, Value: null, IsValid: true, ErrorMessage: '' });

                    break;
                }

            case DropdownType.SubCategory:
                {
                    setSelectedSubCategory({ ...selectedSubCategory, Value: e ?? null, IsValid: true, ErrorMessage: '' });
                    break;
                }

            case DropdownType.Region:
                {
                    setSelectedRegion({ ...selectedRegion, Value: e ?? null, IsValid: true, ErrorMessage: '' });
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

        // const MAX_DAY_DIFF = 30;

        switch (type) {
            case DateType.FromDate:
                {
                    setStartDate({ ...startDate, Value: e, IsValid: true, ErrorMessage: '' });
                    setEndDate({ ...endDate, Value: undefined, IsValid: true, ErrorMessage: '' });
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

    const handleTextChange = async (e: any, type: TextType) => {
        console.log("handleTextChange: ", e);

        switch (type) {
            case TextType.acc_ManagerName:
                {
                    setAccManagerName({ ...acc_ManagerName, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            case TextType.acc_ManagerEmail:
                {
                    setAccManagerEmail({ ...acc_ManagerEmail, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            case TextType.po_No:
                {
                    setPoNo({ ...poNo, Value: e, IsValid: true, ErrorMessage: '' });
                    break;
                }

            default:
                break;
        }
    };

    const handleDownload = async () => {
        try {
            // if (!await checkMandatoryFields()) {
            //     return;
            // }

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

    const handleSubmit = async () => {
        try {
            if (!await checkMandatoryFields()) {
                return;
            }
            setSubmitButton({ ...submitButton, IsLoading: true });

            const formData = new FormData();

            let formFileList: any[] = [];
            if (newFileList?.length) {
                newFileList.forEach(file => {
                    formFileList.push({
                        "id": 0,
                        "contractId": 0,
                        "internalName": file.internalName,
                        "active": true,
                        "createdByName": userDetails?.userName,
                        "createdByEmail": userDetails?.userEmail

                    });
                });
            }

            const formattedStartDate = dayjs(startDate.Value).format('YYYY-MM-DDT00:00:00');
            const formattedEndDate = dayjs(endDate.Value).format('YYYY-MM-DDT23:59:59');

            const jsonBody = {
                "transactionType": "I",
                "id": 0,
                "contractNo": null,
                "tenantId": selectedTenant.Value?.code ?? null,
                "tenantName": selectedTenant.Value?.name ?? null,
                "customerId": selectedCustomer.Value?.code ?? null,
                "customerName": selectedCustomer.Value?.name ?? null,
                "departmentId": selectedContract.Value?.code ?? null,
                "departmentName": selectedContract.Value?.name ?? null,
                "categoryId": selectedCategory.Value?.code ?? null,
                "categoryName": null,
                "subCategoryId": selectedSubCategory.Value?.code ?? null,
                "subCategoryName": null,
                "startDate": formattedStartDate,
                "endDate": formattedEndDate,
                "regionId": selectedRegion.Value?.code ?? null,
                "regionName": selectedRegion.Value?.name ?? null,
                "acc_ManagerName": acc_ManagerName.Value ?? null,
                "acc_ManagerEmail": acc_ManagerEmail.Value ?? null,
                "poNo": poNo.Value ?? null,
                "active": true,
                "createdByName": userDetails?.userName ?? null,
                "createdByEmail": userDetails?.userEmail ?? null,
                "fileList": formFileList
            }

            formData.append("eventData", JSON.stringify(jsonBody));

            if (newFileList?.length) {
                newFileList.forEach(file => {
                    formData.append(file.internalName, file.file);
                });
            }

            console.log("saveContractMstAPI-jsonBody: ", jsonBody);
            const response = await saveContractMstAPI(formData);
            console.log("saveContractMstAPI: ", response);

            if (response.data && response.data?.status == 1) {
                console.log("saveContractMstAPI data-response: ", response.data);
                handleShowAlert('Success', response.data?.message, true, '/listContract', pageHistory);
                // await new Promise((resolve) => setTimeout(resolve, 7000));
                // handleRedirect('List');
            }
            else if (response.data && response.data?.status == 0 && response.data?.id && response.data?.referenceNo) {
                handleShowAlert('Attention Required', response.data?.message, true);
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

            setSubmitButton({ ...submitButton, IsLoading: false });
        }
        catch (error) {
            console.error("Error at handleSubmit():", error);
            setSubmitButton({ ...submitButton, IsLoading: false });
            setAlertVisible({
                visible: true,
                message: CommonMessages.Error,
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
        if (userDetails) {
            getMasterData();
        }
    }, [userDetails]);

    useEffect(() => {
        if (
            selectedId &&
            masterDataList?.TenantList?.length > 0 &&
            masterDataList?.CustomerList?.length > 0 &&
            masterDataList?.DepartmentList?.length > 0
        ) {
            getContractMaster(selectedId);
        }

    }, [masterDataList, selectedId]);

    useEffect(() => {
        console.log('newFileList: ', newFileList);
    }, [newFileList]);

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

                <div className="d-flex align-items-stretch">
                    <div className="container-fluid p-0">

                        <Toast ref={toast} />

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

                                    <BlockUI className="transparent-blockui" blocked={submitButton.IsLoading}>

                                        <div className="title-bar">
                                            <div className="d-flex align-items-center">
                                                <Button className="back-btn" icon="pi pi-arrow-left" rounded text aria-label="Back"
                                                    onClick={() => pageHistory.goBack()}
                                                />
                                                <h5 className="ms-2 mb-0">New Contract</h5>
                                            </div>
                                        </div>

                                        <div className="page-content-wrapper">
                                            <div className="container-fluid mt-2">

                                                {
                                                    alertVisible.visible &&
                                                    (
                                                        <NorthStarAlert
                                                            message={alertVisible.message}
                                                            type={alertVisible.type}
                                                        />
                                                    )
                                                }

                                                {componentLoading.sectionLoading && <ContractMasterCreateSkeleton />}

                                                {
                                                    !componentLoading.sectionLoading &&
                                                    (
                                                        <div className="row px-1">

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Tenant" className="text-grey">Tenant<small className='required'>*</small></label>
                                                                    <Dropdown className="w-100" placeholder="Select Tenant"
                                                                        value={selectedTenant.Value}
                                                                        options={tenantList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.Tenant)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedTenant.IsValid}
                                                                    />
                                                                    {selectedTenant.ErrorMessage && !selectedTenant.IsValid && <small className='require'>{selectedTenant.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Customer" className="text-grey">Customer<small className='required'>*</small></label>
                                                                    <Dropdown className="w-100" placeholder="Select Customer"
                                                                        value={selectedCustomer.Value}
                                                                        options={customerList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.CustomerName)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        loading={componentLoading.customerLoading}
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedCustomer.IsValid}
                                                                    />
                                                                    {selectedCustomer.ErrorMessage && !selectedCustomer.IsValid && <small className='require'>{selectedCustomer.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Contract" className="text-grey">Contract<small className='required'>*</small></label>
                                                                    <Dropdown className="w-100" placeholder="Select Contract"
                                                                        value={selectedContract.Value}
                                                                        options={contractList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.ContractName)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        loading={componentLoading.contractLoading}
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedContract.IsValid}
                                                                    />
                                                                    {selectedContract.ErrorMessage && !selectedContract.IsValid && <small className='require'>{selectedContract.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Category" className="text-grey">Category<small className='required'>*</small></label>
                                                                    <Dropdown className="w-100" placeholder="Select Category"
                                                                        value={selectedCategory.Value}
                                                                        options={categoryList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.Category)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedCategory.IsValid}
                                                                    />
                                                                    {selectedCategory.ErrorMessage && !selectedCategory.IsValid && <small className='require'>{selectedCategory.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Sub-category" className="text-grey">Sub-category<small className='required'>*</small></label>
                                                                    <Dropdown className="w-100" placeholder="Select Sub-category"
                                                                        value={selectedSubCategory.Value}
                                                                        options={subCategoryList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.SubCategory)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        loading={componentLoading.subCategoryLoading}
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedSubCategory.IsValid}
                                                                    />
                                                                    {selectedSubCategory.ErrorMessage && !selectedSubCategory.IsValid && <small className='require'>{selectedSubCategory.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Start Date" className="text-grey">Start Date<small className='required'>*</small></label>
                                                                    <Calendar
                                                                        className="w-100"
                                                                        placeholder="Select a date"
                                                                        showIcon
                                                                        // minDate={dayjs().toDate()}
                                                                        // maxDate={selectedToDate ?? null}
                                                                        value={startDate.Value}
                                                                        dateFormat="dd/mm/yy"
                                                                        onChange={(e) => handleDateChange(e.value, DateType.FromDate)}
                                                                        invalid={!startDate.IsValid}
                                                                    />
                                                                    {startDate.ErrorMessage && !startDate.IsValid && <small className='require'>{startDate.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="End Date" className="text-grey">End Date<small className='required'>*</small></label>
                                                                    <Calendar
                                                                        className="w-100"
                                                                        placeholder="Select a date"
                                                                        showIcon
                                                                        minDate={(startDate.Value) ?? null}
                                                                        // maxDate={selectedToDate ?? null}
                                                                        value={endDate.Value}
                                                                        dateFormat="dd/mm/yy"
                                                                        onChange={(e) => handleDateChange(e.value, DateType.ToDate)}
                                                                        invalid={!endDate.IsValid}
                                                                        disabled={startDate.Value === null}
                                                                    />
                                                                    {endDate.ErrorMessage && !endDate.IsValid && <small className='require'>{endDate.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Region" className="text-grey">Region</label>
                                                                    <Dropdown className="w-100" placeholder="Select Region"
                                                                        value={selectedRegion.Value}
                                                                        options={regionList}
                                                                        onChange={(e) => handleDropdownChange(e.value, DropdownType.Region)}
                                                                        optionLabel="name"
                                                                        showClear
                                                                        emptyMessage={NO_DATA_AVAILABLE}
                                                                        filter
                                                                        filterBy="name"
                                                                        invalid={!selectedRegion.IsValid}
                                                                    />
                                                                    {selectedRegion.ErrorMessage && !selectedRegion.IsValid && <small className='require'>{selectedRegion.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Account Manager Name" className="text-grey">Account Manager Name
                                                                        {/* <small className='required'>*</small> */}
                                                                    </label>
                                                                    <InputText className="w-100" placeholder="Enter Name"
                                                                        value={acc_ManagerName.Value}
                                                                        onChange={(e) => handleTextChange(e.target.value, TextType.acc_ManagerName)}
                                                                        onKeyPress={handleTextKeyPress}
                                                                        maxLength={255}
                                                                    />
                                                                    {acc_ManagerName.ErrorMessage && !acc_ManagerName.IsValid && <small className='require'>{acc_ManagerName.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Division" className="text-grey">Account Manager Email
                                                                        {/* <small className='required'>*</small> */}
                                                                    </label>
                                                                    <InputText className="w-100" placeholder="Enter Email"
                                                                        value={acc_ManagerEmail.Value}
                                                                        onChange={(e) => handleTextChange(e.target.value, TextType.acc_ManagerEmail)}
                                                                        onBlur={validateEmail}
                                                                        onKeyPress={handleEmailKeyPress}
                                                                        maxLength={255}
                                                                    />
                                                                    {acc_ManagerEmail.ErrorMessage && !acc_ManagerEmail.IsValid && <small className='require'>{acc_ManagerEmail.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-3 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select PO NO." className="text-grey">PO No.</label>
                                                                    <InputText className="w-100" placeholder="Enter PO No."
                                                                        value={poNo.Value}
                                                                        onChange={(e) => handleTextChange(e.target.value, TextType.po_No)}
                                                                        onKeyPress={handleTextKeyPress}
                                                                        maxLength={255}
                                                                    />
                                                                    {poNo.ErrorMessage && !poNo.IsValid && <small className='require'>{poNo.ErrorMessage}</small>}
                                                                </div>
                                                            </div>

                                                            <div className="col-md-6 mb-3">
                                                                <div>
                                                                    <label htmlFor="Select Division" className="text-grey">Upload Supporting Document(s)</label>
                                                                    <div {...getRootProps({ className: 'dropzone' })}>
                                                                        <input {...getInputProps()} />
                                                                        <p>Drag 'n' drop some files here, or click to select files</p>
                                                                        <em>(Only *.pdf, *.xlsx, and *.doc files will be accepted)</em>
                                                                    </div>
                                                                    <aside>
                                                                        <ul className="uploaded-file">{files}</ul>
                                                                    </aside>
                                                                </div>
                                                            </div>

                                                            <div className="col-md-12">
                                                                <Button className="primary-fill px-4"
                                                                    loading={submitButton.IsLoading}
                                                                    label='Submit'
                                                                    // disabled={submitButton.IsDisabled}
                                                                    onClick={() => handleSubmit()}
                                                                />
                                                                <Button className="bordered ms-2 px-4" label="Cancel" onClick={() => handleRedirect('List')}
                                                                    disabled={submitButton.IsLoading}
                                                                    style={{ background: '#F2F2F2' }}
                                                                />
                                                            </div>

                                                        </div>
                                                    )
                                                }


                                            </div>
                                        </div>
                                    </BlockUI>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

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

export default ContractMasterCreateComponent;