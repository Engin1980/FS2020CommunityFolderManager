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

    public Dictionary<AddonInfo, bool> AnalyseAddons(Project project, StartupConfiguration startupConfiguration)
    {
      Dictionary<AddonInfo, bool> ret = new();

      foreach (var addonInfo in project.Addons)
      {
        var availableTags = addonInfo.State.Tags;
        var requiredTags = startupConfiguration.Tags;
        ret[addonInfo] = IsTagMatch(requiredTags, availableTags);
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
