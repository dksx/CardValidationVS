using System;
using System.Reflection;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using CardValidation.Api.Shared;

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

        ApiVersionSet versionSet = app.NewApiVersionSet()
            .HasApiVersion(Constants.ApiVersionV1)
            .HasApiVersion(Constants.ApiVersionV2)
            .ReportApiVersions()
            .Build();

        Assembly assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app, versionSet);
            }
        }

        return app;
    }
}
