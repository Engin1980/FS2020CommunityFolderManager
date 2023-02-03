using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.RunProcedure
{
  internal class AddonRunTask : RunTask
  {
    public AddonRunTask(AddonInfo addonInfo, bool shouldBeActive, string communityFolder, string inactiveAddonsSubfolder)
    {
      AddonInfo = addonInfo;
      ShouldBeActive = shouldBeActive;
      CommunityFolder = communityFolder;
      InactiveAddonsSubfolder = inactiveAddonsSubfolder;

      UpdateProperty(nameof(Title), "Addon: " + addonInfo.DisplayTitle);
    }

    public AddonInfo AddonInfo
    {
      get => base.GetProperty<AddonInfo>(nameof(AddonInfo))!;
      set => base.UpdateProperty(nameof(AddonInfo), value);
    }

    public override string Title
    {
      get => GetProperty<string>(nameof(Title))!;
    }


    public bool ShouldBeActive
    {
      get => base.GetProperty<bool>(nameof(ShouldBeActive))!;
      set => base.UpdateProperty(nameof(ShouldBeActive), value);
    }

    public string CommunityFolder { get; set; }
    public string InactiveAddonsSubfolder { get; set; }

    protected override void RunInternal(out RunTaskState resultState, out string resultText)
    {
      string activeExpectedFolder = System.IO.Path.Combine(
        CommunityFolder, AddonInfo.Addon.Folder);
      string inactiveExpectedFolder = System.IO.Path.Combine(
        CommunityFolder, InactiveAddonsSubfolder, AddonInfo.Addon.Folder);

      bool isNowActive;
      if (System.IO.Directory.Exists(activeExpectedFolder))
        isNowActive = true;
      else if (System.IO.Directory.Exists(inactiveExpectedFolder))
        isNowActive = false;
      else
        throw new ApplicationException(
          $"Unable to find addon {AddonInfo.DisplayTitle}/{AddonInfo.Addon.Folder}.Insert");

      if (isNowActive != ShouldBeActive)
      {
        if (ShouldBeActive)
        {
          System.IO.Directory.Move(inactiveExpectedFolder, activeExpectedFolder);
          resultText = "Addon activated.";
        }
        else
        {
          System.IO.Directory.Move(activeExpectedFolder, inactiveExpectedFolder);
          resultText = "Addon deactivated.";
        }
        resultState = RunTaskState.Done;
      }
      else
      {
        resultState = RunTaskState.Done;
        resultText = "Addon " + (isNowActive ? "active" : "inactive") + ", no change needed.";
      }
    }
  }
}
