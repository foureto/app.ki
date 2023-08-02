import React from "react";
import { Table } from "antd";

export interface GridDataProps<T> {
  data: T[];
  name: string;
  total?: number;
  pageIndex?: number;
  pageSize?: number;
  columns: any;
  onChange?: any;
  sortChanged?: any;
  rowKey?: string;
  selectColumn?: any;
  isLoading: boolean;
  rowClassName?: string;
}

const transformSorter = (toTransform: any) => {
  return {
    field: toTransform.field,
    order: toTransform.order === "ascend" ? 1 : 2,
  };
};

const GridData = <T extends object>(props: GridDataProps<T>) => {
  const {
    data,
    total,
    pageIndex,
    pageSize,
    onChange,
    rowKey,
    selectColumn,
    columns,
    isLoading = false,
    rowClassName,
  } = props;

  return (
    <Table
      size="small"
      loading={isLoading}
      bordered
      rowKey={rowKey ?? "id"}
      columns={selectColumn ? [...columns, selectColumn] : columns}
      dataSource={data}
      rowClassName={rowClassName}
      pagination={
        onChange
          ? {
              size: "small",
              total: total,
              current: pageIndex,
              pageSize: pageSize,
              defaultPageSize: 20,
              pageSizeOptions: ["10", "20", "50", "100"],
              showSizeChanger: true,
              showTotal: (total) => `Total: ${total}`,
            }
          : false
      }
      onChange={(pagination, _, sorter: any) => {
        onChange({
          pageIndex: pagination.current,
          pageSize: pagination.pageSize,
          sort: sorter.order ? [transformSorter(sorter)] : [],
        });
      }}
    />
  );
};

export default GridData;
