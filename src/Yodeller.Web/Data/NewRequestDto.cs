namespace Yodeller.Web.Data;

public record NewRequestDto(
    IReadOnlyCollection<string>? SubtitlePatterns,
    string MediaLocator,
    bool? AudioOnly  //if it was a bool, a missing AudioOnly field would default to false.
);
