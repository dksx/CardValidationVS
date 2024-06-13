using System;
using MediatR;
using FluentValidation;
using CardValidation.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using CardValidation.Api.Contracts.ValidateCard;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata.Ecma335;

namespace CardValidation.Api.Features.Articles;
public static class ValidateCard
{
    public class ValidateCardQuery(ValidateCardRequest validateCardRequest) : IRequest<Results<Ok<Card>, BadRequest<ValidationProblemDetails>>>
    {
        public ValidateCardRequest ValidateCardRequest { get; } = validateCardRequest;
    }

    public sealed class ValidateCardHandler : IRequestHandler<ValidateCardQuery, Results<Ok<Card>, BadRequest<ValidationProblemDetails>>>
    {
        public async Task<Results<Ok<Card>, BadRequest<ValidationProblemDetails>>> Handle(ValidateCardQuery query, CancellationToken cancellationToken)
        {
            CardValidator validator = new CardValidator();
            var validationResult = await validator.ValidateAsync(query.ValidateCardRequest, cancellationToken).ConfigureAwait(false);

            //if (!validationResult.IsValid) throw new CardValidation.Api.Shared.ValidationException(validationResult.Errors);

            return !validationResult.IsValid
                ? TypedResults.BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()))
                : TypedResults.Ok(GenerateCard(query.ValidateCardRequest));
        }

        private static Card GenerateCard(ValidateCardRequest validateCardRequest)
        {
            char startDigit = validateCardRequest.CardNumber.First();
            return new Card
            {
                Number = validateCardRequest.CardNumber,
                FullName = validateCardRequest.FullName,
                Cvc = validateCardRequest.Cvc,
                ExpirationDate = DateTime.Parse(validateCardRequest.ExpirationDate),
                CardType = DetectCardType(startDigit).ToString(),
            };
        }

        private static CardType DetectCardType(char startDigit)
        {
            return startDigit switch
            {
                '3' => CardType.AmericanExpress,
                '4' => CardType.Visa,
                '5' => CardType.MasterCard,
                _ => CardType.Unknown
            };
        }
    }
}

public class CardValidator : AbstractValidator<ValidateCardRequest>
{
    public CardValidator()
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
          .When(x => x.CardNumber is not null && ( x.CardNumber.StartsWith('4') || x.CardNumber.StartsWith('5')))
          .WithMessage("A valid 3 digit CVC for this type of card is required.");

        RuleFor(cr => cr.Cvc)
          .Must(cvc => cvc >= 1000 && cvc <= 9099)
          .When(x => x.CardNumber is not null && (x.CardNumber.StartsWith('3')))
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
