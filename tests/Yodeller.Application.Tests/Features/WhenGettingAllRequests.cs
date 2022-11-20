using Yodeller.Application.Features;
using Yodeller.Application.Models;

namespace Yodeller.Application.Tests.Features;

public class WhenGettingAllRequests
{
    private readonly Mock<IDownloadRequestsRepository> _mockRepository = new();

    [Fact]
    public async Task GivenEmptyRepositoryThenReturnEmptyCollection()
    {
        var sut = new GetAllRequestsQueryHandler(_mockRepository.Object);

        var result = await sut.Handle(new(), CancellationToken.None);

        result.Requests.Should().BeEmpty();
    }

    [Theory]
    [InlineData(DownloadRequestStatus.New, "New")]
    [InlineData(DownloadRequestStatus.Completed, "Completed")]
    [InlineData(DownloadRequestStatus.InProgress, "In progress")]
    [InlineData(DownloadRequestStatus.Failed, "Failed")]
    [InlineData(DownloadRequestStatus.Cancelled, "Cancelled")]
    public async Task Given2RequestsWithCertainStatusThenReturnExpectedRequests(DownloadRequestStatus givenStatus, string expectedStatus)
    {
        _mockRepository.Setup(mock => mock.FindByStatus(givenStatus))
            .Returns(new[]
            {
                CreateSampleRequest(givenStatus),
                CreateOtherRequest(givenStatus)
            });
        _mockRepository.Setup(mock => mock.FindByStatus(It.IsNotIn(givenStatus)))
            .Returns(Array.Empty<DownloadRequest>());

        var sut = new GetAllRequestsQueryHandler(_mockRepository.Object);

        var result = await sut.Handle(new(), CancellationToken.None);

        result.Should().BeEquivalentTo(
            new GetAllRequestsQuery.Result(
                new GetAllRequestsQuery.DownloadRequestDto[]
                {
                    new("id1", "aaa111", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), expectedStatus),
                    new("id2", "http://test.test/videos?id=456", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), expectedStatus)
                }
            )
        );
    }

    [Fact]
    public async Task GivenRequestsWithDifferentStatusesExistThenReturnExpectedRequests()
    {
        _mockRepository.Setup(mock => mock.FindByStatus(DownloadRequestStatus.Completed))
            .Returns(new[]{ CreateSampleRequest(DownloadRequestStatus.Completed) });
        _mockRepository.Setup(mock => mock.FindByStatus(DownloadRequestStatus.InProgress))
            .Returns(new[] { CreateOtherRequest(DownloadRequestStatus.InProgress) });

        var sut = new GetAllRequestsQueryHandler(_mockRepository.Object);

        var result = await sut.Handle(new(), CancellationToken.None);

        result.Should().BeEquivalentTo(
            new GetAllRequestsQuery.Result(
                new GetAllRequestsQuery.DownloadRequestDto[]
                {
                    new("id1", "aaa111", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), "Completed"),
                    new("id2", "http://test.test/videos?id=456", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), "In progress")
                }
            )
        );
    }

    private static DownloadRequest CreateSampleRequest(DownloadRequestStatus status) => new(
        "id1",
        new DateTime(2005, 1, 1, 12, 0, 0),
        "aaa111",
        false,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        status
    );

    private static DownloadRequest CreateOtherRequest(DownloadRequestStatus status) => new(
        "id2",
        new DateTime(2005, 1, 2, 1, 59, 59),
        "http://test.test/videos?id=456",
        false,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        status
    );
}
