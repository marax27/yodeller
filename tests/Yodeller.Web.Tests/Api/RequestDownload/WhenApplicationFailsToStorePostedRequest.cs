﻿using System.Net;
using System.Net.Http.Json;
using Yodeller.Application.Messages;
using Yodeller.Web.Features;
using Yodeller.Web.Tests.Helpers;

namespace Yodeller.Web.Tests.Api.RequestDownload;

public class WhenApplicationFailsToStorePostedRequest : IClassFixture<TestApplication>
{
    private readonly TestApplication _application;

    public WhenApplicationFailsToStorePostedRequest(TestApplication application)
    {
        _application = application;
        _application.MockRequestProducer.Setup(mock => mock.Produce(It.IsAny<BaseMessage>()))
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
        var givenRequestBody = JsonContent.Create(new NewRequestDto(givenMediaLocator));
        var sut = _application.CreateClient();

        var response = await sut.PostAsync("/requests", givenRequestBody);

        return response;
    }
}
