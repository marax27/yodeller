using System.Net;
using System.Text;
using Yodeller.Web.Tests.Helpers;
using static System.Net.HttpStatusCode;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenPostingInvalidRequest : IClassFixture<TestApplicationWithMockedQueue>
{
    private readonly TestApplicationWithMockedQueue _application;

    public WhenPostingInvalidRequest(TestApplicationWithMockedQueue application)
    {
        _application = application;
    }

    [Theory]
    [InlineData("""{ "mediaLocator": "123456", "audioOnly": false, "subtitlePatterns": [] }""")]
    [InlineData("""{ "mediaLocator": "123456", "audioOnly": false }""")]
    public async Task GivenValidRequestThenReturnHttpNoContent(string givenRequestBody)
    {
        var content = new StringContent(givenRequestBody, Encoding.UTF8, "application/json");

        var sut = _application.CreateClient();

        var response = await sut.PostAsync("/requests", content);
        response.StatusCode.Should().Be(NoContent);
    }

    [Theory]
    [InlineData("""{ "mediaLocator": "123456", "subtitlePatterns": [] }""")]
    [InlineData("""{ "audioOnly": false, "subtitlePatterns": [] }""")]
    [InlineData("""{ "mediaLocator": null }""")]
    [InlineData("""{ "mediaLocator": "" }""")]
    [InlineData("""{ "unexpectedField": 1 }""")]
    [InlineData("""{  }""")]
    public async Task GivenRequestThenReturnExpectedHttpBadRequest(string givenRequestBody)
    {
        var content = new StringContent(givenRequestBody, Encoding.UTF8, "application/json");

        var sut = _application.CreateClient();

        var response = await sut.PostAsync("/requests", content);
        response.StatusCode.Should().Be(BadRequest);
    }
}
