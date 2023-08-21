import { AService } from "./_AService";
import {
  ApiFilteredRequest,
  ApiResponse,
  DataResponse,
} from "./models/DataResponse";

export interface StrategiesFilter {
  running?: boolean;
  type?: number;
}

export interface Strategy {
  id: string;
  name: string;
  description: string;
  created: string;
  udpated: string;
}

export class StrategiesService extends AService {
  protected static BASE_URI = "strategies";

  public static getStrategies(
    props: ApiFilteredRequest<StrategiesFilter>
  ): Promise<DataResponse<Strategy[]>> {
    return this.post("page", props);
  }

  public static getStrategy(id: string): Promise<DataResponse<Strategy>> {
    return this.get(id);
  }

  public static addStrategy(props: Strategy): Promise<ApiResponse> {
    return this.post("", props);
  }

  public static updateStrategy(props: Strategy): Promise<ApiResponse> {
    return this.put("", props);
  }

  public static deleteStrategy(id: string): Promise<ApiResponse> {
    return this.delete(id);
  }
}
