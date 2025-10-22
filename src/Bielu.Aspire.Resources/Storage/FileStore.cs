using Aspire.Hosting.ApplicationModel;

namespace Bielu.Aspire.Resources.Storage;

public class FileStore : Resource
{
   public string SourcePath { get; }

   public FileStore(string name, string? sourcePath = null, bool isVolume = false) : base(name)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(name);
      if (name.AsSpan().ContainsAny(Path.GetInvalidFileNameChars()))
      {
          throw new ArgumentException("Name contains chars not allowed in folder names", nameof(name));
      }

      if (string.IsNullOrEmpty(sourcePath))
      {
          SourcePath = Directory.CreateTempSubdirectory(name).FullName;
      }
      else if (Path.IsPathRooted(sourcePath))
      {
          throw new NotSupportedException("Use relative paths");
      }
      else
      {
          SourcePath = sourcePath;
      }

      IsVolume = isVolume;
   }

   public bool IsVolume { get; set; }
   internal string RealHostPath<T>(IResourceBuilder<T> builder) where T : IResourceWithEnvironment =>
       IsVolume ?Name : Path.IsPathRooted(SourcePath) ? SourcePath : Path.Combine(builder.ApplicationBuilder.AppHostDirectory, SourcePath);
}
