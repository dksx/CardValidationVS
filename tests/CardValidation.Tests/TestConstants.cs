using CardValidation.Api.Contracts.ValidateCard;

namespace CardValidation.Tests;

internal class TestConstants
{
    internal static readonly ValidateCardRequest BadRequest = new ValidateCardRequest { CardNumber = "string", Cvc = 13, FullName = "unknown" };
    internal static readonly ValidateCardRequest GoodRequest = new ValidateCardRequest { CardNumber = "4263982640269299", Cvc = 420, FullName = "John Doe", ExpirationDate = new DateTime(2030, 1, 1).ToString("o") };

    internal const string BadRequestPayload = @"{""fullName"": ""string"",  ""cardNumber"": ""string"",  ""cvc"": bad,  ""expirationDate"": ""string""}";
    internal const string BadRequestPayloadDetails = "The request body does not conform to the expected model, please see /swagger/index.html";

    internal const string ValidateCardEndpointV1 = "/v1/ValidateCard";
    internal const string ValidateCardEndpointV2 = "/v2/ValidateCard";

    internal const string ErroneousCvc = "Cvc";
    internal const string ErroneousCardNumber = "CardNumber";
    internal const string ErroneousExpirationDate = "ExpirationDate";
}
