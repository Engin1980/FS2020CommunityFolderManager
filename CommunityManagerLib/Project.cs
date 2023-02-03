using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using CommunityManagerLib.StartupConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public class Project : NotifyPropertyChangedBase
  {
    private const string SETTINGS_FILE = "data_config.json";
    private const string PROGRAM_FILE = "data_programs.json";
    private const string ADDONS_FILE = "data_addons.json";
    private const string STARTUP_CONFIGURATIONS_FILE = "data_startup_configurations.json";

    public static Project Load(out List<string> issues)
    {
      issues = new();
      var s = LoadSettings(ref issues);
      var a = string.IsNullOrEmpty(s.CommunityFolderPath) == false
        ? LoadAddons(s.CommunityFolderPath, ADDONS_FILE, ref issues)
        : new();
      var p = LoadPrograms(ref issues);
      var sc = LoadStartupConfigurations(ref issues);
      Project ret = new()
      {
        Settings = s,
        Addons = a,
        Programs = p,
        StartupConfigurations = sc
      };
      return ret;
    }

    public static BindingList<AddonInfo> LoadAddons(string communityFolder, string jsonAddonStateFile,
      ref List<string> issues)
    {
      Scanner scanner = new Scanner();
      BindingList<AddonInfo> ret;
      try
      {
        Dictionary<string, State> stateDict = System.IO.File.Exists(ADDONS_FILE)
          ? Project.Deserialize<Dictionary<string, State>>(ADDONS_FILE)
          : new Dictionary<string, State>();
        ret = scanner.CreateAddonInfos(
            communityFolder, stateDict);
      }
      catch (Exception ex)
      {
        issues.Add($"Failed to load addons and their states from '{communityFolder}' and '{jsonAddonStateFile}'. " +
          $"Reason: {ex.Message}.");
        ret = new();
      }
      return ret;
    }

    public static BindingList<StartupConfiguration> LoadStartupConfigurations(ref List<string> issues)
    {
      var file = Path.GetFullPath(STARTUP_CONFIGURATIONS_FILE);
      List<StartupConfiguration> ret;
      if (File.Exists(file))
      {
        try
        {
          string txt = System.IO.File.ReadAllText(file);
          ret = JsonConvert.DeserializeObject<List<StartupConfiguration>>(txt)!;
        }
        catch (Exception ex)
        {
          issues.Add($"Failed to load {file}. Startup-configuration list will be empty. Error: {ex.Message}");
          ret = new();
        }
      }
      else
      {
        issues.Add($"'{file}' not found. Startup-configuration list will be empty.");
        ret = new();
      }

      return ret.ToBindingList();
    }

    public static BindingList<Program> LoadPrograms(ref List<string> issues)
    {
      var file = Path.GetFullPath(PROGRAM_FILE);
      List<Program> ret;
      if (File.Exists(file))
      {
        try
        {
          ret = Deserialize<List<Program>>(file);
        }
        catch (Exception ex)
        {
          issues.Add($"Failed to load {file}. Program list will be empty. Error: {ex.Message}");
          ret = new();
        }
      }
      else
      {
        issues.Add($"'{file}' not found. Program list will be empty.");
        ret = new();
      }

      return ret.ToBindingList();
    }

    public static Settings LoadSettings(ref List<string> issues)
    {
      var file = Path.GetFullPath(SETTINGS_FILE);
      Settings ret;
      if (File.Exists(file))
      {
        try
        {
          ret = Deserialize<Settings>(file);
        }
        catch (Exception ex)
        {
          issues.Add($"Failed to load {file}. Settings & Addon list will be empty. Error: {ex.Message}");
          ret = new Settings()
          {
            CommunityFolderPath = ""
          };
        }
      }
      else
      {
        issues.Add($"'{file}' not found. Settings & Addon list will be empty.");
        ret = new Settings()
        {
          CommunityFolderPath = ""
        };
      }

      return ret;
    }

    public void Save()
    {
      throw new NotImplementedException("TODO");
    }

    public void ReloadAddons()
    {
      List<string> issues = new();
      this.Addons = Project.LoadAddons(this.Settings.CommunityFolderPath, ADDONS_FILE, ref issues);
    }

    public void SaveAddons()
    {
      Serialize(this.Addons.ToDictionary(
        q => q.Addon.Folder,
        q => q.State), ADDONS_FILE);
    }

    public void SavePrograms()
    {
      Serialize(this.Programs.ToList(), PROGRAM_FILE);
    }

    public void SaveStartupConfigurations()
    {
      Serialize(this.StartupConfigurations.ToList(), STARTUP_CONFIGURATIONS_FILE);
    }

    private void Serialize<T>(T obj, string fileName)
    {
      var txt = JsonConvert.SerializeObject(obj, Formatting.Indented);
      var tmpFile = System.IO.Path.GetTempFileName();

      try
      {
        System.IO.File.WriteAllText(tmpFile, txt);
        System.IO.File.Copy(tmpFile, fileName, true);
        System.IO.File.Delete(tmpFile);
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to save to '{fileName}' ('{tmpFile}').", ex);
      }
    }

    private static T Deserialize<T>(string fileName)
    {
      T ret;

      try
      {
        var txt = System.IO.File.ReadAllText(fileName);
        ret = JsonConvert.DeserializeObject<T>(txt)!;
      }
      catch (Exception ex)
      {
        throw new ApplicationException($"Failed to load from '{fileName}'.", ex);
      }

      return ret;
    }

    public void ReloadPrograms()
    {
      List<string> issues = new();
      this.Programs = Project.LoadPrograms(ref issues);
    }

    public void ReloadStartupConfigurations()
    {
      List<string> issues = new();
      this.StartupConfigurations = Project.LoadStartupConfigurations(ref issues);
    }

    public void ReloadSettings()
    {
      List<string> issues = new();
      this.Settings = Project.LoadSettings(ref issues);
    }

    public void SaveSettings()
    {
      Serialize(this.Settings, SETTINGS_FILE);
    }

    public static Project CreateEmpty()
    {
      Project ret = new Project()
      {
        Addons = new(),
        Programs = new(),
        Settings = new(),
        StartupConfigurations = new()
      };
      return ret;
    }

    public List<string> GetAllTags()
    {
      List<string> ret =
        this.Addons
        .SelectMany(q => q.State.Tags)
        .Union(
          this.Programs.SelectMany(p => p.Tags))
        .Union(
          this.StartupConfigurations.SelectMany(p => p.Tags))
        .Distinct()
        .OrderBy(q => q)
        .ToList();

      return ret;
    }

    private Project() { }

    public BindingList<AddonInfo> Addons
    {
      get => base.GetProperty<BindingList<AddonInfo>>(nameof(Addons))!;
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
    public Settings Settings
    {
      get => base.GetProperty<Settings>(nameof(Settings))!;
      set => base.UpdateProperty(nameof(Settings), value);
    }

  }
}
