using System;
using System.Reflection;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using CardValidation.Api.Infrastructure;

namespace CardValidation.Api.Extensions;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, ApiVersionSet versionSet)
    {
        string groupName = group.GetType().Name;
        return app
            .MapGroup("/v{version:apiVersion}")
            .MapGroup(groupName)
            //.WithGroupName(groupName)
            .WithTags(groupName)
            .WithApiVersionSet(versionSet)
            .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        Type endpointGroupType = typeof(EndpointGroupBase);

        Assembly assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        ApiVersionSet versionSet = HandleApiVersioning(app);

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app, versionSet);
            }
        }

        return app;
    }

    private static ApiVersionSet HandleApiVersioning(WebApplication app)
    {
        ApiVersionSetBuilder verionBuilder = app.NewApiVersionSet();
        foreach (uint supportedVersion in Constants.SUPPORTED_API_VERIONS)
        {
            verionBuilder.HasApiVersion(supportedVersion);
        }

        return verionBuilder.ReportApiVersions().Build();
    }
}
