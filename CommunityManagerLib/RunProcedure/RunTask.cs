using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.RunProcedure
{
  public abstract class RunTask : NotifyPropertyChangedBase
  {
    public enum RunTaskState
    {
      Ready,
      Running,
      Done,
      Failed
    }

    public RunTask()
    {
      State = RunTaskState.Ready;
    }

    public RunTaskState State
    {
      get => base.GetProperty<RunTaskState>(nameof(State))!;
      private set
      {
        base.UpdateProperty(nameof(State), value);
        this.StateChanged?.Invoke(this);
      }
    }

    public abstract string Title { get; }


    public string? ResultText
    {
      get => base.GetProperty<string?>(nameof(ResultText))!;
      private set => base.UpdateProperty(nameof(ResultText), value);
    }

    public delegate void StateChangedDelegate(RunTask runTask);
    public event StateChangedDelegate? StateChanged;

    public void Run()
    {
      this.State = RunTaskState.Running;
      try
      {
        this.RunInternal(out RunTaskState resultState, out string resultText);
        this.ResultText = resultText;
        this.State = resultState;
      }
      catch (Exception ex)
      {
        this.ResultText = $"Task crashed. Reason: {ex.Message}";
        this.State = RunTaskState.Failed;
      }
    }

    protected abstract void RunInternal(out RunTaskState resultState, out string resultText);
  }
}
