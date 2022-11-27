import { ComponentBuilder } from "./component-builder";
import { NotificationsHub } from "./notifications-hub";
import { TableOverlay } from "./table-overlay";

interface GetRequestDto {
  id: string;
  mediaLocator: string;
  audioOnly: boolean;
  history: any[];
  status: string;
}

export class RequestTableModule {
  private readonly _tableOverlay = new TableOverlay();

  private readonly _notificationsHub: NotificationsHub;

  constructor(notificationsHub: NotificationsHub) {
    this._notificationsHub = notificationsHub;
  }

  public updateTable(): void {
    const tableBody = document.querySelector("#downloadsViewTable tbody");

    this._tableOverlay.enable();

    fetch("/requests")
      .then((response) => response.json())
      .then((dtos: GetRequestDto[]) => {
        const sortMap: { [status: string]: number } = {
          New: 1,
          Completed: 2,
          Failed: 0,
          Cancelled: 3,
          "In progress": -1,
        };

        dtos.sort(
          (a: GetRequestDto, b: GetRequestDto) =>
            sortMap[a.status] - sortMap[b.status]
        );
        const newRows = dtos.map((dto) => this._createRowElement(dto));

        if (newRows.length === 0) {
          newRows.push(this._createFallbackRow());
        }

        tableBody.replaceChildren(...newRows);
      })
      .then(() => {
        const spinner = document.querySelector(".refresh-indicator");
        const newSpinner = spinner.cloneNode(true);
        spinner.parentNode.replaceChild(newSpinner, spinner);
      })
      .catch((err) => {
        const title = "Failed to update the table";
        this._notificationsHub.error(`${err}`, title);
      })
      .finally(() => this._tableOverlay.disable());
  }

  private _createFallbackRow(): HTMLElement {
    const message = ComponentBuilder.create("span", [], []);
    message.innerHTML = "<em>No registered downloads.</em>";
    const cell = ComponentBuilder.create("td", [], [message]);
    cell.setAttribute("colspan", "5");
    const row = ComponentBuilder.create("tr", ["no-results-row"], [cell]);
    return row;
  }

  private _createRowElement(dto: GetRequestDto): HTMLElement {
    const cells = [
      this._createTextCell(dto.mediaLocator),
      this._createStatusCell(dto.status),
      this._createTextCell(dto.audioOnly ? "Audio" : "Video"),
      this._createActionsCell(
        dto.id,
        dto.mediaLocator,
        dto.status === "New",
        dto.history
      ),
    ];
    const rowClass = dto.status.toLowerCase().replace(" ", "-") + "-entry";

    return ComponentBuilder.create("tr", [rowClass], cells);
  }

  private _createTextCell(text: string): HTMLElement {
    const cell = document.createElement("td");
    cell.textContent = text;
    return cell;
  }

  private _createStatusCell(text: string): HTMLElement {
    const classMap: { [status: string]: string } = {
      "In progress": "is-info",
      New: "is-dark",
      Failed: "is-danger",
      Completed: "is-success",
      Cancelled: "is-light",
    };
    const classes = classMap[text] ? ["tag", classMap[text]] : ["tag"];

    const tag = ComponentBuilder.create("span", classes, []);
    tag.textContent = text;
    tag.style.textTransform = "uppercase";
    return ComponentBuilder.create("td", [], [tag]);
  }

  private _createActionsCell(
    requestId: string,
    mediaLocator: string,
    canCancel: boolean,
    history: any[]
  ): HTMLElement {
    const commonClasses = ["button"];
    const buttons = [];

    if (history != null && history.length > 0) {
      const entries = history.map(
        (entry) =>
          `[${new Date(entry.dateTime).toLocaleString()}] ${entry.description}`
      );
      const button = ComponentBuilder.create(
        "button",
        [...commonClasses, "is-black", "has-help-text"],
        [ComponentBuilder.createIcon("information-circle-outline", true)]
      );
      button.title = entries.join("\n");
      buttons.push(button);
    }

    if (canCancel) {
      const button = ComponentBuilder.create(
        "button",
        [...commonClasses, "is-danger"],
        [ComponentBuilder.createIcon("trash", true)]
      );
      button.title = `Cancel "${mediaLocator}" download.`;
      button.onclick = () => this._cancelDownload(requestId, mediaLocator);
      buttons.push(button);
    }

    const container = ComponentBuilder.create(
      "div",
      ["buttons", "is-flex-wrap-nowrap"],
      buttons
    );
    return ComponentBuilder.create("td", [], [container]);
  }

  private _cancelDownload(requestId: string, mediaLocator: string): void {
    const url = "/requests/" + encodeURIComponent(requestId);
    fetch(url, { method: "DELETE" })
      .then((response) => {
        setTimeout(() => {
          this.updateTable();
        }, 300);
      })
      .catch((err) => {
        this._notificationsHub.warning(
          `${err}`,
          "Failed to send a cancel request"
        );
      });
  }
}
