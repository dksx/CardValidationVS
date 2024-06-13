using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using CardValidation.Api.Entities;

namespace CardValidation.Tests.Infrastructure.IntegrationTests;

public class ValidateCardEndpointV1Tests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task ValidateCardV1BadRequest()
    {
        var response = await _httpClient.PostAsJsonAsync(TestConstants.ValidateCardEndpointV1, TestConstants.BadRequest).ConfigureAwait(false);

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
    public async Task ValidateCardV1OK()
    {
        var response = await _httpClient.PostAsJsonAsync(TestConstants.ValidateCardEndpointV1, TestConstants.GoodRequest).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Card? card = await response.Content.ReadFromJsonAsync<Card>();

        Assert.NotNull(card);
        Assert.Equal(TestConstants.GoodRequest.FullName, card.FullName);
        Assert.Equal(TestConstants.GoodRequest.CardNumber, card.Number);
        Assert.Equal(TestConstants.GoodRequest.Cvc, card.Cvc);
        Assert.Equal("Visa", card.CardType);
    }

    [Fact]
    public async Task ValidateCardV2OK()
    {
        var response = await _httpClient.GetAsync(TestConstants.ValidateCardEndpointV2).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string? content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        Assert.NotNull(content);
        Assert.Equal("\"Hello V2\"", content);
    }
}
