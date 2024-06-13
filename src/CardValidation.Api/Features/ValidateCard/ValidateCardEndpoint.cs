using Microsoft.AspNetCore.Mvc;
using Asp.Versioning.Builder;
using CardValidation.Api.Contracts.ValidateCard;
using CardValidation.Api.Extensions;
using CardValidation.Api.Shared;
using MediatR;

using static CardValidation.Api.Features.Articles.ValidateCard;
using CardValidation.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CardValidation.Api.Features.ValidateCard;

public class ValidateCardEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app, ApiVersionSet versionSet)
    {
        app.MapGroup(this, versionSet)
            .HasApiVersion(Constants.ApiVersionV1)
            .MapPost(ValidateCardDetails);

        app.MapGroup(this, versionSet)
            .HasApiVersion(Constants.ApiVersionV2)
            .MapGet(ValidateCardDetailsV2);
    }

    public async Task<Results<Ok<Card>, BadRequest<ValidationProblemDetails>>> ValidateCardDetails([FromBody] ValidateCardRequest request, [FromServices] IMediator mediator)
    {
        ValidateCardQuery query = new ValidateCardQuery(request);
        return await mediator.Send(query);
    }

    public IResult ValidateCardDetailsV2()
    {
        return TypedResults.Ok("Hello V2");
    }
}
