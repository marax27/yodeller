import "./main.css";
import "./bulma.min.css";

const ComponentBuilder = {
  create: function (tag: any, classNames: any, children: any) {
    const element = document.createElement(tag);

    if (classNames != null && classNames.length > 0) {
      element.classList.add(...classNames);
    }

    if (children != null) {
      children.forEach((child: any) => element.appendChild(child));
    }
    return element;
  },
  createIcon: function (iconName: any, isHuge: any) {
    const classes = isHuge ? ["huge-icon"] : [];
    const icon = this.create("ion-icon", classes, []);
    icon.setAttribute("name", iconName);
    return this.create("span", ["icon"], [icon]);
  },
};

const NotificationsModule = {
  success: function (message: any, title: any) {
    this._renderNew(message, "is-success", title);
  },
  warning: function (message: any, title: any) {
    this._renderNew(message, "is-warning", title);
  },
  error: function (message: any, title: any) {
    this._renderNew(message, "is-danger", title);
  },
  _renderNew: function (message: any, className: any, title: any) {
    const notifElement = this._create(message, className, title);
    const formCard = document.getElementById("formCard");
    formCard.parentNode.insertBefore(notifElement, formCard.nextSibling);
  },
  _create(message: any, className: any, title: any) {
    const notifTitle = ComponentBuilder.create("p", [], []);
    notifTitle.textContent = title;

    const deleteButton = ComponentBuilder.create("button", ["delete"], []);
    deleteButton.setAttribute("aria-label", "delete");

    const notifHeader = ComponentBuilder.create(
      "div",
      ["message-header"],
      [notifTitle, deleteButton]
    );

    const notifBody = ComponentBuilder.create("div", ["message-body"], []);
    notifBody.textContent = message;

    const notifElement = ComponentBuilder.create(
      "div",
      ["message", "block", className],
      [notifHeader, notifBody]
    );

    deleteButton.onclick = () => (notifElement.style.display = "none");

    return notifElement;
  },
};

const EnvironmentDetailsModule = {
  initialise: function () {
    fetch("/environment", { method: "GET" })
      .then((response) => response.json())
      .then((dto) => this._process(dto));
  },
  _process: function (environmentDto: any) {
    const freeSpace = environmentDto.availableDiskSpacePercentage;
    this._updateTag(
      "diskSpaceStatusTag",
      freeSpace > 25 ? "is-success" : freeSpace > 5 ? "is-warning" : "is-danger"
    );

    this._updateTag(
      "downloaderStatusTag",
      environmentDto.downloaderAvailable ? "is-success" : "is-danger"
    );
    this._updateTag(
      "postProcessingStatusTag",
      environmentDto.postProcessingAvailable ? "is-success" : "is-warning"
    );
  },
  _updateTag: function (tagId: any, className: any) {
    document.getElementById(tagId).classList.add(className);
  },
};

const TableOverlayModule = {
  enable: function () {
    this._overlay().classList.add("is-loading");
    this._startTime = new Date();
  },
  disable: function () {
    const overlayDurationMs = <any>new Date() - this._startTime;
    const overlay = this._overlay();

    if (overlayDurationMs > 250) {
      overlay.classList.remove("is-loading");
    } else {
      setTimeout(
        () => overlay.classList.remove("is-loading"),
        250 - overlayDurationMs
      );
    }
  },
  _overlay: function () {
    return document.querySelector("#refreshOverlay");
  },
  _startTime: null as any,
};

interface GetRequestDto {
  id: string;
  mediaLocator: string;
  audioOnly: boolean;
  history: any[];
  status: string;
}

