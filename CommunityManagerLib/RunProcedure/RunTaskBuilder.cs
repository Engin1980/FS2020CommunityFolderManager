using CommunityManagerLib.StartupConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.RunProcedure
{
  public class RunTaskBuilder
  {
    public ProgramRunTask.StartTimeReferenceValue StartTimeReferenceValue { get; private set; }
      = new ProgramRunTask.StartTimeReferenceValue();

    public List<RunTask> BuildTasks(Project project, StartupConfiguration startupConfiguration)
    {
      var includedAddons = new RunExecutor().AnalyseAddons(project, startupConfiguration.Tags);
      var includedPrograms = new RunExecutor().AnalysePrograms(project, startupConfiguration.Tags);

      List<RunTask> ret = new();

      var tmp = includedAddons.Select(q => new AddonRunTask(
        q.Key, q.Value, project.Settings.CommunityFolderPath, Settings.Settings.INACTIVE_ADDONS_SUBFOLDER));
      ret.AddRange(tmp);

      var xmp = includedPrograms
        .Where(q => q.Value)
        .OrderBy(q => q.Key.StartupDelay)
        .Select(q => new ProgramRunTask(q.Key, this.StartTimeReferenceValue));
      ret.AddRange(xmp);

      return ret;
    }

    public void EnsureFoldersExist(Project project)
    {
      var communityFolder = project.Settings.CommunityFolderPath;
      var inactiveCommunityFolder = System.IO.Path.Combine(communityFolder, Settings.Settings.INACTIVE_ADDONS_SUBFOLDER);

      if (System.IO.Directory.Exists(communityFolder) == false)
        throw new ApplicationException($"Community folder at '{communityFolder}' not found.");
      if (System.IO.Directory.Exists(inactiveCommunityFolder) == false)
      {
        try
        {
          System.IO.Directory.CreateDirectory(inactiveCommunityFolder);
        }
        catch (Exception ex)
        {
          throw new ApplicationException(
            $"Inactive community subfolder at '{inactiveCommunityFolder}' cannot be created.", ex);
        }
      }
    }
  }
}
