export interface ApiResponse {
  success: boolean;
  message: string;
  statusCode: number;
}

export interface DataResponse<T> extends ApiResponse {
  data: T;
  page?: number;
  count?: number;
  total?: number;
}

export interface Filter {
  field: string;
  value?: string | number | number[] | string[];
}

export interface Sort {
  field: string;
  order: number;
}

export interface PagedRequest {
  pageIndex: number;
  pageSize: number;
  filter?: Filter[];
  listSort?: Sort[];
}

export interface ApiPageRequest {
  pageIndex: number;
  pageSize: number;
  listSort?: Sort[];
}

export interface ApiFilteredRequest<T> extends ApiPageRequest {
  filter?: T;
}
