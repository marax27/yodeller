﻿using System.Net.Http.Json;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using Yodeller.Application.Features.GetAllRequests;
using Yodeller.Web.Data;
using Yodeller.Web.Tests.Helpers;
namespace Yodeller.Web.Tests.EndToEnd;

public class WhenRequestingVideoDownloadWithSubtitles : IClassFixture<TestApplicationWithFunctionalQueue>
{
    private readonly TestApplicationWithFunctionalQueue _application;

    public WhenRequestingVideoDownloadWithSubtitles(TestApplicationWithFunctionalQueue application)
    {
        _application = application;
    }

    [Fact]
    public async Task Given1ValidRequestThen1DownloadIsExecuted()
    {
        var givenSubtitles = new[] { "en.*", "pl" };

        var sut = _application.CreateClient();

        await PostRequest(givenSubtitles, "valid-media-locator", sut);
        await Task.Delay(300);
        var registeredRequests = await GetRegisteredRequests(sut);

        using (new AssertionScope())
        {
            _application.ExecutedDownloads.Should().HaveCount(1);

            _application.ExecutedDownloads.Single().AudioOnly.Should().Be(false);

            _application.ExecutedDownloads.Single().SubtitlePatterns.Should().BeEquivalentTo(givenSubtitles);

            registeredRequests.Should().NotBeNull();

            registeredRequests.Should().HaveCount(1);

            registeredRequests!.Single().MediaLocator.Should().Be("valid-media-locator");

            registeredRequests!.Single().Status.Should().Be("Completed");
        }
    }

    private async Task PostRequest(string[] givenSubtitles, string givenMediaLocator, HttpClient client)
    {
        var givenRequestBody = JsonContent.Create(new NewRequestDto(givenSubtitles, givenMediaLocator, false));
        
        await client.PostAsync("/requests", givenRequestBody);
    }

    private async Task<IReadOnlyCollection<GetAllRequestsQuery.DownloadRequestDto>?> GetRegisteredRequests(HttpClient client)
    {
        var response = await client.GetAsync("/requests");

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IReadOnlyCollection<GetAllRequestsQuery.DownloadRequestDto>>(content);
    }
}