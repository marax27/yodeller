import { ComponentBuilder } from "./component-builder";

export class NotificationsHub {
  public success(message: string, title: string): void {
    this._renderNew(message, "is-success", title);
  }

  public warning(message: string, title: string): void {
    this._renderNew(message, "is-warning", title);
  }

  public error(message: string, title: string): void {
    this._renderNew(message, "is-danger", title);
  }

  private _renderNew(message: string, className: string, title: string): void {
    const notifElement = this._create(message, className, title);
    const formCard = document.getElementById("formCard");
    formCard.parentNode.insertBefore(notifElement, formCard.nextSibling);
  }

  private _create(
    message: string,
    className: string,
    title: string
  ): HTMLElement {
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
  }
}
