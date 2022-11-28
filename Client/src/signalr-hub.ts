import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { GetRequestDto } from "./request-table-module";

export interface SignalRMessageHandlers {
  onRequestUpdate: (updatedRequests: GetRequestDto[]) => void;
}

export class SignalRHub {
  private _connection?: HubConnection;

  public async initialise(handlers: SignalRMessageHandlers): Promise<void> {
    this._connection = this._createConnection();

    this._subscribeToEvents(handlers);

    await this._connection
      .start()
      .then(() => {
        console.log("SignalR Connected!");
      })
      .catch((err) => {
        console.log(err);
      });
  }

  private _createConnection(): HubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl("/real-time")
      .configureLogging(LogLevel.Information)
      .build();
    return connection;
  }

  private _subscribeToEvents(handlers: SignalRMessageHandlers): void {
    this._connection.on("RequestUpdate", handlers.onRequestUpdate);
  }
}
