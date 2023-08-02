import { useEffect, useRef, useCallback } from "react";
import { createEvent, createStore } from "effector";
import { useStore } from "effector-react";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  IStreamSubscriber,
} from "@microsoft/signalr";

export enum WsStatus {
  open,
  closed,
  error,
}

const serverAddress = "/ws";

const open = createEvent<string>("open");
const closed = createEvent<Error>("closed");
const error = createEvent("error");

const wsStatus = createStore<WsStatus>(WsStatus.closed)
  .on(open, () => WsStatus.open)
  .on(closed, () => WsStatus.closed)
  .on(error, () => WsStatus.error);

wsStatus.watch((state) => console.log("ws", state));

export function useWebSocket(
  uri: string,
  onConnected?: () => void,
  onError?: (e: Error) => void
) {
  const status = useStore(wsStatus);
  const connectionRef = useRef<HubConnection>();

  const handleOpen = () => {
    open(connectionRef.current?.connectionId ?? "");
    if (onConnected) onConnected();
  };

  const handleError = (err: Error) => {
    error();
    if (onError) onError(err);
  };

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${serverAddress}/${uri}`)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;
    connectionRef.current
      .start()
      .then(() => {
        handleOpen();
        connectionRef.current?.onclose(closed);
        connectionRef.current?.onreconnected((c) => {
          console.log("reconnected", c);
          open(c ?? "");
        });
      })
      .catch((e) => {
        handleError(e);
      });

    return () => {
      console.log("stopped");
      connectionRef.current?.stop();
    };
  }, [uri]);

  const subscribe = useCallback(
    <T>(endpoint: string, handler: (msg: T) => void) => {
      if (connectionRef.current?.state === HubConnectionState.Connected) {
        connectionRef.current.on(endpoint, handler);
      }
    },
    [connectionRef]
  );

  // const subscribe = useCallback(
  //   <T>(endpoint: string, handler: (msg: T) => void, ...args: any[]) => {
  //     if (connectionRef.current?.state === HubConnectionState.Connected) {
  //       console.log("subscription request");
  //       connectionRef.current.stream(endpoint, args).subscribe({
  //         next: (item: T) => {
  //           console.log(item);
  //           handler(item);
  //         },
  //         complete: () => console.log("Feed complete"),
  //         error: (e: Error) => console.log(e),
  //       } as IStreamSubscriber<T>);
  //     }
  //   },
  //   [connectionRef]
  // );

  const unSubscribe = useCallback(
    (endpoint: string) => {
      if (connectionRef.current?.state === HubConnectionState.Connected)
        connectionRef.current.off(endpoint);
    },
    [connectionRef]
  );

  const sendMessage = useCallback(
    <T>(sendMethod: string, message?: T) => {
      if (connectionRef.current?.state !== HubConnectionState.Connected) {
        console.log("not connected");
        return;
      }

      connectionRef.current?.send(sendMethod, message);
    },
    [connectionRef]
  );
  return { status, sendMessage, subscribe, unSubscribe };
}
