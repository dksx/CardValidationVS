using Asp.Versioning.Builder;

namespace CardValidation.Api.Infrastructure;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app, ApiVersionSet versionSet);
}
