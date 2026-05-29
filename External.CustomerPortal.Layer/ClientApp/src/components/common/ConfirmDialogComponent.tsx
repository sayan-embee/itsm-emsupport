// src/utils/confirmDialog.ts
import { confirmDialog } from 'primereact/confirmdialog';

interface ConfirmDialogParams {
    header?: string;
    message?: string;
    icon?: string;
}

let isDialogOpen = false; // Global flag

export const showConfirmDialog = ({ header, message, icon }: ConfirmDialogParams): Promise<boolean> => {

    if (isDialogOpen) return Promise.resolve(false); // Prevent duplicate dialogs
    isDialogOpen = true; // Mark as open

    const getHeaderColor = (header: string) => {
        if (header.includes("Error") || header.includes("Attention")) {
            return "#d13438";
        } else if (header.includes("Success")) {
            return "#107c10";
        } else {
            return "#10446f";
        }
    };

    return new Promise<boolean>((resolve) => {
        confirmDialog({
            className: 'custom-confirm-dialog',
            message: message || 'Are you sure you want to proceed?',
            header: (
                <div style={{ color: getHeaderColor(header || 'Confirmation') }}>
                    {header || 'Confirmation'}
                </div>
            ),
            icon: icon || 'pi pi-exclamation-triangle',
            accept: () => {
                isDialogOpen = false;
                resolve(true);
            },
            reject: () => {
                isDialogOpen = false;
                resolve(false);
            },
            draggable: false,
            onHide: () => {
                isDialogOpen = false;
                resolve(false);
            },
            style: { width: '30vw' },
            breakpoints: {
                '767px': '90vw'
            }
        });
    });
};