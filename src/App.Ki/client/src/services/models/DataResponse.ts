export interface ApiResponse {
  success: boolean;
  message: string;
  statusCode: number;
}

export interface DataResponse<T> extends ApiResponse {
  data: T;
}
