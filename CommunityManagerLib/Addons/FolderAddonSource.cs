using System;

namespace CommunityManagerLib.Addons
{
  public class FolderAddonSource : AddonSource
  {
    private string folderName;

    public override string Folder => folderName;
    public override string ManifestFilePath => System.IO.Path.Combine(folderName, MANIFEST_FILE_NAME);
    public override string Source => folderName;
    public FolderAddonSource(string folderName)
    {
      this.folderName = folderName ?? throw new ArgumentNullException(nameof(folderName));
    }
  }
}
