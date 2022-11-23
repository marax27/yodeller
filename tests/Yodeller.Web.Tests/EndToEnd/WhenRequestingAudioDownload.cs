using FluentAssertions.Execution;
using System.Net.Http.Json;
using Yodeller.Web.Data;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.EndToEnd;

public class WhenRequestingAudioDownload : IClassFixture<TestApplicationWithFunctionalQueue>
{
    private readonly TestApplicationWithFunctionalQueue _application;

    public WhenRequestingAudioDownload(TestApplicationWithFunctionalQueue application)
    {
        _application = application;
    }

    [Fact]
    public async Task Given1ValidRequestWithNullSubtitlePatternsThen1DownloadIsExecuted()
    {
        var sut = _application.CreateClient();

        await PostRequest(null, "valid-media-locator", sut);
        await Task.Delay(300);

        _application.ExecutedDownloads.Single().AudioOnly.Should().BeTrue();
    }

    private async Task PostRequest(List<string>? subtitlePatterns, string givenMediaLocator, HttpClient client)
    {
        var payload = new NewRequestDto(subtitlePatterns, givenMediaLocator, true);
        var givenRequestBody = JsonContent.Create(payload);

        await client.PostAsync("/requests", givenRequestBody);
    }
}
