using CommunityManagerLib.Programs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.RunProcedure
{
  public class ProgramRunTask : RunTask
  {
    public class StartTimeReferenceValue
    {
      public DateTime Value { get; set; }
    }

    public ProgramRunTask(Program program, StartTimeReferenceValue strv)
    {
      Program = program ?? throw new ArgumentNullException(nameof(program));
      this.startTimeReferenceValue = strv ?? throw new ArgumentNullException(nameof(strv));
    }

    private StartTimeReferenceValue startTimeReferenceValue;
    private Program Program { get; set; }

    public override string Title => $"Run '{Program.DisplayTitle}' after {Program.StartupDelay} secs";

    protected override void RunInternal(out RunTaskState resultState, out string resultText)
    {
      TimeSpan delayLeft = (startTimeReferenceValue.Value.AddSeconds(Program.StartupDelay)) - DateTime.Now;
      if (delayLeft.TotalMilliseconds > 0)
        System.Threading.Thread.Sleep((int)delayLeft.TotalMilliseconds);

      ProcessStartInfo psi;
      if (Program.IsDirectlyExecutable)
        psi = new()
        {
          FileName = Program.Path,
          WorkingDirectory = System.IO.Path.GetDirectoryName(Program.Path),
          Arguments = Program.Arguments
        };
      else
      {
        psi = new()
        {
          FileName = "start",
          WorkingDirectory = System.IO.Path.GetDirectoryName(Program.Path),
          Arguments = $"{Program.Path} \"{Program.Arguments}\""
        };
      }
      Process p = new()
      {
        StartInfo = psi
      };
      p.Start();

      resultState = RunTaskState.Done;
      resultText = "Started at " + DateTime.Now;
    }
  }
}
