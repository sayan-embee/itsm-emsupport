import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { Button } from 'primereact/button';

// Images
import logo from '../../assets/Logo.svg';
import avater from '../../assets/user_blank.svg';
import office from '../../assets/corporate.svg';
import email from '../../assets/email-icon.svg';
import phone from '../../assets/phone.svg';

const SidePanelSkeleton: React.FC = () => {
    return (
        <div>
            {/* <div className="avater">
                <img src={avater} alt="" />
            </div> */}
            <h6><Skeleton width="200px" height="20px" /></h6>
            <div className="mb-2 d-flex align-items-center">
                <img src={office} alt="" />
                <span className="text-xs ms-2 mb-0"><Skeleton width="150px" height="20px" /></span>
            </div>
            <div className="mb-2 d-flex align-items-center">
                <img src={email} alt="" />
                <span className="text-xs ms-2 mb-0"><Skeleton width="150px" height="20px" /></span>
            </div>
            <div className="mb-2 d-flex align-items-center">
                <img src={phone} alt="" />
                <span className="text-xs ms-2 mb-0"><Skeleton width="150px" height="20px" /></span>
            </div>
            {/* <Button className='signout px-4 mt-3' label="Sign out" disabled /> */}
        </div>
    );
};

export default SidePanelSkeleton;