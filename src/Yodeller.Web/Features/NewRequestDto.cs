namespace Yodeller.Web.Features;

public record NewRequestDto(
    IReadOnlyCollection<string>? SubtitlePatterns,
    string MediaLocator,
    bool AudioOnly
);
