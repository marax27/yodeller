using Core.Shared.StateManagement;
using System.Net;
using System.Net.Http.Json;
using Yodeller.Application.State;
using Yodeller.Web.Data;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenApplicationFailsToStorePostedRequest : IClassFixture<TestApplicationWithMockedQueue>
{
    private readonly TestApplicationWithMockedQueue _application;

    public WhenApplicationFailsToStorePostedRequest(TestApplicationWithMockedQueue application)
    {
        _application = application;
        _application.MockRequestProducer.Setup(mock => mock.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()))
            .Throws(new InvalidOperationException("Internal producer failure"));
    }

    [Fact]
    public async Task GivenValidRequestThenReturnHttpInternalServerError()
    {
        var response = await PostRequest("1234id");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private async Task<HttpResponseMessage> PostRequest(string givenMediaLocator)
    {
        var givenRequestBody = JsonContent.Create(new NewRequestDto(Array.Empty<string>(), givenMediaLocator, false));
        var sut = _application.CreateClient();

        var response = await sut.PostAsync("/requests", givenRequestBody);

        return response;
    }
}
