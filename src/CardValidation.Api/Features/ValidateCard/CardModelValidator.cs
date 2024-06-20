using CardValidation.Api.Contracts.ValidateCard;
using FluentValidation;

namespace CardValidation.Api.Features.ValidateCard;

public class CardModelValidator : AbstractValidator<ValidateCardRequest>
{
    public CardModelValidator()
    {
        RuleFor(cr => cr.FullName)
          .NotEmpty()
          .WithMessage("A valid fullname for the card owner is required.");

        RuleFor(cr => cr.CardNumber)
          .NotEmpty()
          .CreditCard()
          .WithMessage("A valid credit card number is required.");

        RuleFor(cr => cr.Cvc)
          .Must(cvc => cvc >= 100 && cvc <= 9999)
          .WithMessage("A valid CVC is required.");

        RuleFor(cr => cr.Cvc)
          .Must(cvc => cvc >= 100 && cvc <= 999)
          .When(x => x.CardNumber is not null && (x.CardNumber.StartsWith('4') || x.CardNumber.StartsWith('5')))
          .WithMessage("A valid 3 digit CVC for this type of card is required.");

        RuleFor(cr => cr.Cvc)
          .Must(cvc => cvc >= 1000 && cvc <= 9099)
          .When(x => x.CardNumber is not null && x.CardNumber.StartsWith('3'))
          .WithMessage("A valid 4 digit CVC for this type of card is required.");

        RuleFor(cr => cr.ExpirationDate)
          .NotEmpty()
          .Must(expirationDate => BeAValidDate(expirationDate))
          .WithMessage("A valid expiration date is required.");
    }

    private static bool BeAValidDate(string date)
    {
        if (!DateTime.TryParse(date, out DateTime expirationDate))
        {
            return false;
        }

        return expirationDate > DateTime.UtcNow;
    }
}
