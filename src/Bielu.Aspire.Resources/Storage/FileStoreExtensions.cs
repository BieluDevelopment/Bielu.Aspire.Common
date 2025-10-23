using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Bielu.Aspire.Resources.Storage;

public static class FileStoreExtensions
{
    public static IResourceBuilder<FileStore> AddFileStore(this IDistributedApplicationBuilder builder,
        [ResourceName] string name, string? sourcePath = null, bool isVolume=false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var store = new FileStore(name, sourcePath,isVolume ,  builder.ExecutionContext.IsPublishMode);

        return builder.AddResource(store);
    }

    public static IResourceBuilder<T> WithStorage<T>(this IResourceBuilder<T> builder, IResourceBuilder<FileStore> store, string? containerTargetMountLocation) where T : ContainerResource
    {
        var resource                 = store.Resource;
        var resourceContainerPath    = containerTargetMountLocation ?? $"/appdata/{resource.Name}";
        var containerMountAnnotation = new ContainerMountAnnotation(resource.RealHostPath(builder), resourceContainerPath, resource.IsVolume ? ContainerMountType.Volume : ContainerMountType.BindMount, false);
        return builder.WithAnnotation(containerMountAnnotation)
            .WithEnvironment(context => context.EnvironmentVariables[$"ConnectionStrings__{resource.Name}"] = resourceContainerPath);
    }

    public static IResourceBuilder<T> WithBindMount<T>(this IResourceBuilder<T> builder, IResourceBuilder<FileStore> store) where T : ProjectResource
    {
        var resource = store.Resource;
        return builder.WithEnvironment(context => context.EnvironmentVariables[$"ConnectionStrings__{resource.Name}"] = resource.RealHostPath(builder));
    }
}
