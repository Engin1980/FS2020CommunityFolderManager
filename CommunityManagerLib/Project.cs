using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using CommunityManagerLib.StartupConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityManagerLib
{
  public class Project : NotifyPropertyChangedBase
  {
    private const string DATA_FOLDER = "data\\";
    private const string SETTINGS_FILE = "config.json";
    private const string PROGRAM_FILE = "programs.json";
    private const string ADDONS_FILE = "addons.json";
    private const string STARTUP_CONFIGURATIONS_FILE = "startup_configurations.json";

    private static class EJson
    {
      public static T Load<T>(string path)
      {
        T ret;
        string txt;
        try
        {
          txt = File.ReadAllText(path);
        }
        catch (Exception ex)
        {
          throw new ApplicationException($"Failed to read content of file {path}.", ex);
        }
        try
        {
          ret = JsonConvert.DeserializeObject<T>(txt);
        }
        catch (Exception ex)
        {
          throw new ApplicationException($"Failed to deserialize content of {path} into {typeof(T).Name}.", ex);
        }
        return ret;
      }

      public static void Save<T>(T obj, string fileName)
      {
        var txt = JsonConvert.SerializeObject(obj, Formatting.Indented);
        var tmpFile = Path.GetTempFileName();

        try
        {
          File.WriteAllText(tmpFile, txt);
        }
        catch (Exception ex)
        {
          throw new ApplicationException($"Failed to write data to temporal file '{tmpFile}'", ex);
        }

        try
        {
          File.Copy(tmpFile, fileName, true);
        }
        catch (Exception ex)
        {
          throw new ApplicationException($"Failed to copy data from temporal file '{tmpFile}' to target file '{fileName}'", ex);
        }
        finally
        {
          File.Delete(tmpFile);
        }
      }
    }

    #region Load/Save

    public static BindingList<AddonView> LoadAddons(string communityFolder, string jsonAddonStateFile,
        ref List<string> issues)
    {
      if (Directory.Exists(communityFolder) == false)
        throw new ApplicationException($"Unable to find community folder at '{communityFolder}'");

      Dictionary<string, AddonCustomInfo> customInfos;
      try
      {
        customInfos = EJson.Load<Dictionary<string, AddonCustomInfo>>(DATA_FOLDER + ADDONS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Unable to load addon custom infos from '{jsonAddonStateFile}'.", ex);
      }

      AddonInitializer nai = new AddonInitializer();
      List<AddonView> ret = nai.AnalyseAddons(communityFolder, customInfos, out List<string> errors);
      issues.AddRange(errors);
      return ret.ToBindingList(); //.OrderBy(q => q.Title).ToBindingList();
    }

    public void SaveAddons() //todo rename parameter (everywhere in code)
    {
      Dictionary<string, AddonCustomInfo> dict = new();

      Dictionary<string, AddonCustomInfo> a = this.Addons
        .OfType<SingleAddonView>()
        .ToDictionary(
          q => q.Addon.Source.SourceName,
          q => q.Addon.State);
      Dictionary<string, AddonCustomInfo> b = this.Addons
        .OfType<GroupAddonView>()
        .SelectMany(q => q.Addons)
        .ToDictionary(
          q => q.Addon.Source.SourceName,
          q => q.Addon.State);

      Trace.Assert(a.Keys.Intersect(b.Keys).Count() == 0);
      dict = a.Concat(b).ToDictionary(q => q.Key, q => q.Value);

      try
      {
        EJson.Save(dict, DATA_FOLDER + ADDONS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to save addon infos.", ex);
      }
    }

    public static BindingList<Program> LoadPrograms()
    {
      List<Program> ret;
      try
      {
        ret = EJson.Load<List<Program>>(DATA_FOLDER + PROGRAM_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to load {DATA_FOLDER + PROGRAM_FILE}.", ex);
      }

      return ret.ToBindingList();
    }

    public void SavePrograms()
    {
      try
      {
        EJson.Save(this.Programs.ToList(), DATA_FOLDER + PROGRAM_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to save programs to file '{DATA_FOLDER + PROGRAM_FILE}'.", ex);
      }
    }

    public static BindingList<StartupConfiguration> LoadStartupConfigurations()
    {
      List<StartupConfiguration> ret;
      try
      {
        ret = EJson.Load<List<StartupConfiguration>>(DATA_FOLDER + STARTUP_CONFIGURATIONS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to load {DATA_FOLDER + STARTUP_CONFIGURATIONS_FILE}.", ex);
      }

      return ret.ToBindingList();
    }

    public void SaveStartupConfigurations()
    {
      try
      {
        EJson.Save(this.StartupConfigurations.ToList(), DATA_FOLDER + STARTUP_CONFIGURATIONS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to store starupt configurations to '{DATA_FOLDER + STARTUP_CONFIGURATIONS_FILE}.", ex);
      }
    }

    public static Settings.Settings LoadSettings()
    {
      Settings.Settings ret;
      try
      {
        ret = EJson.Load<Settings.Settings>(DATA_FOLDER + SETTINGS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to load {DATA_FOLDER + SETTINGS_FILE}. Settings & Addon list will be empty.", ex);
      }

      return ret;
    }

    public void SaveSettings()
    {
      try
      {
        EJson.Save(this.Settings, DATA_FOLDER + SETTINGS_FILE);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to save settings to '{DATA_FOLDER + SETTINGS_FILE}'.", ex);
      }
    }

    #endregion

    #region Reloads

    public void ReloadAddons(out List<string> issues)
    {
      issues = new();
      this.Addons = Project.LoadAddons(this.Settings.CommunityFolderPath, DATA_FOLDER + ADDONS_FILE, ref issues);
    }

    public void ReloadPrograms()
    {
      this.Programs = Project.LoadPrograms();
    }

    public void ReloadStartupConfigurations()
    {
      this.StartupConfigurations = Project.LoadStartupConfigurations();
    }

    public void ReloadSettings()
    {
      this.Settings = Project.LoadSettings();
    }

    #endregion

    public Project()
    {
      this.Addons = new();
      this.Programs = new();
      this.Settings = new()
      {
        CommunityFolderPath = ""
      };
      this.StartupConfigurations = new();
    }

    public List<string> GetAllTags()
    {
      var a = this.Addons
        .Where(q => q is SingleAddonView)
        .Select(q => (SingleAddonView)q)
        .SelectMany(q => q.Addon.State.Tags);
      var b = this.Addons
        .Where(q => q is GroupAddonView)
        .Select(q => (GroupAddonView)q)
        .SelectMany(q => q.Addons.SelectMany(p => p.Addon.State.Tags));
      var c = this.Programs
        .SelectMany(q => q.Tags);
      var d = this.StartupConfigurations
        .SelectMany(q => q.Tags);

      List<string> ret = a.Union(b).Union(c).Union(d).Distinct().OrderBy(q => q).ToList();
      return ret;
    }

    public static bool AnyDataFileExists()
    {
      if (System.IO.Directory.Exists(DATA_FOLDER) == false)
        System.IO.Directory.CreateDirectory(DATA_FOLDER);
      bool ret =
        File.Exists(DATA_FOLDER + ADDONS_FILE)
        ||
        File.Exists(DATA_FOLDER + PROGRAM_FILE)
        ||
        File.Exists(DATA_FOLDER + STARTUP_CONFIGURATIONS_FILE)
        ||
        File.Exists(DATA_FOLDER + SETTINGS_FILE);
      return ret;
    }

    #region Properties

    public BindingList<AddonView> Addons
    {
      get => base.GetProperty<BindingList<AddonView>>(nameof(Addons))!;
      set => base.UpdateProperty(nameof(Addons), value);
    }

    public BindingList<Program> Programs
    {
      get => base.GetProperty<BindingList<Program>>(nameof(Programs))!;
      set => base.UpdateProperty(nameof(Programs), value);
    }
    public BindingList<StartupConfiguration> StartupConfigurations
    {
      get => base.GetProperty<BindingList<StartupConfiguration>>(nameof(StartupConfigurations))!;
      set => base.UpdateProperty(nameof(StartupConfigurations), value);
    }
    public Settings.Settings Settings
    {
      get => base.GetProperty<Settings.Settings>(nameof(Settings))!;
      set => base.UpdateProperty(nameof(Settings), value);
    }

    #endregion
  }
}
