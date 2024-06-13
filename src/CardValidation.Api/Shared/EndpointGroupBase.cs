using Asp.Versioning.Builder;

namespace CardValidation.Api.Shared;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app, ApiVersionSet versionSet);
}
