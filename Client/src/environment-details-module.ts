export interface EnvironmentChecksDto {
  availableDiskSpacePercentage: number;
  downloaderAvailable: boolean;
  postProcessingAvailable: boolean;
}

export class EnvironmentDetailsModule {
  public static async initialise(): Promise<void> {
    const response = await fetch("/environment", { method: "GET" });
    const environmentDto = await response.json();
    return EnvironmentDetailsModule._process(environmentDto);
  }

  private static _process(environmentDto: EnvironmentChecksDto): void {
    const freeSpace = environmentDto.availableDiskSpacePercentage;
    EnvironmentDetailsModule._updateTag(
      "diskSpaceStatusTag",
      freeSpace > 25 ? "is-success" : freeSpace > 5 ? "is-warning" : "is-danger"
    );
    EnvironmentDetailsModule._updateTag(
      "downloaderStatusTag",
      environmentDto.downloaderAvailable ? "is-success" : "is-danger"
    );
    EnvironmentDetailsModule._updateTag(
      "postProcessingStatusTag",
      environmentDto.postProcessingAvailable ? "is-success" : "is-warning"
    );
  }

  private static _updateTag(tagId: string, className: string): void {
    document.getElementById(tagId).classList.add(className);
  }
}
