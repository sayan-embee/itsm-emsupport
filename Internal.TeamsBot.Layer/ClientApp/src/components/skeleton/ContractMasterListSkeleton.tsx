import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Card } from 'primereact/card';

import contractIcon from '../../assets/contract.svg';

interface ReportSectionSkeletonProps {

}

const ContractMasterListSkeleton: React.FC<ReportSectionSkeletonProps> = () => {

    return (
        <div>
            {/* <Skeleton width="100%" height="250px" className='mb-2' /> */}
            <Skeleton width="100%" height="2rem" className='mb-1' />
            <Skeleton width="100%" height="2rem" className='mb-1' />
            <Skeleton width="100%" height="2rem" />
        </div>
    );
};

export default ContractMasterListSkeleton;