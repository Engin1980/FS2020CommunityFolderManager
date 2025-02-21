using CommunityManagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityManager.Windows
{
  public static class GuiUtils
  {
    public enum Result
    {
      Success,
      Error
    }

    internal static void ReloadAddons(Project project, bool askForSure, bool showSuccessConfirmation)
    {
      if (askForSure)
        if (Message.ShowDialog(
           CurrentWindow,
           "Load",
           "You will loose all unsaved changes. Are you sure you would like to reload the data?",
           Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      try
      {
        project.ReloadAddons(out List<string> issues);
        if (issues.Count == 0 && showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
        else if (issues.Count > 0)
          Message.ShowDialog(CurrentWindow, "Reloaded with issues",
            "Changes have been reloaded. However, there were some issues:\n" + string.Join("\n\t", issues),
            Types.DialogResult.Ok);
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Reload failed.", "Changes have not been reloaded. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
      }
    }

    internal static void ReloadPrograms(Project project, bool askForSure, bool showSuccessConfirmation)
    {
      if (askForSure)
        if (Message.ShowDialog(
          CurrentWindow,
            "Load",
            "You will loose all unsaved changes. Are you sure you would like to reload the data?",
            Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      try
      {
        project.ReloadPrograms();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Reload failed.", "Changes have not been reloaded. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
      }
    }

    internal static void ReloadSettings(Project project, bool askForSure, bool showSuccessConfirmation)
    {
      if (askForSure)
        if (Message.ShowDialog(CurrentWindow, "Load",
          "You will loose all unsaved changes. Are you sure you would like to reload the data?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;
      try
      {
        project.ReloadSettings();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Load", "Changes have been reloaded.", Types.DialogResult.Ok);
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Failed to load settings.",
          "Settings was not loaded: " + ex.ToMessageString() + "\n\n" +
          "Settings are empty. Reset the FS2020 Configuration file. Addons WILL NOT be loaded too.",
          Types.DialogResult.Ok);
      }
    }

    internal static void ReloadStartupConfigurations(Project project, bool askForSure, bool showSuccessConfirmation)
    {
      if (askForSure)
        if (Message.ShowDialog(
          CurrentWindow,
          "Load",
          "You will loose all unsaved changes. Are you sure you would like to reload the data?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      try
      {
        project.ReloadStartupConfigurations();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Load", "Changes have been reloaded.", Types.DialogResult.Ok);
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Reload failed.", "Changes have not been reloaded. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
      }
    }

    internal static Result SaveAddons(Project project, bool showSuccessConfirmation)
    {
      try
      {
        project.SaveAddons();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Saved.", "Changes have been saved.", Types.DialogResult.Ok);
        return Result.Success;
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Save failed.", "Changes have not been saved. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
        return Result.Error;
      }
    }

    private static Window? CurrentWindow => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

    internal static Result SavePrograms(Project project, bool showSuccessConfirmation)
    {
      try
      {
        project.SavePrograms();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Saved.", "Changes have been saved.", Types.DialogResult.Ok);
        return Result.Success;
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Save failed.", "Changes have not been saved. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
        return Result.Error;
      }
    }

    internal static Result SaveSettings(Project project, bool showSuccessConfirmation)
    {
      try
      {
        project.SaveSettings();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Saved.", "Changes have been saved.", Types.DialogResult.Ok);
        return Result.Success;
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Save failed.", "Changes have not been saved. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
        return Result.Error;
      }
    }

    internal static Result SaveStartupConfigurations(Project project, bool showSuccessConfirmation)
    {
      try
      {
        project.SaveStartupConfigurations();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Saved.", "Changes have been saved.", Types.DialogResult.Ok);
        return Result.Success;
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Save failed.", "Changes have not been saved. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
        return Result.Error;
      }
    }

    internal static void ReloadFavouriteRunTags(Project project, bool askForSure, bool showSuccessConfirmation)
    {
      if (askForSure)
        if (Message.ShowDialog(CurrentWindow, "Load",
          "You will loose all unsaved changes. Are you sure you would like to reload the data?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;
      try
      {
        project.ReloadFavouriteRunTags();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Load", "Changes have been reloaded.", Types.DialogResult.Ok);
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Failed to load settings.",
          "Settings was not loaded: " + ex.ToMessageString() + "\n\n" +
          "Settings are empty. Reset the FS2020 Configuration file. Addons WILL NOT be loaded too.",
          Types.DialogResult.Ok);
      }
    }

    internal static Result SaveFavouriteRunTags(Project project, bool showSuccessConfirmation)
    {
      try
      {
        project.SaveFavouriteRunTags();
        if (showSuccessConfirmation)
          Message.ShowDialog(CurrentWindow, "Saved.", "Changes have been saved.", Types.DialogResult.Ok);
        return Result.Success;
      }
      catch (Exception ex)
      {
        Message.ShowDialog(CurrentWindow, "Save failed.", "Changes have not been saved. Reason: " + ex.ToMessageString(),
          Types.DialogResult.Ok);
        return Result.Error;
      }
    }
  }
}
