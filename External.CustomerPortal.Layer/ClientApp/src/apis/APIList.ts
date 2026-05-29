import axios from 'axios';

import { getUrl, endpointList } from './Endpoint'
import { HEADER_VALUE } from '../Constants';
import { IAxiosResponse } from '../Interfaces';

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

export const getReportSectionMstAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportSectionMst));
    try {
        const response: IAxiosResponse = await axios.get(getUrl(endpointList.getReportSectionMst) + data, {
            headers
        });
        return response;
    }
    catch (error) {
        console.error('Error at getReportSectionMstAPI():', error);
        return null;
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

export const getReportInExcelAPI = async (data?: any) => {
    const headers = {
        'api_key': HEADER_VALUE
    };
    console.log('In api', getUrl(endpointList.getReportInExcel));
    const response: IAxiosResponse = await axios.post(getUrl(endpointList.getReportInExcel), data, { headers });
    return response;
}


export const logoutAPI = async (data?: any) => {
    try {
        console.log('In api', getUrl(endpointList.logout));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.logout),
            data,
            {
                withCredentials: true // Ensures the cookie is sent with the request
            }
        );
        return response;
    }
    catch (error) {
        console.error('Error at logoutAPI():', error); // Explicit error handling
        return null;
    }
};

export const getNewOTPAPI = async (data?: any) => {
    try {
        const headers = {
            'api_key': HEADER_VALUE
        };
        console.log('In api', getUrl(endpointList.getNewOTP));
        const response: IAxiosResponse = await axios.post(getUrl(endpointList.getNewOTP), data, { headers });
        return response;
    }
    catch (error) {
        throw error;
    }
}

export const verifyOTPAPI = async (data?: any) => {
    try {
        const headers = {
            'api_key': HEADER_VALUE
        };
        console.log('In api', getUrl(endpointList.verifyOTP));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.verifyOTP),
            data,
            {
                headers,
                withCredentials: true // Ensures cookies are stored
            }
        );
        return response;
    }
    catch (error) {
        throw error;
    }
};

export const GetCustomerDetailsAPI = async (data?: any) => {
    try {
        console.log('In api', getUrl(endpointList.getCustomerDetails));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.getCustomerDetails),
            data,
            {
                withCredentials: true // Ensures the cookie is sent with the request
            }
        );
        return response;
    }
    catch (error) {
        throw error;
    }
};

export const getCustomerWiseMasterDataAPI = async (data?: any) => {
    try {
        console.log('In api', getUrl(endpointList.getCustomerWiseMasterData));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.getCustomerWiseMasterData),
            data,
            {
                withCredentials: true // Ensures the cookie is sent with the request
            }
        );
        return response;
    }
    catch (error) {
        throw error;
    }
};

export const getTicketDetailsAPI = async (data?: any) => {
    try {
        console.log('In api', getUrl(endpointList.getTicketDetails));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.getTicketDetails),
            data,
            {
                withCredentials: true // Ensures the cookie is sent with the request
            }
        );
        return response;
    }
    catch (error) {
        throw error;
    }
};

//#endregion


//#region BOT-Helper API

export const getDirectlineTokenAPI = async (data?: any) => {
    try {
        console.log('In api', getUrl(endpointList.directLineToken));

        const response: IAxiosResponse = await axios.post(
            getUrl(endpointList.directLineToken),
            data,
            {
                withCredentials: true // Ensures the cookie is sent with the request
            }
        );
        return response;
    }
    catch (error) {
        throw error;
    }
}

//#endregion

//#region Ticket Conversations - Freshservice API
export const getTicketConversationsAPI = async (ticketId: number) => {
    try {
        const username = "4fOaien4ba3RTtwQY4E0"; // API key
        const password = "x"; // Freshservice requires 'x' as password

        const response: IAxiosResponse = await axios.get(
            `https://embee.freshservice.com/api/v2/tickets/${ticketId}/conversations`,
            {
                auth: {
                    username,
                    password
                }
            }
        );
        return response;
    }
    catch (error) {
        console.error("Error at getTicketConversationsAPI():", error);
        throw error;
    }
};
//#endregion