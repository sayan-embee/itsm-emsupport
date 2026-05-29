import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';

interface ReportSectionSkeletonProps {

}

const ContractMasterCreateSkeleton: React.FC<ReportSectionSkeletonProps> = () => {

    return (
        // <div className="p-4 px-1">
        //     <div className="row mb-3">
        //         <div className="col-md-12">
        //             <div className="d-flex justify-content-between align-items-center">
        //                 <Skeleton width="100%" height="2rem" />
        //             </div>
        //         </div>
        //     </div>
        //     <div className="row mb-3">
        //         <div className="col-md-12">
        //             <div className="d-flex justify-content-between align-items-center">
        //                 <Skeleton width="100%" height="2rem" />
        //             </div>
        //         </div>
        //     </div>
        //     <div className="row mb-3">
        //         <div className="col-md-12">
        //             <div className="d-flex justify-content-between align-items-center">
        //                 <Skeleton width="100%" height="2rem" />
        //             </div>
        //         </div>
        //     </div>
        //     <div className="row mb-3">
        //         <div className="col-md-12">
        //             <div className="d-flex justify-content-between align-items-center">
        //                 <Skeleton width="100%" height="4rem" />
        //             </div>
        //         </div>
        //     </div>
        //     <div className="row mb-2">
        //         <div className="col-md-3 m-0">
        //             <Skeleton className='m-0' width="100%" height="2rem" />
        //         </div>
        //     </div>
        // </div>

        <div className="row px-1">

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Tenant" className="text-grey">Tenant<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Customer" className="text-grey">Customer<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Contract" className="text-grey">Contract<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Category" className="text-grey">Category<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Sub-category" className="text-grey">Sub-category<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Start Date" className="text-grey">Start Date<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="End Date" className="text-grey">End Date<small className='required'>*</small></label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Region" className="text-grey">Region</label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Account Manager Name" className="text-grey">Account Manager Name
                        {/* <small className='required'>*</small> */}
                    </label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select Division" className="text-grey">Account Manager Email
                        {/* <small className='required'>*</small> */}
                    </label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-3 mb-3">
                <div>
                    <label htmlFor="Select PO NO." className="text-grey">PO No.</label>
                    <Skeleton width="100%" height="2rem" />
                </div>
            </div>

            <div className="col-md-6 mb-3">
                <div>
                    <label htmlFor="Select Division" className="text-grey">Upload Supporting Document(s)</label>
                    <Skeleton width="100%" height="4rem" />
                </div>
            </div>

            <div className="col-md-12">
                <Button className="primary-fill px-4" label='Submit' disabled />
                <Button className="bordered ms-2 px-4" label="Cancel" disabled />
            </div>

        </div>
    );
};

export default ContractMasterCreateSkeleton;