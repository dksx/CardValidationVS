namespace CardValidation.Api.Contracts.ValidateCard;

public readonly record struct ValidateCardRequest
{
    public ValidateCardRequest() { }

    public string FullName { get; init; } = string.Empty;

    public string CardNumber { get; init; } = string.Empty;

    public uint Cvc { get; init; }

    public DateTime ExpirationDate { get; init; }
}
