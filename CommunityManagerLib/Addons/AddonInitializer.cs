using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Addons
{
  public class AddonInitializer
  {
    private const string INACTIVE_ADDONS_FOLDER = Settings.Settings.INACTIVE_ADDONS_SUBFOLDER;

    public List<AddonView> AnalyseAddons(string communityFolder,
      Dictionary<string, AddonCustomInfo> stateDict,
      out List<string> errors)
    {
      errors = new();
      List<SingleAddonView> tmp = new();
      List<AddonSource> addonSources;

      addonSources = ScanForAddonSources(communityFolder, ref errors);
      foreach (AddonSource addonSource in addonSources)
      {
        AddonManifestData manifest = AddonManifestData.Create(addonSource);
        Addon a;
        if (stateDict.ContainsKey(addonSource.Source))
          a = new Addon(addonSource, manifest, stateDict[addonSource.Source], true, false);
        else
          a = new Addon(addonSource, manifest, new AddonCustomInfo(), true, true);
        tmp.Add(new SingleAddonView(a));
      }

      string inactiveCommunityFolder = System.IO.Path.Combine(communityFolder, INACTIVE_ADDONS_FOLDER);
      if (System.IO.Directory.Exists(inactiveCommunityFolder))
      {
        addonSources = ScanForAddonSources(inactiveCommunityFolder, ref errors);
        foreach (AddonSource addonSource in addonSources)
        {
          AddonManifestData manifest = AddonManifestData.Create(addonSource);
          Addon a;
          if (stateDict.ContainsKey(addonSource.Source))
            a = new Addon(addonSource, manifest, stateDict[addonSource.Source], true, false);
          else
            a = new Addon(addonSource, manifest, new AddonCustomInfo(), true, true);
          tmp.Add(new SingleAddonView(a));
        }
      }

      List<AddonView> ret = BuildGroupedAddons(tmp);

      return ret;
    }

    private List<AddonView> BuildGroupedAddons(List<SingleAddonView> lst)
    {
      List<AddonView> ret = new();
      HashSet<string> usedGroupKeys = new();

      foreach (var item in lst)
      {
        if (string.IsNullOrEmpty(item.Addon.State.GroupGuid))
          ret.Add(item);
        else
        {
          string key = item.Addon.State.GroupGuid;
          if (usedGroupKeys.Contains(key)) continue;
          usedGroupKeys.Add(key);
          List<SingleAddonView> addons = lst.Where(q => q.Addon.State.GroupGuid == key).ToList();
          GroupAddonView gav = new(addons, addons.First().Title + $" + {addons.Count - 1} addons");
          ret.Add(gav);
        }
      }

      return ret;
    }

    private List<AddonSource> ScanForAddonSources(string communityFolder, ref List<string> errors)
    {
      List<AddonSource> folderSources = System.IO.Directory.GetDirectories(communityFolder)
        .Where(q => System.IO.Path.GetFileName(q) != INACTIVE_ADDONS_FOLDER)
        .Select(q => (AddonSource)new FolderAddonSource(q))
        .ToList();

      List<AddonSource> linkSources = System.IO.Directory.GetFiles(communityFolder)
        .Where(q => System.IO.Path.GetExtension(q) == ".lnk")
        .Select(q => (AddonSource)new LinkFileAddonSource(q))
        .ToList();

      List<AddonSource> ret = new();
      ret.AddRange(folderSources);
      ret.AddRange(linkSources);

      var missingManifestSources = ret
        .Where(q => !System.IO.File.Exists(System.IO.Path.Combine(q.ManifestFilePath)))
        .ToList();

      var tmp = errors;
      missingManifestSources.ForEach(q => tmp.Add($"Addon source {q.Source} has missing manifest file. Cannot be loaded."));
      missingManifestSources.ForEach(q => ret.Remove(q));

      return ret;
    }
  }
}
