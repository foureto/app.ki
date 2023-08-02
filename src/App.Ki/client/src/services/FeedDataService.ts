import { AService } from "./_AService";
import { DataResponse } from "./models/DataResponse";

export interface AppSymbol {
  base: string;
  quoted: string;
  apiSymbol: string;
  exchange: string;
}

export interface Ticker {
  symbol: AppSymbol;
  bid: number;
  ask: number;
  last: number;
  stamp: string;
  move?: number
}

export interface SymbolFilter {
  exchange?: string;
}

export interface TickerFilter extends SymbolFilter {
  currency?: string;
}

export class FeedDataService extends AService {
  protected static BASE_URI = "feed";

  public static symbols(
    props: SymbolFilter
  ): Promise<DataResponse<AppSymbol[]>> {
    return this.get("symbols", props);
  }

  public static tickers(props: TickerFilter): Promise<DataResponse<Ticker[]>> {
    return this.get("tickers", props);
  }
}
