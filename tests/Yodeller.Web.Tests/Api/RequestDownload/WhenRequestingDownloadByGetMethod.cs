using System.Net;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenRequestingDownloadByGetMethod : IClassFixture<TestApplication>
{
    private readonly TestApplication _application;

    public WhenRequestingDownloadByGetMethod(TestApplication application)
    {
        _application = application;
    }

    [Theory]
    [InlineData("o")]
    [InlineData("1234")]
    [InlineData("TEST-9999")]
    public async Task GivenValidMediaLocatorThenReturnHttpContent(string givenMediaLocator)
    {
        var response = await SendGetRequest(givenMediaLocator);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<HttpResponseMessage> SendGetRequest(string givenMediaLocator)
    {
        var sut = _application.CreateClient();

        var response = await sut.GetAsync($"/requests/{givenMediaLocator}");

        return response;
    }
}
