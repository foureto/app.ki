import React from "react";
import { useWebSocket } from "@services/helpers/socket.hook";

const CryptoFeed: React.FC = () => {
  const { status, subscribe, unSubscribe } = useWebSocket(
    "feed",
    () => console.log("connected"),
    (e) => console.log(e)
  );

  React.useEffect(() => {
    if (status === 0) {
      subscribe("tickers", (e) => {
        console.log(e);
      });
    }
  }, [status, subscribe]);

  return <>feed </>;
};

export default CryptoFeed;