const RequestTableModule = {
  updateTable: function () {
    const tableBody = document.querySelector("#downloadsViewTable tbody");

    TableOverlayModule.enable();

    fetch("/requests")
      .then((response) => response.json())
      .then((dtos: GetRequestDto[]) => {
        dtos = Array.from(dtos).filter(
          (dto: any) => dto.status !== "Forgotten"
        );

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
        NotificationsModule.error(`${err}`, title);
      })
      .finally(() => TableOverlayModule.disable());
  },
  _createFallbackRow: function () {
    const message = ComponentBuilder.create("span", [], []);
    message.innerHTML = "<em>No registered downloads.</em>";
    const cell = ComponentBuilder.create("td", [], [message]);
    cell.setAttribute("colspan", 5);
    const row = ComponentBuilder.create("tr", ["no-results-row"], [cell]);
    return row;
  },
  _createRowElement: function (dto: GetRequestDto) {
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
  },
  _createTextCell: function (text: string) {
    const cell = document.createElement("td");
    cell.textContent = text;
    return cell;
  },
  _createStatusCell: function (text: string) {
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
  },
  _createActionsCell: function (
    requestId: string,
    mediaLocator: string,
    canCancel: boolean,
    history: any[]
  ) {
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
      button.onclick = () => cancelDownload(requestId, mediaLocator);
      buttons.push(button);
    }

    const container = ComponentBuilder.create(
      "div",
      ["buttons", "is-flex-wrap-nowrap"],
      buttons
    );
    return ComponentBuilder.create("td", [], [container]);
  },
};

function cancelDownload(requestId: string, mediaLocator: string) {
  const url = "/requests/" + encodeURIComponent(requestId);
  fetch(url, { method: "DELETE" })
    .then((response) => {
      setTimeout(() => {
        RequestTableModule.updateTable();
      }, 300);
    })
    .catch((err) => {
      NotificationsModule.warning(`${err}`, "Failed to send a cancel request");
    });
}

function validateForm() {
  const form = document.forms["requestDownloadForm" as any];

  const url = form["url"].value;
  const pass = url !== "";

  const paragraph: HTMLElement = document.querySelector(
    "#requestDownloadForm .validation-paragraph"
  );
  paragraph.style.display = pass ? "none" : null;

  if (!pass) {
    paragraph.querySelector(".validation-message").textContent =
      "Enter media locator.";
  }

  return pass;
}

function postMediaRequest(mediaType: string) {
  if (!validateForm()) return false;

  const mediaLocatorFormField =
    document.forms["requestDownloadForm" as any]["url"];

  const subtitlesFormField =
    document.forms["requestDownloadForm" as any]["subtitles"];

  const requestBody = {
    mediaLocator: mediaLocatorFormField.value,
    audioOnly: mediaType === "audio",
    subtitlePatterns: (subtitlesFormField.value || "").split(","),
  };

  fetch("/requests", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(requestBody),
  })
    .then(() => {
      mediaLocatorFormField.value = "";
      subtitlesFormField.value = "";
      NotificationsModule.success(
        `Requested download: ${requestBody.mediaLocator}`,
        "Requested a download"
      );
      setTimeout(() => RequestTableModule.updateTable(), 300);
    })
    .catch((err) => {
      NotificationsModule.error(
        `Failed to request a download: ${err}`,
        "Failed to send a request"
      );
    });

  return false;
}

function clearFinishedRequests() {
  fetch("/requests/clear-finished", { method: "POST" })
    .then(() => {
      setTimeout(() => RequestTableModule.updateTable(), 300);
    })
    .catch((err) => {
      NotificationsModule.error(
        `Failed to clear finished requests: ${err}`,
        "Failed to send a request"
      );
    });
}

// This replaces in-HTML function calls such as the "onclick" attribute.
function bindPageComponentsToActions() {
  const byId = (id: string) => document.getElementById(id);

  byId("submitVideoButton").onclick = () => postMediaRequest("video");
  byId("submitAudioButton").onclick = () => postMediaRequest("audio");
  byId("refreshTableButton").onclick = () => RequestTableModule.updateTable();
  byId("clearFinishedButton").onclick = () => clearFinishedRequests();
}

function main() {
  bindPageComponentsToActions();
  EnvironmentDetailsModule.initialise();

  RequestTableModule.updateTable();
  setInterval(() => {
    RequestTableModule.updateTable();
  }, 60000);
}

main();
