using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.StartupConfigurations
{
  public class StartupConfiguration : NotifyPropertyChangedBase
  {
    public StartupConfiguration()
    {
      this.Tags = new();
    }
    public string Title
    {
      get => base.GetProperty<string>(nameof(Title))!;
      set => base.UpdateProperty(nameof(Title), value);
    }

    public List<string> Tags
    {
      get => base.GetProperty<List<string>>(nameof(Tags))!;
      set
      {
        value.Sort();
        base.UpdateProperty(nameof(Tags), value);
      }
    }
  }
}
