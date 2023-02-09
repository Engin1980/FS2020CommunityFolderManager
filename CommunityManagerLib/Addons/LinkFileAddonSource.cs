using System;

namespace CommunityManagerLib.Addons
{
  public class LinkFileAddonSource : AddonSource
  {
    private readonly string folder;

    public override string Folder => folder;
    public string LinkFile { get; private set; }
    public override string Source => LinkFile;
    public LinkFileAddonSource(string linkFileName)
    {
      LinkFile = linkFileName ?? throw new ArgumentNullException(nameof(linkFileName));
      IWshRuntimeLibrary.IWshShell wsh = new IWshRuntimeLibrary.WshShellClass();
      IWshRuntimeLibrary.IWshShortcut sc = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(LinkFile);
      this.folder = sc.TargetPath;
    }
  }
}
