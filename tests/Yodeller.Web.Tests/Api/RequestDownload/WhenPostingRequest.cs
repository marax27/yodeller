using System.Net;
using System.Net.Http.Json;
using Yodeller.Application.Messages;
using Yodeller.Web.Features;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenPostingRequest : IClassFixture<TestApplicationWithMockedQueue>
{
    private readonly TestApplicationWithMockedQueue _application;

    public WhenPostingRequest(TestApplicationWithMockedQueue application)
    {
        _application = application;
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("http://sample-media-page.com/videos?id=333333333333")]
    public async Task GivenValidRequestThenReturnHttpNoContent(string givenMediaLocator)
    {
        var response = await PostRequest(givenMediaLocator);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("9999")]
    [InlineData("http://sample-media-page.com/videos?id=444444")]
    public async Task GivenValidRequestThenProducerReceivedExpectedRequest(string givenMediaLocator)
    {
        var response = await PostRequest(givenMediaLocator);

        _application.MockRequestProducer
            .Verify(mock => mock.Produce(It.Is<RequestedNewDownload>(
                message => message.Request.MediaLocator == givenMediaLocator)), Times.Once);
    }

    private async Task<HttpResponseMessage> PostRequest(string givenMediaLocator)
    {
        var givenRequestBody = JsonContent.Create(new NewRequestDto(givenMediaLocator, false));
        var sut = _application.CreateClient();

        var response = await sut.PostAsync("/requests", givenRequestBody);

        return response;
    }
}
