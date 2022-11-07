namespace Yodeller.Web.Features;

public record NewRequestDto(
    string MediaLocator,
    bool AudioOnly
);
