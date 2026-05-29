import React, { useState } from 'react';
import { Message } from 'primereact/message';
import { Sidebar } from 'primereact/sidebar';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';

import dayjs from "dayjs";

// Images
import logo from '../../assets/Logo.svg';
// import itsmlogo from '../../assets/itsm-logo.svg';
import itsmlogo from '../../assets/em-support-logo.png';

interface HeaderComponentProps {
    headerTitle: string;
    sidebarIsVisible: boolean;
    setSidebarIsVisible: (visible: boolean) => void;
}

const HeaderComponent: React.FC<HeaderComponentProps> = ({
    headerTitle,
    sidebarIsVisible,
    setSidebarIsVisible,
}) => {
    return (
        <header>
            <div className="container-fluid px-md-5 px-4">
                <div className="d-flex justify-content-between align-items-center">

                    {/* <div className="d-flex align-items-center">
                        <div className="itsm-logo">
                            <img src={itsmlogo} alt="EIS" />
                        </div>
                        <Tooltip target=".tooltip-target" />
                        <h4 className="ms-4 mb-0 text-blue tooltip-target"
                            style={{ cursor: 'pointer' }}
                            data-pr-tooltip="Embee Intelligent Support"
                            data-pr-position="bottom"
                        >
                            EIS
                        </h4>
                        <h4 className="ms-1 mb-0 text-blue">
                            Automation Portal
                        </h4>
                    </div> */}

                    <div className="d-flex align-items-center">
                        <div className="itsm-logo">
                            <img src={itsmlogo} alt="AI-EmSupport" />
                        </div>
                    </div>

                    <div className="brand-logo">
                        <img src={logo} alt="Embee" />
                    </div>
                    <div className="mobile-menu">
                        <Button icon="pi pi-bars" rounded text aria-label="Menu" onClick={() => setSidebarIsVisible(true)} />
                    </div>
                </div>
            </div>
        </header >
    );
};

export default HeaderComponent;