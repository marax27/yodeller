namespace Yodeller.Web.Features;

public record GetRequestDto(
    string RequestId,
    string MediaLocator,
    string Status
);
