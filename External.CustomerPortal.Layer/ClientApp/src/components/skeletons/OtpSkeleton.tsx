import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import logo from '../../assets/Logo.svg';
const OtpSkeleton: React.FC = () => {
    return (
        <div className='auth-wrapper'>
            <div className="auth-bg">
                <div className="container">

                    <div className="auth-form-wrapper">
                        <img className='brand' src={logo} alt="Embee" />
                        <h3>Enter OTP</h3>
                        {/* <p>to access ITSM Automation Portal</p> */}
                        <div className='mt-4 mb-4'>
                            <p className='m-1'>Please enter OTP sent on your email ID</p>
                        </div>
                        <div className="mb-3 autocpmplete">
                            <Skeleton width="275px" height="38px" />
                        </div>
                        <div className="mb-3 autocpmplete">
                            <Skeleton width="250px" height="40px" />
                        </div>
                        <div className="d-flex flex-column flex-md-row gap-3 justify-content-between align-items-center mb-4">
                            <Skeleton width="100%" height="40px" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default OtpSkeleton;