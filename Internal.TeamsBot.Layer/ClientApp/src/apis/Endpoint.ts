// const BASE_URL = window.location.origin + '/WebApi/api/';
// const BASE_URL = 'https://gkj31kgt-3978.inc1.devtunnels.ms/api/';

const isLocal = process.env.REACT_APP_BASE_URL?.includes('devtunnels.ms');

const BASE_URL = isLocal
    ? process.env.REACT_APP_BASE_URL
    : `${window.location.origin}${process.env.REACT_APP_BASE_URL}`;

export const endpointList = {
    getUserAccess: 'getUserAccess',
    getDepartmentMst: 'getDepartmentMst',
    getReportSectionMst: 'getReportSectionMst?active=',
    getReportInPPT: 'GetReports',
     getReportOnmobileInPPT: 'GetReportsForOnMobile',
    getReportInExcel: 'GetReportInExcel',
    downloadFile: 'DownloadFile?filePath=',

    getMasterData: 'getMasterData',

    saveContractMaster: 'contractMaster/save',
    getContractMaster: 'contractMaster/get',

    directLineToken: 'directLine/token'
}

export const getUrl = (key: any) => {
    return BASE_URL + key;
}