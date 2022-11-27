import "./main.css";
import "./bulma.min.css";

import { NotificationsHub } from "./notifications-hub";
import { EnvironmentDetailsModule } from "./environment-details-module";
import { RequestTableModule } from "./request-table-module";

const NOTIFICATIONS_HUB = new NotificationsHub();
const REQUEST_TABLE_MODULE = new RequestTableModule(NOTIFICATIONS_HUB);

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
      NOTIFICATIONS_HUB.success(
        `Requested download: ${requestBody.mediaLocator}`,
        "Requested a download"
      );
      setTimeout(() => REQUEST_TABLE_MODULE.updateTable(), 300);
    })
    .catch((err) => {
      NOTIFICATIONS_HUB.error(
        `Failed to request a download: ${err}`,
        "Failed to send a request"
      );
    });

  return false;
}

function clearFinishedRequests() {
  fetch("/requests/clear-finished", { method: "POST" })
    .then(() => {
      setTimeout(() => REQUEST_TABLE_MODULE.updateTable(), 300);
    })
    .catch((err) => {
      NOTIFICATIONS_HUB.error(
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
  byId("refreshTableButton").onclick = () => REQUEST_TABLE_MODULE.updateTable();
  byId("clearFinishedButton").onclick = () => clearFinishedRequests();
}

function main() {
  bindPageComponentsToActions();
  EnvironmentDetailsModule.initialise();

  REQUEST_TABLE_MODULE.updateTable();
  setInterval(() => {
    REQUEST_TABLE_MODULE.updateTable();
  }, 60000);
}

main();
