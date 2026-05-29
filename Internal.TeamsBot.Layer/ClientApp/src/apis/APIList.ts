import axios from 'axios';
import { getUrl, endpointList } from './Endpoint'
import { IAxiosResponse } from '../components/Interfaces';
import { HEADER_VALUE } from '../components/Constants';

//#region AUTH API

export const getUserAccessAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getUserAccess));
    try {
        const response: IAxiosResponse = await axios.post(getUrl(endpointList.getUserAccess), data, { headers });
        return response;
    }
    catch (error) {
        console.error('Error at getUserAccessAPI():', error);
        return null;
    }
};

//#endregion


//#region GET API

export const getReportFileAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.downloadFile));
    try {
        const response: IAxiosResponse = await axios.get(getUrl(endpointList.downloadFile) + data, {
            headers,
            responseType: 'blob', // Ensures the response is treated as binary data
        });
        return response;
    }
    catch (error) {
        console.error('Error at getReportFileAPI():', error);
        return null;
    }
};

export const getReportSectionMstAPI = async (data?: any, departmentId?: number) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportSectionMst)+ data+'&departmentId='+departmentId);
    try {
        const response: IAxiosResponse = await axios.get(getUrl(endpointList.getReportSectionMst) + data+'&departmentId='+departmentId, {
            headers
        });
        return response;
    }
    catch (error) {
        console.error('Error at getReportSectionMstAPI():', error);
        return null;
    }
};

export const getMasterDataAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getMasterData));
    try {
        const response: IAxiosResponse = await axios.get(getUrl(endpointList.getMasterData), {
            headers
        });
        return response;
    }
    catch (error) {
        console.error('Error at getMasterDataAPI():', error);
        throw error;
    }
};

//#endregion


//#region POST API

export const getDepartmentMstAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getDepartmentMst));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.getDepartmentMst), data, { headers });
    return response;
};

export const getReportAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportInPPT));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.getReportInPPT), data, { headers });
    return response;
}

export const getReportOnmobileAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportOnmobileInPPT));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.getReportOnmobileInPPT), data, { headers });
    return response;
}

export const getReportInExcelAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportInExcel));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.getReportInExcel), data, { headers });
    return response;
}

export const getContractMstAPI = async (data?: any) => {
    try {
        const headers = {
            'api_key': HEADER_VALUE
        };
        console.log('In api', getUrl(endpointList.getContractMaster));
        const response: IAxiosResponse = await axios.post(getUrl(endpointList.getContractMaster), data, { headers });
        return response;
    }
    catch (error) {
        console.error('Error at getContractMstAPI():', error);
        throw error;
    }
};

export const saveContractMstAPI = async (data?: any) => {
    try {
        const headers = {
            'api_key': HEADER_VALUE
        };
        console.log('In api', getUrl(endpointList.saveContractMaster));
        const response: IAxiosResponse = await axios.post(getUrl(endpointList.saveContractMaster), data, { headers });
        return response;
    }
    catch (error) {
        console.error('Error at saveContractMstAPI():', error);
        throw error;
    }
};


//#endregion


//#region Bot-API

export const getDirectlineTokenAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.directLineToken));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.directLineToken), data, { headers });
    return response;
}

//#endregion