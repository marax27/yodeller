using System.Net;
using Yodeller.Application.Models;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenApplicationFailsToStoreRequestSentByGetMethod : IClassFixture<TestApplication>
{
    private readonly TestApplication _application;

    public WhenApplicationFailsToStoreRequestSentByGetMethod(TestApplication application)
    {
        _application = application;
        _application.MockRequestProducer.Setup(mock => mock.Produce(It.IsAny<DownloadRequest>()))
            .Throws(new InvalidOperationException("Internal producer failure"));
    }

    [Fact]
    public async Task GivenValidRequestThenReturnHttpInternalServerError()
    {
        var response = await SendGetRequest("123abc456");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private async Task<HttpResponseMessage> SendGetRequest(string givenMediaLocator)
    {
        var sut = _application.CreateClient();

        var response = await sut.GetAsync($"/requests/{givenMediaLocator}");

        return response;
    }
}
