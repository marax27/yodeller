import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

export class SignalRHub {
  private _connection?: HubConnection;

  public async initialise(): Promise<void> {
    this._connection = this._createConnection();

    this._subscribeToEvents();

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

  private _subscribeToEvents(): void {
    this._connection.on("Receive", (user: unknown, message: unknown) => {
      // TODO.
    });
  }
}
