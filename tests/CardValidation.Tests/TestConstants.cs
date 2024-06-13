using CardValidation.Api.Contracts.ValidateCard;

namespace CardValidation.Tests;

internal class TestConstants
{
    internal static readonly ValidateCardRequest BadRequest = new ValidateCardRequest { CardNumber = "string", Cvc = 1337, FullName = "unknown" };
    internal static readonly ValidateCardRequest GoodRequest = new ValidateCardRequest { CardNumber = "4263982640269299", Cvc = 420, FullName = "John Doe", ExpirationDate = new DateTime(2030, 1, 1).ToString("o") };

    internal const string ValidateCardEndpointV1 = "/v1/ValidateCardEndpoint";
    internal const string ValidateCardEndpointV2 = "/v2/ValidateCardEndpoint";

    internal const string ErroneousCvc = "Cvc";
    internal const string ErroneousCardNumber = "CardNumber";
    internal const string ErroneousExpirationDate = "ExpirationDate";
}
