import React from "react";
import { useStore } from "effector-react";
import { Table } from "antd";
import {
  $mainPage,
  tickersRequested,
  tickerReceived,
} from "@stores/mainPage.store";
import { Ticker } from "@services/FeedDataService";
import { useWebSocket } from "@services/helpers/socket.hook";
import "./mainPage.scss";

const Colorable: React.FC<{ children: React.ReactNode; move?: number }> = ({
  move,
  children,
}) => {
  return (
    <div className={!move ? "" : move < 0 ? "cell-down" : "cell-up"}>
      {children}
    </div>
  );
};

const columns = [
  {
    title: "Base",
    render: (row: Ticker) => (
      <Colorable move={row.move}>{row.symbol.base}</Colorable>
    ),
    sorter: (a: Ticker, b: Ticker) => (a.symbol.base > b.symbol.base ? -1 : 1),
    width: 80,
  },
  {
    title: "Quoted",
    render: (row: Ticker) => (
      <Colorable move={row.move}>{row.symbol.quoted}</Colorable>
    ),
    sorter: (a: Ticker, b: Ticker) =>
      a.symbol.quoted > b.symbol.quoted ? -1 : 1,
    width: 80,
  },
  {
    title: "Exchange",
    render: (row: Ticker) => (
      <Colorable move={row.move}>{row.symbol.exchange}</Colorable>
    ),
    sorter: (a: Ticker, b: Ticker) =>
      a.symbol.exchange > b.symbol.exchange ? -1 : 1,
  },
  {
    title: "Bid",
    render: (row: Ticker) => <Colorable move={row.move}>{row.bid}</Colorable>,
    sorter: (a: Ticker, b: Ticker) => a.bid - b.bid,
    width: 120,
  },
  {
    title: "Ask",
    render: (row: Ticker) => <Colorable move={row.move}>{row.ask}</Colorable>,
    sorter: (a: Ticker, b: Ticker) => a.ask - b.ask,
    width: 120,
  },
  {
    title: "Stamp",
    render: (row: Ticker) => <Colorable move={row.move}>{row.stamp}</Colorable>,
  },
];

const MainPage: React.FC = () => {
  const { loading, tickers } = useStore($mainPage);
  const { subscribe, sendMessage, status } = useWebSocket("feed");

  React.useEffect(() => {
    tickersRequested({});
    if (status === 0) {
      sendMessage("SubscribeTickers", {});
      subscribe<Ticker>("Ticker", tickerReceived);
    }
  }, [subscribe, sendMessage, status]);

  return (
    <div>
      <Table
        size="small"
        columns={columns}
        loading={loading}
        dataSource={tickers}
        pagination={{ position: ["topCenter"] }}
        rowClassName={"table-row-light"}
      />
    </div>
  );
};

export default MainPage;
