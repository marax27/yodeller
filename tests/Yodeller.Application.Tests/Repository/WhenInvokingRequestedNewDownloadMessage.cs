using Yodeller.Application.Messages;
using Yodeller.Application.Models;

namespace Yodeller.Application.Tests.Repository;

public class WhenInvokingRequestedNewDownloadMessage
{
    private readonly DownloadRequest _sampleRequest = new(
        "id1",
        new DateTime(1995, 10, 21, 19, 59, 30),
        "http://test-media-page.test?id=1234",
        true,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        DownloadRequestStatus.New
    );

    private readonly DownloadRequestsRepository _sut = new ();

    [Fact]
    public void GivenSampleMessageThenDownloadRequestIsAddedToRepository()
    {
        var givenMessage = new RequestedNewDownload(_sampleRequest);

        givenMessage.Invoke(_sut);

        var actual = _sut.FindById("id1");
        actual.Should().BeEquivalentTo(_sampleRequest);
    }

    [Fact]
    public void GivenSameMessageSentTwiceThenRepositoryThrowsException()
    {
        var firstMessage = new RequestedNewDownload(_sampleRequest);
        var secondMessage = new RequestedNewDownload(_sampleRequest);
        var act = () => secondMessage.Invoke(_sut);

        firstMessage.Invoke(_sut);
        act.Should().Throw<ArgumentException>();
    }
}
