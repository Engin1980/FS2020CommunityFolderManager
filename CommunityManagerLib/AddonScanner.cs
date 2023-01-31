using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public class AddonScanner
  {
    private const string HISTORY_JSON_FILE = "history.json";
    private const string INACTIVE_ADDONS_FOLDER = "___inactive";

    public List<AddonState> ScanAddons(string communityFolder)
    {
      List<Addon> activeAddons = ScanAddonsInCommunityFolder(communityFolder);
      string inactiveCommunityFolder = System.IO.Path.Combine(communityFolder, INACTIVE_ADDONS_FOLDER);
      List<Addon> inactiveAddons =
        System.IO.Directory.Exists(inactiveCommunityFolder)
          ? ScanAddonsInCommunityFolder(inactiveCommunityFolder)
          : new();

      List<AddonState> states =
        activeAddons.Select(q => new AddonState(q))
        .ForEach(q => q.IsActive = true)
        .Concat(
          inactiveAddons
          .Select(p => new AddonState(p))
          .ForEach(p => p.IsActive = false))
        .ForEach(q => q.IsNew = true)
        .ToList();

      JObject? json = TryLoadJObject(communityFolder);
      if (json != null)
      {
        JObject jroot = json.ToObject<JObject>()!;
        foreach (AddonState state in states)
        {
          if (jroot.ContainsKey(state.Addon.Folder) == false) continue;

          JObject jobj = (JObject)jroot[state.Addon.Folder]!;

          var customTitle = (string?)jobj["customTitle"];
          var tags = ((JArray)jobj["tags"]!).Select(q => (string)q!).ToList();

          if (customTitle != null)
            state.CustomTitle = customTitle;
          state.Tags = tags;
          state.IsNew = false;
        }
      }
      return states;
    }

    public void SaveAddonsState(List<AddonState> addonStates, string communityFolder)
    {
      JObject jroot = new JObject();
      foreach (AddonState state in addonStates)
      {
        JObject jobj = new JObject();
        if (string.IsNullOrWhiteSpace(state.CustomTitle) == false)
          jobj["customTitle"] = state.CustomTitle;
        jobj["tags"] = new JArray(state.Tags.ToArray());
        jroot[state.Addon.Folder] = jobj;
      }

      var str = JsonConvert.SerializeObject(jroot, Formatting.Indented);
      var file = GetHistoryFilePath(communityFolder);
      var tmpFile = System.IO.Path.GetTempFileName();

      try
      {
        System.IO.File.WriteAllText(tmpFile, str);
        System.IO.File.Copy(tmpFile, file, true);
        System.IO.File.Delete(tmpFile);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private JObject? TryLoadJObject(string communityFolder)
    {
      JObject ret;
      string fileName = GetHistoryFilePath(communityFolder);
      if (System.IO.File.Exists(fileName))
      {
        string fileContent = System.IO.File.ReadAllText(fileName);
        ret = JObject.Parse(fileContent);
      }
      else
        ret = null;

      return ret;
    }

    private string GetHistoryFilePath(string path) => System.IO.Path.Combine(path, HISTORY_JSON_FILE);

    public List<Addon> ScanAddonsInCommunityFolder(string communityFolder)
    {
      List<Addon> ret = new();
      var dirs = System.IO.Directory.GetDirectories(communityFolder);
      foreach (var dir in dirs)
      {
        Addon addon = ScanAddon(dir);
        ret.Add(addon);
      }
      return ret;
    }

    private Addon ScanAddon(string addonFolder)
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
