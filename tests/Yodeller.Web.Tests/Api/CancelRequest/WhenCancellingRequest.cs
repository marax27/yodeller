using System.Net;
using Yodeller.Application.Features.CancelRequest;
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
        var expectedReducer = new CancelRequestReducer("qwerty-123456");

        await PostRequest("qwerty-123456");

        _application.MockRequestProducer
            .Verify(mock => mock.Produce(expectedReducer), Times.Once);
    }

    private async Task<HttpResponseMessage> PostRequest(string givenRequestId)
    {
        var sut = _application.CreateClient();

        var response = await sut.DeleteAsync($"/requests/{givenRequestId}");

        return response;
    }
}
