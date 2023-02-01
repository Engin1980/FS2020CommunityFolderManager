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
    public string Title
    {
      get => base.GetProperty<string>(nameof(Title))!;
      set => base.UpdateProperty(nameof(Title), value);
    }

    public BindingList<string> Tags
    {
      get => base.GetProperty<BindingList<string>>(nameof(Tags))!;
      set => base.UpdateProperty(nameof(Tags), value);
    }
  }
}
