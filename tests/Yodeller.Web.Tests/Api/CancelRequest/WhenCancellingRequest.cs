using System.Net;
using System.Net.Http.Json;
using Yodeller.Application.Messages;
using Yodeller.Web.Features;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.CancelRequest;

public class WhenCancellingRequest : IClassFixture<TestApplicationWithMockedQueue>
{
    private readonly TestApplicationWithMockedQueue _application;

    public WhenCancellingRequest(TestApplicationWithMockedQueue application)
    {
        _application = application;
    }

    [Fact]
    public async Task GivenValidRequestThenReturnHttpNoContent()
    {
        var response = await PostRequest("qwerty-123456");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenValidRequestThenQueueExpectedMessage()
    {
        var expectedMessage = new RequestedDownloadCancellation("qwerty-123456");

        await PostRequest("qwerty-123456");

        _application.MockRequestProducer
            .Verify(mock => mock.Produce(expectedMessage), Times.Once);
    }

    private async Task<HttpResponseMessage> PostRequest(string givenRequestId)
    {
        var sut = _application.CreateClient();

        var response = await sut.DeleteAsync($"/requests/{givenRequestId}");

        return response;
    }
}
