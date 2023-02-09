namespace CommunityManagerLib.Addons
{
  public abstract class AddonSource
  {
    public const string MANIFEST_FILE_NAME = "manifest.json";
    public abstract string Folder { get; }
    public abstract string ManifestFilePath { get; }
    public abstract string Source { get; }
  }
}
