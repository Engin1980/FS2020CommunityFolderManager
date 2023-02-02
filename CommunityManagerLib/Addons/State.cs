using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;

namespace CommunityManagerLib.Addons
{
  public class State : NotifyPropertyChangedBase
  {
    public string? CustomTitle
    {
      get => GetProperty<string?>(nameof(CustomTitle));
      set => UpdateProperty(nameof(CustomTitle), value);
    }

    [JsonIgnore]
    public bool IsActive
    {
      get => GetProperty<bool>(nameof(IsActive));
      set => UpdateProperty(nameof(IsActive), value);
    }

    [JsonIgnore]
    public bool IsNew
    {
      get => GetProperty<bool>(nameof(IsNew));
      set => UpdateProperty(nameof(IsNew), value);
    }

    public List<string> Tags { get => GetProperty<List<string>>(nameof(Tags))!; set => UpdateProperty(nameof(Tags), value); }

    public State()
    {
      Tags = new();
    }
  }
}
