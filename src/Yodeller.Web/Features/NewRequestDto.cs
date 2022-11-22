namespace Yodeller.Web.Features;

public record NewRequestDto(
    IReadOnlyCollection<string>? SubtitlePatterns,
    string MediaLocator,
    bool? AudioOnly  //if it was a bool, a missing AudioOnly field would default to false.
);
