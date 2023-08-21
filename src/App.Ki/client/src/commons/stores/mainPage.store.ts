import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";
import {
  FeedDataService,
  Ticker,
  TickerFilter,
} from "@services/FeedDataService";

const tickersRequested = createEvent<TickerFilter>();
const tickerReceived = createEvent<Ticker>();

const getTickersFx = createEffect((props: TickerFilter) =>
  FeedDataService.tickers(props)
);

const $loading = createStore<boolean>(false)
  .on(getTickersFx.pending, (_, p) => p)
  .reset(tickersRequested);

const $tickers = createStore<Ticker[]>([])
  .on(getTickersFx.doneData, (_, p) => p.data ?? [])
  .on(tickerReceived, (s, p) => {
    const old = s.find(
      (e) =>
        e.symbol.base === p.symbol.base &&
        e.symbol.quoted === p.symbol.quoted &&
        e.symbol.exchange === p.symbol.exchange
    );

    if (!old) return s;
    const move = p.last - old.last;

    s.splice(s.indexOf(old), 1, { ...p, ...{ move } });
    return [...s];
  })
  .reset(tickersRequested);

const $mainPage = combine({
  tickers: $tickers,
  loading: $loading,
});

sample({ clock: tickersRequested, target: getTickersFx });

export { $mainPage, tickersRequested, tickerReceived };
