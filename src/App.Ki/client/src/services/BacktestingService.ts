import { AService } from "./_AService";
import { ApiFilteredRequest, DataResponse } from "./models/DataResponse";

export interface BacktestFilter {
  running?: boolean;
  name?: string;
}

export interface Backtest {
  id: string;
  name: string;
  description: string;
  created: string;
  updated: string;
  started?: string;
  finished?: string;
}

export class BacktestingService extends AService {
  protected static BASE_URI = "backtests";

  public static getTests(
    props: ApiFilteredRequest<BacktestFilter>
  ): Promise<DataResponse<Backtest[]>> {
    return this.post("page", props);
  }
}
