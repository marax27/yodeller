export class TableOverlay {
  private _startTime?: Date;

  public enable(): void {
    this._overlay().classList.add("is-loading");
    this._startTime = new Date();
  }

  public disable(): void {
    const overlayDurationMs = new Date().getTime() - this._startTime.getTime();
    const overlay = this._overlay();

    if (overlayDurationMs >= 250) {
      overlay.classList.remove("is-loading");
    } else {
      setTimeout(
        () => overlay.classList.remove("is-loading"),
        250 - overlayDurationMs
      );
    }
  }

  private _overlay(): HTMLElement {
    return document.querySelector("#refreshOverlay");
  }
}
