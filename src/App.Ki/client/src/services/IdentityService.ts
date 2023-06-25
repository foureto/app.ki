import { AService } from "./_AService.ts";
import { ApiResponse, DataResponse } from "./models/DataResponse";

export interface User {
  id: string;
  name: string;
}

export interface LoginCodeRequest {
  code: string;
}

export class IdentityService extends AService {
  protected static BASE_URI = "identity";

  public static me(): Promise<DataResponse<User>> {
    return this.get("me");
  }

  public static login(): Promise<ApiResponse> {
    return this.post("login");
  }

  public static loginCode(props: LoginCodeRequest): Promise<ApiResponse> {
    return this.post("login/code", props);
  }
}
