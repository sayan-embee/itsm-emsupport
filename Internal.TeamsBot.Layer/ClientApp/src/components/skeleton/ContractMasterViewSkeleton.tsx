import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Card } from 'primereact/card';

import contractIcon from '../../assets/contract.svg';

interface ReportSectionSkeletonProps {

}

const ContractMasterViewSkeleton: React.FC<ReportSectionSkeletonProps> = () => {

    return (
        <>
            <div className="row mb-3">
                <div className="col-md-6 d-flex align-items-center">
                    {/* <img src={contractIcon} alt="" className="me-2" loading="lazy" /> */}
                    <Skeleton width="100%" className="ms-4" height="2rem" />
                </div>
            </div>

            <div className="row mb-2 ps-2 ms-1">

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Status</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Tenant</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Department Id</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Customer Name</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Contact Person Name</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Contact Person Email</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Contact Person Mobile</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Region</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Account Manager Name</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Account Manager Email</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Start Date</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">End Date</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Category</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">Sub-Category</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                <div className="col-md-4 mb-3">
                    <p className="text-grey m-0">PO No.</p>
                    <p className="m-0 fw-semibold"><Skeleton width="100%" height="2rem" /></p>
                </div>

                {/* <div className="col-md-6 mb-3">
                    <p className="text-grey mb-1">Attachment(s)</p>
                    <ul className="uploaded-file">
                        <Skeleton width="100%" height="2rem" />
                    </ul>
                </div> */}

            </div>
        </>
    );
};

export default ContractMasterViewSkeleton;