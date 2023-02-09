using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using CommunityManagerLib.StartupConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public class RunExecutor
  {
    public RunExecutor() { }

    public Dictionary<SingleAddonView, bool> AnalyseAddons(Project project, StartupConfiguration startupConfiguration)
    {
      Dictionary<SingleAddonView, bool> ret = new();

      foreach (var addonInfo in project.Addons.OfType<SingleAddonView>())
      {
        var availableTags = addonInfo.Addon.State.Tags;
        var requiredTags = startupConfiguration.Tags;
        ret[addonInfo] = IsTagMatch(requiredTags, availableTags);
      }

      foreach (var addonInfo in project.Addons.OfType<GroupAddonView>())
      {
        var availableTags = addonInfo.Addons.First().Addon.State.Tags;
        var requiredTags = startupConfiguration.Tags;
        foreach (var subaddonInfo in addonInfo.Addons)
        {
          ret[subaddonInfo] = IsTagMatch(requiredTags, availableTags);
        }
      }

      return ret;
    }

    public Dictionary<Program, bool> AnalysePrograms(Project project, StartupConfiguration startupConfiguration)
    {
      Dictionary<Program, bool> ret = new();

      foreach (var program in project.Programs)
      {
        var availableTags = program.Tags;
        var requiredTags = startupConfiguration.Tags;
        ret[program] = IsTagMatch(requiredTags, availableTags);
      }

      return ret;
    }

    private bool IsTagMatch(List<string> requiredTags, List<string> availableTags)
    {
      bool ret = requiredTags.Intersect(availableTags).Count() > 0;
      return ret;
    }
  }
}
