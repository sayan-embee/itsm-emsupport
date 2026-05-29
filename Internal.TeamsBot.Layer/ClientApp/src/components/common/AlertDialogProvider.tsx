import React, { createContext, useContext, useState } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import 'primereact/resources/themes/saga-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import "./../../App.scss";
import { useHistory } from 'react-router-dom';

interface AlertDialogContextType {
    showDialog: (header: string, content: string, isClosable?: boolean, redirect?: string, history?: any) => void;
    hideDialog: () => void;
}

const AlertDialogContext = createContext<AlertDialogContextType | null>(null);

export const useAlertDialog = () => {
    const context = useContext(AlertDialogContext);
    if (!context) {
        throw new Error('useAlertDialog must be used within an AlertDialogProvider');
    }
    return context;
};

const AlertDialogProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {

    const [visible, setVisible] = useState(false);
    const [headerMessage, setHeaderMessage] = useState('');
    const [contentMessage, setContentMessage] = useState('');
    const [isClosable, setIsClosable] = useState(true);
    const [redirect, setRedirect] = useState('');
    const [history, setHistory] = useState<ReturnType<typeof useHistory> | undefined>(undefined);

    const showDialog = (header: string, content: string, isClosable: boolean = true, redirect = '', history = undefined) => {
        setHeaderMessage(header);
        setContentMessage(content);
        setIsClosable(isClosable);
        setRedirect(redirect);
        setHistory(history);
        setVisible(true);
    };

    const hideDialog = () => {
        if (isClosable) {
            setVisible(false);
            if (redirect && history) {
                history.push(`${redirect}`);
            }
        }
    };

    // Determine header color based on the header message
    const getHeaderColor = (header: string) => {
        if (header.includes("Error") || header.includes("Attention")) {
            return "#d13438";
        } else if (header.includes("Success")) {
            return "#107c10";
        } else {
            return "#0078d4";
        }
    };

    const getHeaderIcon = (header: string) => {
        if (header.includes("Error") || header.includes("Attention")) {
            return "pi pi-exclamation-triangle"; // Error icon
        } else if (header.includes("Success")) {
            return "pi pi-check-circle"; // Success icon
        } else {
            return "pi pi-info-circle"; // Info icon
        }
    };

    // Custom footer for the dialog
    const renderFooter = () => {
        return (
            <div>
                {isClosable && (
                    <Button
                        label="OK"
                        onClick={hideDialog}
                        className="p-button-primary"
                    />
                )}
                {/* {isClosable && (
                    <Button
                        label="Cancel"
                        onClick={hideDialog}
                        className="p-button-secondary"
                    />
                )} */}
            </div>
        );
    };

    const dynamicWidth = contentMessage?.length > 45 ? '60vw' : '40vw';

    return (
        <AlertDialogContext.Provider value={{ showDialog, hideDialog }}>
            {children}
            <Dialog
                header={
                    <div style={{ color: getHeaderColor(headerMessage), fontSize: '1.25rem', fontWeight: 600 }}>
                        <i className={`${getHeaderIcon(headerMessage)} animated-icon`} style={{ marginRight: '8px', borderRadius: '50%' }} />
                        {headerMessage}
                    </div>
                }
                footer={renderFooter()}
                visible={visible}
                style={{ width: dynamicWidth, borderRadius: '4px' }}
                draggable={false}
                blockScroll={true}
                onHide={hideDialog}
                // closable={isClosable}
                className="teams-dialog"
                resizable={false}
            >
                <div style={{ fontSize: '1rem', lineHeight: '1.5', color: '#333' }}>{contentMessage}</div>
            </Dialog>
        </AlertDialogContext.Provider>
    );
};

export default AlertDialogProvider;