import dayjs from 'dayjs';

//#region Regex

export const EMAIL_REGEX: RegExp = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

//#endregion

// Constants

export const HEADER_VALUE = '40025788865f4c24aa8f24c11207deed';

export const HTTP_CODES = Object.freeze({
    OK: 200,
    BAD_REQUEST: 400
});

export const NO_DATA_AVAILABLE = "No data available";

export const MONTH_LIST = [
    { key: 1, header: 'January' },
    { key: 2, header: 'February' },
    { key: 3, header: 'March' },
    { key: 4, header: 'April' },
    { key: 5, header: 'May' },
    { key: 6, header: 'June' },
    { key: 7, header: 'July' },
    { key: 8, header: 'August' },
    { key: 9, header: 'September' },
    { key: 10, header: 'October' },
    { key: 11, header: 'November' },
    { key: 12, header: 'December' }
];

const CURRENT_YEAR = dayjs().year();
export const PAST_YEAR_LIST = Array.from({ length: 10 }, (_, index) => CURRENT_YEAR - index).map(year => ({
    key: year,
    header: year.toString(),
}));
