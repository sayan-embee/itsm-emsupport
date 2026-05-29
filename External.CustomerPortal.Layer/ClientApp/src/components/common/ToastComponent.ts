import { Toast, ToastMessage } from 'primereact/toast';
import * as React from 'react';

const toast = React.createRef<Toast>();
export { toast };

const TOAST_DURATION = 7000; // 7 seconds
const TOAST_FONTSIZE = '14px';

export enum CommonMessage {
    NoData = 'No data available',
    MandatoryFields = 'Please fill all the required fields',
    Error = 'There was an error processing your request.',
    Refresh = 'Please refresh the page and try again',
    SessionExpired = 'Your session is no longer valid. Please log in again.',
    ServerNotResponding = 'Server is not responding. Please check your internet connection.',
    InternalServerError = 'Internal server error. Please try again later.',
    Unauthorized = 'You are not authorized to access this resource.',
}

export enum toastMessageType {
    Success = 'Success Message',
    Error = 'Error Message',
    Info = 'Note',
    Warn = 'Warning Message',
    Note = 'Please Note'
}

export function showSuccessToast(toast: React.RefObject<Toast>, summary: string, detail: string) {
    toast.current?.show({ severity: 'success', summary, detail, life: 7000, contentStyle: { fontSize: TOAST_FONTSIZE } });
}

export function showInfoToast(toast: React.RefObject<Toast>, summary: string, detail: string) {
    toast.current?.show({ severity: 'info', summary, detail, life: TOAST_DURATION, contentStyle: { fontSize: TOAST_FONTSIZE } });
}

export function showWarningToast(toast: React.RefObject<Toast>, summary: string, detail: string) {
    toast.current?.show({ severity: 'warn', summary, detail, life: TOAST_DURATION, contentStyle: { fontSize: TOAST_FONTSIZE } });
}

export function showErrorToast(toast: React.RefObject<Toast>, summary: string, detail: string) {
    toast.current?.show({ severity: 'error', summary, detail, life: TOAST_DURATION, contentStyle: { fontSize: TOAST_FONTSIZE } });
}