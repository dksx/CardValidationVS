using System;
using System.Reflection;
using Microsoft.Extensions.Options;
using Asp.Versioning;
using CardValidation.Api;
using CardValidation.Api.Extensions;
using CardValidation.Api.Shared;
using FluentValidation;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

Assembly assembly = Assembly.GetExecutingAssembly();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(Constants.ApiVersionV1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

WebApplication app = builder.Build();

app.MapEndpoints();

if (app.Configuration.GetValue("EnableSwagger", false))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var dsc in descriptions)
        {
            string url = $"/swagger/{dsc.GroupName}/swagger.json";
            string name = dsc.GroupName;
            options.SwaggerEndpoint(url, name);
        }
    });
}

//app.UseHttpsRedirection();

app.UseExceptionHandler(options => {});

app.Run();

public partial class Program { }
