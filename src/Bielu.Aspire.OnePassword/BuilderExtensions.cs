using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Bielu.Aspire.Resources.Storage;
using Microsoft.Extensions.Configuration;

namespace Bielu.Aspire.OnePassword;

public static class BuilderExtensions
{
    public static (IResourceBuilder<ContainerResource> onePasswordSyncApi, IResourceBuilder<ContainerResource> onePasswordConnectApi) AddOnePasswordConnect(
        this IDistributedApplicationBuilder builder)
    {
        var buildCredentialVolume = builder.AddFileStore("onepasswordCredentialsVolume",
            builder.Configuration.GetValue<string>("OnePassword:ServiceUser:AuthFileLocation"), false);
        var buildVolume = builder.AddFileStore("onepasswordDataVolume", builder.Configuration.GetValue<string>("OnePassword:ServiceUser:VolumeLocation"), false);
        var onePasswordConnectSync = builder.AddOnePasswordContainer("onepassword-connect-sync", "1password/connect-sync",
            buildVolume, buildCredentialVolume, 8081);

        var onePasswordConnectApi = builder
            .AddOnePasswordContainer("onepassword-connect-api", "1password/connect-api", buildVolume,
                buildCredentialVolume,
                8080).WaitFor(onePasswordConnectSync);
        return (onePasswordConnectSync, onePasswordConnectApi);
    }
    private static IResourceBuilder<ContainerResource> AddOnePasswordContainer(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name, string image, IResourceBuilder<FileStore> volume,
        IResourceBuilder<FileStore> buildCredentialVolume, int internalPort = 8080)
    {
        ArgumentNullException.ThrowIfNull(builder);
      return   builder.AddContainer(name, image, "latest")
          .WithHttpEndpoint(targetPort:internalPort, name:"http")
          .WithEnvironment("OP_CONNECT_TOKEN", builder.Configuration.GetValue<string>("OnePassword:ServiceUser:Token"))
          .WithStorage(buildCredentialVolume, "/home/opuser/.op/1password-credentials.json")
          .WithStorage(volume, "/home/opuser/.op/data");
    }
}
