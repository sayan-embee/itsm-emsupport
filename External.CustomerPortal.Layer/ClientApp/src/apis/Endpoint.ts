// const BASE_URL = window.location.origin + '/WebApi/api/';
// const BASE_URL = 'https://gkj31kgt-3978.inc1.devtunnels.ms/api/';

const isLocal = process.env.REACT_APP_BASE_URL?.includes('devtunnels.ms') || process.env.REACT_APP_BASE_URL?.includes('localhost');
if (process.env.NODE_ENV === 'development') {
    console.log('Env: ', process.env.REACT_APP_BASE_URL);
}

const BASE_URL = isLocal
    ? process.env.REACT_APP_BASE_URL
    : `${window.location.origin}${process.env.REACT_APP_BASE_URL}`;

export const endpointList = {
    directLineToken: 'directLineToken',

    getUserAccess: 'getUserAccess',
    getDepartmentMst: 'getDepartmentMst',
    getReportSectionMst: 'getReportSectionMst?active=',
    getReportInPPT: 'GetReports',
    getReportInExcel: 'GetReportInExcel',
    downloadFile: 'DownloadFile?filePath=',

    logout: 'logout',
    getNewOTP: 'newOTP',
    verifyOTP: 'verifyOTP',
    getCustomerDetails: 'getCustomerDetails',
    getTicketDetails: 'getTicketDetails',
    getCustomerWiseMasterData: 'getMasterData',
}

export const getUrl = (key: any) => {
    return BASE_URL + key;
}