using System;
using System.Collections.Generic;
using System.Windows.Markup;
using Newtonsoft.Json.Linq;

namespace CommunityManagerLib.Addons
{
  public class AddonManifestData
  {

    public string? Creator { get; private set; }

    public List<AddonDependency> Dependencies { get; private set; }

    public string ManifestTitle { get; private set; }

    public string? Manufacturer { get; private set; }

    public string? Type { get; private set; }

    private AddonManifestData() { }

    public static AddonManifestData Create(AddonSource addonSource)
    {
      AddonManifestData ret;
      try
      {
        string fileContent = System.IO.File.ReadAllText(addonSource.ManifestFilePath);
        JObject json = JObject.Parse(fileContent);
        string title = (string)json["title"]!;
        string? contentType = (string?)json["content_type"];
        string? manufacturer = (string?)json["manufacturer"];
        string? creator = (string?)json["creator"];
        JArray? deps = (JArray?)json["depenencies"];
        List<AddonDependency> dependencies = new();
        if (deps != null)
          foreach (var dep in deps)
          {
            AddonDependency ad = new AddonDependency()
            {
              Name = (string)dep["name"]!,
              PackageVersion = (string)dep["package_version"]!
            };
            dependencies.Add(ad);
          }

        ret = new()
        {
          Creator = creator,
          ManifestTitle = title,
          Manufacturer = manufacturer,
          Type = contentType,
          Dependencies = dependencies
        };
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to load manifest file from '{addonSource.ManifestFilePath}'.", ex);
        throw;
      }

      return ret;
    }
    public string? GetAuthor()
    {
      {
        string? ret;
        if (!string.IsNullOrWhiteSpace(this.Manufacturer))
          if (!string.IsNullOrWhiteSpace(this.Creator) && this.Manufacturer != this.Creator)
            ret = this.Manufacturer + ", " + this.Creator;
          else
            ret = this.Manufacturer;
        else
          ret = (!string.IsNullOrWhiteSpace(this.Creator)) ? this.Creator : null;
        return ret;
      }
    }
  }
}
