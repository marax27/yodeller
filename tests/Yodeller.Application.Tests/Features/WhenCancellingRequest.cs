using Core.Shared.StateManagement;
using Yodeller.Application.Features.CancelRequest;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Features;

public class WhenCancellingRequest
{
    private readonly Mock<IMessageProducer<IStateReducer<DownloadRequestsState>>> _producerMock = new();


    [Fact]
    public async Task GivenValidIdThenFinishSuccessfully()
    {
        var givenCommand = new CancelRequestCommand("1234");
        var sut = new CancelRequestCommandHandler(_producerMock.Object);

        var act = async () => await sut.Handle(givenCommand, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GivenValidIdThenProduceExactly1Reducer()
    {
        var givenCommand = new CancelRequestCommand("1234");
        var sut = new CancelRequestCommandHandler(_producerMock.Object);

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()), Times.Once);
    }

    [Theory]
    [InlineData("Sample-Request-Id")]
    [InlineData("1234")]
    [InlineData("ABC")]
    public async Task GivenValidIdThenProduceExpectedReducer(string givenRequestId)
    {
        var givenCommand = new CancelRequestCommand(givenRequestId);
        var expectedMessage = new CancelRequestReducer(givenRequestId);
        var sut = new CancelRequestCommandHandler(_producerMock.Object);

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(expectedMessage), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    \t")]
    public async Task GivenInvalidIdThenThrowArgumentException(string? givenRequestId)
    {
        var givenCommand = new CancelRequestCommand(givenRequestId!);
        var sut = new CancelRequestCommandHandler(_producerMock.Object);

        var act = async () => await sut.Handle(givenCommand, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
