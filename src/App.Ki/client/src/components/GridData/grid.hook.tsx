/* eslint-disable @typescript-eslint/no-explicit-any */
import { Table } from "antd";
import { Store, Event } from "effector";
import { useStore } from "effector-react";
import { createGrid } from "./grid.store";
import Spinner from "@components/Spinner";
import {
  DataResponse,
  Filter,
  PagedRequest,
  Sort,
} from "@services/models/DataResponse";

const stores: {
  [key: string]: {
    gridStore: Store<{
      loading: boolean;
      gridData: any[];
      pageIndex: number;
      pageSize: number;
      total: number;
      columns: any[];
      hiddenColumns: string[];
      filter: Filter[];
      sort: Sort[];
    }>;
    paginationChanged: Event<PagedRequest>;
    tableRendered: Event<Filter | Filter[] | undefined>;
    tableUnmounted: Event<void>;
    filtersChanged: Event<Filter[]>;
    fetchData: Event<void>;
  };
} = {};

const parseSorter = (toTransform: any) => {
  return {
    field: toTransform.field,
    order: toTransform.order === "ascend" ? 1 : 2,
  };
};

export function useGrid<T>(
  name: string,
  call: (arg: PagedRequest) => Promise<DataResponse<T[]>>,
  cols: any[],
  selectColumn?: any,
  afterAction?: Event<void>,
  rowKey = "id"
) {
  if (!stores[name]) {
    stores[name] = createGrid<T>(name, call, cols, undefined, afterAction);
  }

  const {
    gridStore,
    paginationChanged,
    fetchData,
    filtersChanged,
    tableRendered,
    tableUnmounted,
  } = stores[name];

  const { loading, columns, gridData, total, pageIndex, pageSize } =
    useStore(gridStore);

  const onChange = (args: any) => {
    if (paginationChanged) paginationChanged(args);
  };

  const table = (
    <Table
      size="small"
      loading={{ spinning: loading, indicator: <Spinner /> }}
      bordered
      rowKey={rowKey ?? "id"}
      columns={selectColumn ? [...columns, selectColumn] : columns}
      dataSource={gridData}
      pagination={{
        size: "small",
        total: total,
        current: pageIndex,
        pageSize: pageSize,
        defaultPageSize: 10,
        pageSizeOptions: ["10", "20", "50", "100"],
        showSizeChanger: true,
        showTotal: (total) => `Total: ${total}`,
      }}
      onChange={(pagination, _, sorter: any) => {
        onChange({
          pageIndex: pagination.current,
          pageSize: pagination.pageSize,
          sort: sorter.order ? [parseSorter(sorter)] : [],
        });
      }}
    />
  );

  return {
    table,
    fetchData,
    filtersChanged,
    tableRendered,
    tableUnmounted,
  };
}
