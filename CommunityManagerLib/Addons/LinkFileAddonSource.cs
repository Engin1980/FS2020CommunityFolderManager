using System;

namespace CommunityManagerLib.Addons
{
  public class LinkFileAddonSource : AddonSource
  {
    private string folder;

    public override string Folder => folder;
    public string LinkFileName { get; private set; }

    public override string ManifestFilePath => System.IO.Path.Combine(folder, MANIFEST_FILE_NAME);
    public override string Source => LinkFileName;
    public LinkFileAddonSource(string linkFileName)
    {
      LinkFileName = linkFileName ?? throw new ArgumentNullException(nameof(linkFileName));
      IWshRuntimeLibrary.IWshShell wsh = new IWshRuntimeLibrary.WshShellClass();
      IWshRuntimeLibrary.IWshShortcut sc = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(LinkFileName);
      this.folder = sc.FullName;
    }
  }
}
