﻿using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.RunProcedure
{
  internal class AddonRunTask : RunTask
  {
    public AddonRunTask(SingleAddonView addonInfo, bool shouldBeActive, string communityFolder, string inactiveAddonsSubfolder)
    {
      AddonInfo = addonInfo;
      ShouldBeActive = shouldBeActive;
      CommunityFolder = communityFolder;
      InactiveAddonsSubfolder = inactiveAddonsSubfolder;

      UpdateProperty(nameof(Title), "Addon: " + addonInfo.Title);
    }

    public SingleAddonView AddonInfo
    {
      get => base.GetProperty<SingleAddonView>(nameof(AddonInfo))!;
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

    private void RunInternalFolder(out RunTaskState resultState, out string resultText)
    {
      FolderAddonSource fas = (FolderAddonSource)AddonInfo.Addon.Source;
      string activeExpectedFolder = System.IO.Path.Combine(CommunityFolder, fas.FolderName);
      string inactiveExpectedFolder = System.IO.Path.Combine(CommunityFolder, InactiveAddonsSubfolder, fas.FolderName);

      bool isNowActive;
      if (System.IO.Directory.Exists(activeExpectedFolder))
        isNowActive = true;
      else if (System.IO.Directory.Exists(inactiveExpectedFolder))
        isNowActive = false;
      else
        throw new ApplicationException(
          $"Unable to find addon {AddonInfo.Title}/{fas.SourceName}.");

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

    private void RunInternalLink(out RunTaskState resultState, out string resultText)
    {
      LinkFileAddonSource lfas = (LinkFileAddonSource)AddonInfo.Addon.Source;
      string activeExpectedFile = System.IO.Path.Combine(CommunityFolder, lfas.SourceName);
      string inactiveExpectedFile = System.IO.Path.Combine(CommunityFolder, InactiveAddonsSubfolder, lfas.Folder);

      bool isNowActive;
      if (System.IO.File.Exists(activeExpectedFile))
        isNowActive = true;
      else if (System.IO.File.Exists(inactiveExpectedFile))
        isNowActive = false;
      else
        throw new ApplicationException(
          $"Unable to find addon {AddonInfo.Title}/{lfas.Source}.Insert");

      if (isNowActive != ShouldBeActive)
      {
        if (ShouldBeActive)
        {
          System.IO.File.Move(inactiveExpectedFile, activeExpectedFile);
          resultText = "Addon activated.";
        }
        else
        {
          System.IO.File.Move(activeExpectedFile, inactiveExpectedFile);
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

    protected override void RunInternal(out RunTaskState resultState, out string resultText)
    {
      if (AddonInfo.Addon.Source is FolderAddonSource)
        RunInternalFolder(out resultState, out resultText);
      else if (AddonInfo.Addon.Source is LinkFileAddonSource)
        RunInternalLink(out resultState, out resultText);
      else
        throw new NotImplementedException();
    }
  }
}
