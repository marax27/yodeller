namespace Yodeller.Web.Features;

public record GetRequestDto(
    string RequestId,
    string MediaId,
    string Status
);
