using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Programs
{
  public class Program : NotifyPropertyChangedBase
  {
    public Program(string path)
    {
      this.Path = path;
      this.Tags = new();
      this.PropertyChanged += Program_PropertyChanged;
    }

    [JsonIgnore]
    public string FileName { get => System.IO.Path.GetFileName(this.Path); }

    private void Program_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Path))
        base.InvokePropertyChanged(nameof(FileName));
      else if (e.PropertyName == nameof(FileName) || e.PropertyName == nameof(CustomName))
        base.InvokePropertyChanged(nameof(DisplayTitle));
    }

    public string? CustomName
    {
      get => base.GetProperty<string>(nameof(CustomName));
      set => base.UpdateProperty(nameof(CustomName), value);
    }

    [JsonIgnore]
    public string DisplayTitle
    {
      get => CustomName ?? FileName;
    }

    public string Path
    {
      get => base.GetProperty<string>(nameof(Path))!;
      set => base.UpdateProperty(nameof(Path), value);
    }

    public int StartupDelay
    {
      get => base.GetProperty<int>(nameof(StartupDelay))!;
      set => base.UpdateProperty(nameof(StartupDelay), value);
    }

    public List<String> Tags
    {
      get => base.GetProperty<List<String>>(nameof(Tags))!;
      set => base.UpdateProperty(nameof(Tags), value);
    }
  }
}
