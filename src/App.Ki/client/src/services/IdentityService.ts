import { AService } from "./_AService.ts";
import { ApiResponse, DataResponse } from "./models/DataResponse";

export interface User {
  id: string;
  name: string;
}

export interface LoginRequest {
  login: string;
  password: string;
}

export class IdentityService extends AService {
  protected static BASE_URI = "identity";

  public static me(): Promise<DataResponse<User>> {
    return this.get("me");
  }

  public static login(props: LoginRequest): Promise<ApiResponse> {
    return this.post("login", props);
  }
}
