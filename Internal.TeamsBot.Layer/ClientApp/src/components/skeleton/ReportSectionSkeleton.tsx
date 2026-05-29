import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Card } from 'primereact/card';

interface ReportSectionSkeletonProps {

}

const ReportSectionSkeleton: React.FC<ReportSectionSkeletonProps> = () => {

    return (
        <div>
            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
                <div className="col-md-4 mb-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <Skeleton width="100%" height="2rem" />
                    </div>
                </div>
            </div>

        </div>
    );
};

export default ReportSectionSkeleton;