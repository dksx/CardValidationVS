using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using CardValidation.Api.Entities;
using MediatR;
using Xunit;

using static CardValidation.Api.Features.Articles.ValidateCard;

namespace CardValidation.Tests.Application.UnitTests;
public class ValidateCardMediaTRPipelineTests
{
    internal IMediator Mediator;

    public ValidateCardMediaTRPipelineTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
            .BuildServiceProvider();

        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task ValidationRequestShouldSucceed()
    {
        var query = new ValidateCardQuery(TestConstants.GoodRequest);
        // Act
        var response = await Mediator.Send(query);
        Assert.NotNull(response);
        Assert.IsType<Ok<Card>>(response.Result);
    }

    [Fact]
    public async Task ValidationRequestShouldFail()
    {
        var query = new ValidateCardQuery(TestConstants.BadRequest);
        // Act
        var response = await Mediator.Send(query);
        Assert.NotNull(response);
        Assert.IsType<BadRequest<ValidationProblemDetails>>(response.Result);
    }
}
