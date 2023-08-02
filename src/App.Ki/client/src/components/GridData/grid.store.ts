/* eslint-disable @typescript-eslint/no-explicit-any */
import {
  DataResponse,
  Filter,
  PagedRequest,
  Sort,
} from "@services/models/DataResponse";
import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
  Event,
} from "effector";
import { debounce } from "patronum";

const defaultPageIndex = 1;
const defaultPageSize = 10;

export const createGrid = <T>(
  name: string,
  handlerCall: (arg: PagedRequest) => Promise<DataResponse<T[]>>,
  columns: any[],
  hiddenColumns?: string[],
  afterAction?: Event<void>
) => {
  const tableUnmounted = createEvent();
  const tableRendered = createEvent<Filter | Filter[] | undefined>();

  const fetchData = createEvent();
  const filterChanged = createEvent<Filter[]>();
  const filtersChanged = createEvent<Filter[]>();
  const oneFilterChanged = createEvent<Filter[]>();
  const oneFilterChangedDebounced = debounce({
    source: oneFilterChanged,
    timeout: 1000,
  });

  const paginationChanged = createEvent<PagedRequest>();

  const sortChanged = createEvent<{ listSort: Sort[] }>();
  const dirtySortChanged = createEvent<string>();

  const handler = createEffect((props: PagedRequest) => handlerCall(props));

  const $data = createStore<T[]>([])
    .on(handler.doneData, (_, p) => (p.data ?? []) as T[])
    .reset(tableUnmounted);

  const $pageIndex = createStore<number>(defaultPageIndex)
    .on(handler.doneData, (_, p) => p.page)
    .reset(tableUnmounted);

  const $pageSize = createStore<number>(defaultPageSize)
    .on(handler.doneData, (_, p) => p.count)
    .reset(tableUnmounted);

  const $total = createStore<number>(0)
    .on(handler.doneData, (_, p) => p.total)
    .reset(tableUnmounted);

  const $columns = createStore<any[]>(columns);
  const $hiddenColumns = createStore<string[]>(hiddenColumns ?? []);
  const $staticFilter = createStore<Filter[]>([]).on(
    tableRendered,
    (_, payload) => {
      if (payload !== undefined) {
        if (Array.isArray(payload)) return payload;
        return [payload];
      }
      return [];
    }
  );

  const $filter = createStore<Filter[]>([])
    .on(oneFilterChangedDebounced, (prev, state) => {
      const prevFilters = prev.filter(
        (f) => state.find((s) => s.field === f.field) === undefined
      );
      const stateFilters = state.filter((f) => f.value !== "");
      return [...prevFilters, ...stateFilters];
    })
    .on(filterChanged, (prev, state) => {
      const prevFilters = prev.filter(
        (f) => state.find((s) => s.field === f.field) === undefined
      );
      const stateFilters = state.filter((f) => f.value !== "");
      return [...prevFilters, ...stateFilters];
    });

  const $sort = createStore<Sort[]>([])
    .on(sortChanged, (_, p) => p.listSort)
    .on(dirtySortChanged, (prev, state) => {
      if (state) {
        const sorts = state.split(";");
        return sorts.map((val) => {
          const srt = val.split("_");
          return {
            field: srt[0],
            order: Number(srt[1]),
          };
        });
      }
      return prev;
    });

  const $loading = handler.pending;
  const $isTableRendered = createStore<boolean>(false)
    .on(handler.pending, (_, p) => !p)
    .reset(tableUnmounted);

  sample({
    clock: tableRendered,
    source: {
      pageIndex: $pageIndex,
      pageSize: $pageSize,
      filter: $filter,
      staticFilter: $staticFilter,
      listSort: $sort,
      isTableRendered: $isTableRendered,
    },
    filter: ({ isTableRendered }) => !isTableRendered,
    fn({ pageIndex, pageSize, filter, listSort, staticFilter }) {
      return {
        pageIndex,
        pageSize,
        filter: [...staticFilter, ...filter],
        listSort,
      };
    },
    target: handler,
  });

  sample({
    clock: fetchData,
    source: {
      pageIndex: $pageIndex,
      pageSize: $pageSize,
      filter: $filter,
      staticFilter: $staticFilter,
      listSort: $sort,
    },
    fn({ pageIndex, pageSize, filter, listSort, staticFilter }) {
      return {
        pageIndex,
        pageSize,
        filter: [...staticFilter, ...filter],
        listSort,
      };
    },
    target: handler,
  });

  sample({
    clock: oneFilterChangedDebounced,
    source: {
      pageSize: $pageSize,
      listSort: $sort,
      oldFilter: $filter,
      staticFilter: $staticFilter,
    },
    fn({ pageSize, listSort, oldFilter, staticFilter }, payload) {
      const oldFilters = oldFilter.filter(
        (f) => payload.find((s) => s.field === f.field) === undefined
      );
      const payloadFilters = payload.filter((f) => f.value !== "");
      return {
        pageIndex: 1,
        pageSize,
        listSort,
        filter: [...oldFilters, ...staticFilter, ...payloadFilters],
      };
    },
    target: [handler],
  });

  sample({
    clock: paginationChanged,
    source: {
      filter: $filter,
      staticFilter: $staticFilter,
    },
    fn({ filter, staticFilter }, payload) {
      return {
        pageIndex: payload.pageIndex,
        pageSize: payload.pageSize,
        filter: [...filter, ...staticFilter],
        listSort: payload.listSort,
      };
    },
    target: [handler, sortChanged],
  });

  if (afterAction) {
    sample({ clock: afterAction, target: fetchData });
  }

  const $gridStore = combine({
    loading: $loading,
    gridData: $data,
    pageIndex: $pageIndex,
    pageSize: $pageSize,
    total: $total,
    columns: $columns,
    hiddenColumns: $hiddenColumns,
    filter: $filter,
    sort: $sort,
  });

  return {
    gridStore: $gridStore,
    paginationChanged,
    tableRendered,
    tableUnmounted,
    filtersChanged,
    fetchData,
  };
};
