using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Bielu.Aspire.Common.Annotations;

public static class ReverseProxyExtensions
{
    public static IResourceBuilder<T> WitHostnameForEndpoint<T>(this IResourceBuilder<T> builder, string name,
        string hostname)
        where T : IResource
    {
        var ednpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        // Best effort to prevent duplicate endpoint names
        if (ednpoint == null)
        {
            throw new DistributedApplicationException($"Endpoint with name '{name}' does not exists.");
        }

        ednpoint.TargetHost = hostname;
        return builder;
    }


}
