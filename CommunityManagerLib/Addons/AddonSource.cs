namespace CommunityManagerLib.Addons
{
  public abstract class AddonSource
  {
    public const string MANIFEST_FILE_NAME = "manifest.json";
    public abstract string Folder { get; }
    public string ManifestFilePath => System.IO.Path.Combine(Folder, MANIFEST_FILE_NAME);
    public abstract string Source { get; }
    public string SourceName => System.IO.Path.GetFileName(Source);
    public string FolderName => System.IO.Path.GetFileName(Folder);
  }
}
