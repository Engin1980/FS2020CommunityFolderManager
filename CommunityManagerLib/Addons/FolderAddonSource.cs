using System;
using System.Diagnostics;

namespace CommunityManagerLib.Addons
{
  public class FolderAddonSource : AddonSource
  {
    private readonly string folder;

    public override string Folder => folder;
    public override string Source => folder;
    public FolderAddonSource(string folder)
    {
      Trace.Assert(System.IO.Directory.Exists(folder));
      this.folder = folder;
    }
  }
}
