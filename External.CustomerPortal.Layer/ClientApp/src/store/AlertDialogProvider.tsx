// import React, { createContext, useContext, useState } from 'react';
// import { Dialog } from 'primereact/dialog';
// import { Button } from 'primereact/button';

// interface AlertDialogContextType {
//     showDialog: (header: string, content: string, isClosable?: boolean) => void;
//     hideDialog: () => void;
// }

// const AlertDialogContext = createContext<AlertDialogContextType | null>(null);

// export const useAlertDialog = () => {
//     const context = useContext(AlertDialogContext);
//     if (!context) {
//         throw new Error('useAlertDialog must be used within an AlertDialogProvider');
//     }
//     return context;
// };

// const AlertDialogProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
//     const [visible, setVisible] = useState(false);
//     const [headerMessage, setHeaderMessage] = useState('');
//     const [contentMessage, setContentMessage] = useState('');
//     const [isClosable, setIsClosable] = useState(true);

//     const showDialog = (header: string, content: string, isClosable: boolean = true) => {
//         setHeaderMessage(header);
//         setContentMessage(content);
//         setIsClosable(isClosable);
//         setVisible(true);
//     };

//     const hideDialog = () => {
//         if (isClosable) {
//             setVisible(false);
//         }
//     };

//     return (
//         <AlertDialogContext.Provider value={{ showDialog, hideDialog }}>
//             {children}
//             <Dialog
//                 header={
//                     <div style={{
//                         color: headerMessage.includes("Attention") ? "red" : "inherit"
//                     }}>
//                         {headerMessage}
//                     </div>
//                 }
//                 // footer={<Button label="OK" onClick={hideDialog} />}
//                 visible={visible}
//                 style={{ width: '60vw' }}
//                 draggable={false}
//                 blockScroll={true}
//                 onHide={hideDialog}
//                 closable={isClosable}
//             >
//                 <div>{contentMessage}</div>
//             </Dialog>
//         </AlertDialogContext.Provider>
//     );
// };

// export default AlertDialogProvider;



import React, { createContext, useContext, useState } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import 'primereact/resources/themes/saga-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import "../../src/App.css";

interface AlertDialogContextType {
    showDialog: (header: string, content: string, isClosable?: boolean) => void;
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

    const showDialog = (header: string, content: string, isClosable: boolean = true) => {
        setHeaderMessage(header);
        setContentMessage(content);
        setIsClosable(isClosable);
        setVisible(true);
    };

    const hideDialog = () => {
        if (isClosable) {
            setVisible(false);
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
                        className="p-button-primary py-2 px-2"
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

    const dynamicWidth = contentMessage?.length > 80 ? '60vw' : '50vw';
    const headerColor = getHeaderColor(headerMessage);

    return (
        <AlertDialogContext.Provider value={{ showDialog, hideDialog }}>
            {children}
            <Dialog
                header={
                    <div style={{ color: getHeaderColor(headerMessage), fontSize: '1.25rem', fontWeight: 600, display: 'flex', alignItems: 'center' }}>
                        <i className={`${getHeaderIcon(headerMessage)} animated-icon`} style={{ marginRight: '8px', borderRadius: '50%', border: `1px solid ${headerColor}` }} />
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
                breakpoints={{ '767px': '90vw' }}
            >
                <div style={{ fontSize: '1rem', lineHeight: '1.5', color: '#333' }}>{contentMessage}</div>
            </Dialog>
        </AlertDialogContext.Provider>
    );
};

export default AlertDialogProvider;