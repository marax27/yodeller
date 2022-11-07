using FluentAssertions.Execution;
using System.Net.Http.Json;
using Yodeller.Web.Features;
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
    public async Task Given1ValidRequestThen1DownloadIsExecuted()
    {
        var sut = _application.CreateClient();

        await PostRequest("valid-media-locator", sut);
        await Task.Delay(1100);

        _application.ExecutedDownloads.Single().AudioOnly.Should().BeTrue();
    }

    private async Task PostRequest(string givenMediaLocator, HttpClient client)
    {
        var givenRequestBody = JsonContent.Create(new NewRequestDto(givenMediaLocator, true));

        await client.PostAsync("/requests", givenRequestBody);
    }
}
