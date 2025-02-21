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

    public Dictionary<SingleAddonView, bool> AnalyseAddons(Project project, List<string> activeTags)
    {
      Dictionary<SingleAddonView, bool> ret = new();

      foreach (var addonInfo in project.Addons.OfType<SingleAddonView>())
      {
        var availableTags = addonInfo.Addon.State.Tags;
        var requiredTags = activeTags;
        ret[addonInfo] = IsTagMatch(requiredTags, availableTags);
      }

      foreach (var addonInfo in project.Addons.OfType<GroupAddonView>())
      {
        var availableTags = addonInfo.Addons.First().Addon.State.Tags;
        var requiredTags = activeTags;
        foreach (var subaddonInfo in addonInfo.Addons)
        {
          ret[subaddonInfo] = IsTagMatch(requiredTags, availableTags);
        }
      }

      return ret;
    }

    public Dictionary<Program, bool> AnalysePrograms(Project project, List<string> activeTags)
    {
      Dictionary<Program, bool> ret = new();

      foreach (var program in project.Programs)
      {
        var availableTags = program.Tags;
        var requiredTags = activeTags;
        ret[program] = IsTagMatch(requiredTags, availableTags);
      }

      return ret;
    }

    private bool IsTagMatch(List<string> requiredTags, List<string> availableTags)
    {
      bool ret = availableTags.All(q => string.Compare(q, "off", true) != 0) 
        && (availableTags.Any(q => string.Compare(q, "on", true) == 0)
        || requiredTags.Intersect(availableTags).Any());
      return ret;
    }
  }
}
