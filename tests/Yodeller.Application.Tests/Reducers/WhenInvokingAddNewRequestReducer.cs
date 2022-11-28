using System.Globalization;
using Yodeller.Application.Features.RequestDownload;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingAddNewRequestReducer
{
    private static readonly DownloadRequestsState EmptyState = new(new(), new());

    private static readonly DateTime SampleDateTime = new(2005, 1, 1, 3, 45, 59);

    private readonly DownloadRequest _sampleRequest = new(
        "id1",
        SampleDateTime,
        "media-locator",
        false,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        DownloadRequestStatus.New
    );

    [Theory]
    [InlineData("TEST1234")]
    [InlineData("0010100101010011")]
    [InlineData("http://test-media.page/videos?id=1234")]
    [InlineData("x")]
    public void GivenSampleMediaLocatorThenAddedRequestHasExpectedMediaLocator(string givenMediaLocator)
    {
        var givenRequest = _sampleRequest with { MediaLocator = givenMediaLocator };
        var sut = new AddNewRequestReducer(givenRequest);

        var newState = sut.Invoke(EmptyState);

        newState.Requests.Single()
            .MediaLocator
            .Should().Be(givenMediaLocator);
    }

    [Theory]
    [InlineData("")]
    [InlineData("en")]
    [InlineData("en.*,pl")]
    [InlineData("live_chat,en,de,jp")]
    public void GivenSampleSubtitlePatternsThenAddedRequestHasExpectedSubtitlePatterns(string givenSubtitlePatterns)
    {
        var givenSubtitlePatternsList = givenSubtitlePatterns.Split(',');
        var givenRequest = _sampleRequest with { SubtitlePatterns = givenSubtitlePatternsList };
        var sut = new AddNewRequestReducer(givenRequest);

        var newState = sut.Invoke(EmptyState);

        newState.Requests.Single()
            .SubtitlePatterns
            .Should().BeEquivalentTo(givenSubtitlePatternsList);
    }

    [Theory]
    [InlineData("1999-01-01 01:01:01")]
    [InlineData("2012-05-30 14:00:00")]
    [InlineData("2022-12-31 23:59:59")]
    public void GivenSampleRequestedTimeThenAddedRequestHasExpectedRequestedTime(string givenDateTimeText)
    {
        var givenRequestedTime = DateTime.ParseExact(givenDateTimeText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        var givenRequest = _sampleRequest with { RequestedTime = givenRequestedTime };
        var sut = new AddNewRequestReducer(givenRequest);

        var newState = sut.Invoke(EmptyState);

        newState.Requests.Single()
            .RequestedTime
            .Should().Be(givenRequestedTime);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GivenSampleAudioFlagThenAddedRequestHasExpectedAudioFlag(bool givenAudioFlag)
    {
        var givenRequest = _sampleRequest with { AudioOnly = givenAudioFlag };
        var sut = new AddNewRequestReducer(givenRequest);

        var newState = sut.Invoke(EmptyState);

        newState.Requests.Single()
            .AudioOnly
            .Should().Be(givenAudioFlag);
    }
}
