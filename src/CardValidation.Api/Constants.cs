namespace CardValidation.Api;

internal static class Constants
{
    internal const uint ApiVersionV1 = 1;
    internal const uint ApiVersionV2 = 2;
    internal const uint ApiVersionV3 = 3;
    internal static ReadOnlySpan<uint> SUPPORTED_API_VERIONS => [ApiVersionV1, ApiVersionV2, ApiVersionV3];
}
