using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using CardValidation.Api.Entities;

namespace CardValidation.Tests.Infrastructure.IntegrationTests;

public class ValidateCardEndpointV1Tests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task ValidateCardV1InvalidInput_ReturnsValidationErrors()
    {
        var response = await _httpClient.PostAsJsonAsync(TestConstants.ValidateCardEndpointV1, TestConstants.BadRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ValidationProblemDetails? problemResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemResult);
        Assert.NotNull(problemResult.Errors);
        Assert.True(problemResult.Errors.Any());
        Assert.True(problemResult.Errors.ContainsKey(TestConstants.ErroneousCardNumber)
                && problemResult.Errors.ContainsKey(TestConstants.ErroneousCvc)
                && problemResult.Errors.ContainsKey(TestConstants.ErroneousExpirationDate));
    }

    [Fact]
    public async Task ValidateCardV1BadInput_ReturnsBadRequest()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TestConstants.ValidateCardEndpointV1)
        {
            Content = new StringContent(TestConstants.BadRequestPayload, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.NotNull(problemDetails.Detail);
        Assert.Equal(TestConstants.BadRequestPayloadDetails, problemDetails.Detail);
    }

    [Fact]
    public async Task ValidateCardV1ValidInput_ReturnsOK()
    {
        var response = await _httpClient.PostAsJsonAsync(TestConstants.ValidateCardEndpointV1, TestConstants.GoodRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Card? card = await response.Content.ReadFromJsonAsync<Card>();

        Assert.NotNull(card);
        Assert.Equal(TestConstants.GoodRequest.FullName, card.FullName);
        Assert.Equal(TestConstants.GoodRequest.CardNumber, card.Number);
        Assert.Equal(TestConstants.GoodRequest.Cvc, card.Cvc);
        Assert.Equal("Visa", card.CardType);
    }

    [Fact]
    public async Task ValidateCardV2ValidInput_ReturnsOK()
    {
        var response = await _httpClient.GetAsync(TestConstants.ValidateCardEndpointV2);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string? content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);
        Assert.Equal("\"Hello V2\"", content);
    }
}
