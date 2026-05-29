import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';

interface DataTableSkeletonProps {
    columns: string[];
    rows: number;
}

const DataTableSkeleton: React.FC<DataTableSkeletonProps> = ({ columns, rows }) => {
    const skeletonRows = Array.from({ length: rows }).map((_, index) => ({
        id: index,
    }));

    const skeletonBodyTemplate = () => {
        return <Skeleton width="100%" height="1.5rem" />;
    };

    return (
        <DataTable
            emptyMessage={<></>}
            value={skeletonRows}>
            {columns.map((col, index) => (
                <Column
                    key={index}
                    field={col}
                    header={col}
                    body={skeletonBodyTemplate}
                />
            ))}
        </DataTable>
    );
};

export default DataTableSkeleton;