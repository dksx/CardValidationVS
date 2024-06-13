namespace CardValidation.Api.Entities;

public class Card
{
    public string Number { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public uint Cvc { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string CardType { get; set; } = string.Empty;
}

public enum CardType
{
    Visa,
    MasterCard,
    AmericanExpress,
    Unknown
}
