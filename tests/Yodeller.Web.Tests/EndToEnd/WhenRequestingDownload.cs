using System.Net.Http.Json;
using Yodeller.Web.Features;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.EndToEnd;

public class WhenRequestingDownload : IClassFixture<TestApplicationWithFunctionalQueue>
{
    private readonly TestApplicationWithFunctionalQueue _application;

    public WhenRequestingDownload(TestApplicationWithFunctionalQueue application)
    {
        _application = application;
    }

    [Fact]
    public async Task Given1ValidRequestThen1DownloadIsExecuted()
    {
        await PostRequest("valid-media-locator");

        await Task.Delay(1200);

        _application.ExecutedDownloads.Should().HaveCount(1);
    }

    private async Task PostRequest(string givenMediaLocator)
    {
        var givenRequestBody = JsonContent.Create(new NewRequestDto(givenMediaLocator));
        var sut = _application.CreateClient();

        await sut.PostAsync("/requests", givenRequestBody);
    }
}
