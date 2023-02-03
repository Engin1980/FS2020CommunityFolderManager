using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityManagerLib.Addons
{
    public class Scanner
  {
    private const string INACTIVE_ADDONS_FOLDER = Settings.Settings.INACTIVE_ADDONS_SUBFOLDER;


    public BindingList<AddonInfo> CreateAddonInfos(string communityFolder, Dictionary<string, State> stateDict)
    {
      List<AddonInfo> ret = new();

      List<Addon> activeAddons = ScanAddons(communityFolder);

      string inactiveCommunityFolder = System.IO.Path.Combine(communityFolder, INACTIVE_ADDONS_FOLDER);
      List<Addon> inactiveAddons =
        System.IO.Directory.Exists(inactiveCommunityFolder)
          ? ScanAddons(inactiveCommunityFolder)
          : new();

      foreach (Addon addon in activeAddons)
      {
        State state = stateDict.ContainsKey(addon.Folder)
            ? stateDict[addon.Folder]
            : new State()
            {
              IsNew = true
            };
        state.IsActive = true;
        ret.Add(new AddonInfo(addon, state));
      }

      foreach (Addon addon in inactiveAddons)
      {
        State state = stateDict.ContainsKey(addon.Folder)
            ? stateDict[addon.Folder]
            : new State()
            {
              IsNew = true
            };
        state.IsActive = false;
        ret.Add(new AddonInfo(addon, state));
      }

      return ret.ToBindingList();
    }

    private List<Addon> ScanAddons(string communityFolder)
    {
      List<Addon> ret = new();
      var dirs = System.IO.Directory.GetDirectories(communityFolder);
      foreach (var dir in dirs)
      {
        if (System.IO.Path.GetFileName(dir) == INACTIVE_ADDONS_FOLDER) continue;
        Addon addon = ScanAddonManifestFile(dir);
        ret.Add(addon);
      }
      return ret;
    }

    private Addon ScanAddonManifestFile(string addonFolder)
    {
      string file = System.IO.Path.Combine(addonFolder, "manifest.json");
      string fileContent = System.IO.File.ReadAllText(file);
      JObject json = JObject.Parse(fileContent);
      string? title = (string?)json["title"];
      string? contentType = (string?)json["content_type"];
      Addon ret = new Addon(System.IO.Path.GetFileName(addonFolder))
      {
        ManifestTitle = title,
        Type = contentType
      };
      JArray? deps = (JArray)json["depenencies"];
      if (deps != null)
        foreach (var dep in deps)
        {
          AddonDependency ad = new AddonDependency()
          {
            Name = (string)dep["name"]!,
            PackageVersion = (string)dep["package_version"]!
          };
          ret.Dependencies.Add(ad);
        }

      return ret;
    }
  }
}
