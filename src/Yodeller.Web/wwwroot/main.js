const ComponentBuilder = {
  create: function (tag, classNames, children) {
    const element = document.createElement(tag);

    if (classNames != null && classNames.length > 0) {
      element.classList.add(...classNames);
    }

    if (children != null) {
      children.forEach((child) => element.appendChild(child));
    }
    return element;
  },
  createIcon: function (iconName, isHuge) {
    const classes = isHuge ? ["huge-icon"] : [];
    const icon = this.create("ion-icon", classes, []);
    icon.setAttribute("name", iconName);
    return this.create("span", ["icon"], [icon]);
  },
};

const TimezoneModule = {
  toUtc: function (localHHMM) {
    const pieces = localHHMM.split(":");
    const timezoneOffset = new Date().getTimezoneOffset() / 60;
    return `${this._wrapHour(parseInt(pieces[0]) + timezoneOffset)}:${this._pad(
      pieces[1]
    )}`;
  },
  toLocal: function (utcHHMM) {
    const pieces = utcHHMM.split(":");
    const timezoneOffset = new Date().getTimezoneOffset() / 60;
    return `${this._wrapHour(parseInt(pieces[0]) - timezoneOffset)}:${this._pad(
      pieces[1]
    )}`;
  },
  _pad: function (num) {
    return String(num).padStart(2, "0");
  },
  _wrapHour: function (hour) {
    if (hour < 0) return hour + 24;
    else if (hour >= 24) return hour - 24;
    else return hour;
  },
};

const EnvironmentDetailsModule = {
  initialise: function () {
    fetch("/environment", { method: "GET" })
      .then((response) => response.json())
      .then((dto) => this._process(dto));
  },
  _process: function (environmentDto) {
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
  _updateTag: function (tagId, className) {
    document.getElementById(tagId).classList.add(className);
  },
};

const RequestTableModule = {
  updateTable: function () {
    const tableBody = document.querySelector("#downloadsViewTable tbody");

    fetch("/requests")
      .then((response) => response.json())
      .then((dtos) => {
        dtos = Array.from(dtos).filter((dto) => dto.status !== "Forgotten");

        const sortMap = {
          New: 1,
          Completed: 2,
          Failed: 0,
          Cancelled: 3,
          "In progress": -1,
        };
        dtos.sort((a, b) => sortMap[a.status] - sortMap[b.status]);
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
        const title = "Failed to update requests";
        console.error(`${title}: ${err}`);
        renderNewAlert(`${err}`, "is-danger", title);
      });
  },
  _createFallbackRow: function () {
    const message = ComponentBuilder.create("span", [], []);
    message.innerHTML = "<em>No registered downloads.</em>";
    const cell = ComponentBuilder.create("td", [], [message]);
    cell.setAttribute("colspan", 5);
    const row = ComponentBuilder.create("tr", ["no-results-row"], [cell]);
    return row;
  },
  _createRowElement: function (dto) {
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
  _createTextCell: function (text) {
    const cell = document.createElement("td");
    cell.textContent = text;
    return cell;
  },
  _createStatusCell: function (text) {
    const classMap = {
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
  _createActionsCell: function (requestId, mediaLocator, canCancel, history) {
    const commonClasses = ["button"];
    const buttons = [];

    if (history != null && history.length > 0) {
      const entries = history.map(
        (entry) =>
          `[${new Date(entry.utc_time).toLocaleString()}] ${entry.description}`
      );
      const button = ComponentBuilder.create(
        "button",
        [...commonClasses, "is-black"],
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

function cancelDownload(requestId, mediaLocator) {
  const url = "/requests/" + encodeURIComponent(requestId);
  fetch(url, { method: "DELETE" })
    .then((response) => {
      RequestTableModule.updateTable();
    })
    .catch((err) => {
      console.error(`Failed to cancel a "${mediaLocator}" download: ${err}`);
      console.warn(`TODO Alert: ${err}`);
    });
}

function validateForm() {
  const form = document.forms["requestDownloadForm"];

  const url = form["url"].value;
  const pass = url !== "";

  const paragraph = document.querySelector(
    "#requestDownloadForm .validation-paragraph"
  );
  paragraph.style.display = pass ? "none" : null;

  if (!pass) {
    paragraph.querySelector(".validation-message").textContent =
      "Enter media ID.";
  }

  return pass;
}

function postMediaRequest(mediaType) {
  if (!validateForm()) return false;

  const requestBody = {
    mediaLocator: document.forms["requestDownloadForm"]["url"].value,
    audioOnly: mediaType === "audio",
  };

  fetch("/requests", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(requestBody),
  })
    .then(() => setTimeout(() => RequestTableModule.updateTable(), 300))
    .catch((err) => console.error("failed to submit a video request: " + err));

  return false;
}

function main() {
  EnvironmentDetailsModule.initialise();

  RequestTableModule.updateTable();
  setInterval(() => {
    RequestTableModule.updateTable();
  }, 60000);
}

main();
