using System;
using MediatR;
using FluentValidation;
using CardValidation.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using CardValidation.Api.Contracts.ValidateCard;
using Microsoft.AspNetCore.Http.HttpResults;

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
                ExpirationDate = validateCardRequest.ExpirationDate,
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
          //.NotEmpty()
          .Must(cvc => cvc >= 100 && cvc <= 999)
          .WithMessage("A valid CVC number is required.");
        RuleFor(cr => cr.ExpirationDate)
          .NotEmpty()
          .Must(expirationDate => expirationDate > DateTime.UtcNow)
          .WithMessage("A valid expiration date is required.");
    }
}
