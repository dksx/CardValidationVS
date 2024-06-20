using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using CardValidation.Api.Contracts.ValidateCard;
using CardValidation.Api.Entities;
using MediatR;

namespace CardValidation.Api.Features.ValidateCard;
public static class ValidateCardLogic
{
    public class ValidateCardQuery(ValidateCardRequest validateCardRequest) : IRequest<Results<Ok<Card>, BadRequest<ValidationProblemDetails>>>
    {
        public ValidateCardRequest ValidateCardRequest { get; } = validateCardRequest;
    }

    public sealed class ValidateCardHandler : IRequestHandler<ValidateCardQuery, Results<Ok<Card>, BadRequest<ValidationProblemDetails>>>
    {
        public async Task<Results<Ok<Card>, BadRequest<ValidationProblemDetails>>> Handle(ValidateCardQuery query, CancellationToken cancellationToken)
        {
            CardModelValidator validator = new CardModelValidator();
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
