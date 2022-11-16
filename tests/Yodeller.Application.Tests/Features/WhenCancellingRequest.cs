using Yodeller.Application.Features;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Tests.Features;

public class WhenCancellingRequest
{
    private readonly Mock<IMessageProducer<BaseMessage>> _producerMock = new();


    [Fact]
    public async Task GivenValidIdThenFinishSuccessfully()
    {
        var givenCommand = new CancelDownloadCommand("1234");
        var sut = new CancelDownloadCommandHandler(_producerMock.Object);

        var act = async () => await sut.Handle(givenCommand, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GivenValidIdThenProduceExactly1Command()
    {
        var givenCommand = new CancelDownloadCommand("1234");
        var sut = new CancelDownloadCommandHandler(_producerMock.Object);

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(It.IsAny<BaseMessage>()), Times.Once);
    }

    [Theory]
    [InlineData("Sample-Request-Id")]
    [InlineData("1234")]
    [InlineData("ABC")]
    public async Task GivenValidIdThenProduceExpectedMessage(string givenRequestId)
    {
        var givenCommand = new CancelDownloadCommand(givenRequestId);
        var expectedMessage = new RequestedDownloadCancellation(givenRequestId);
        var sut = new CancelDownloadCommandHandler(_producerMock.Object);

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(expectedMessage), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    \t")]
    public async Task GivenInvalidIdThenThrowArgumentException(string? givenRequestId)
    {
        var givenCommand = new CancelDownloadCommand(givenRequestId!);
        var sut = new CancelDownloadCommandHandler(_producerMock.Object);

        var act = async () => await sut.Handle(givenCommand, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
