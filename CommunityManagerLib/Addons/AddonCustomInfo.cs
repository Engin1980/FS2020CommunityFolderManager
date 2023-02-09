﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CommunityManagerLib.Addons
{
  public class AddonCustomInfo : NotifyPropertyChangedBase
  {
    public string? CustomTitle
    {
      get => GetProperty<string?>(nameof(CustomTitle));
      set => UpdateProperty(nameof(CustomTitle), value);
    }

    public string? GroupGuid
    {
      get => base.GetProperty<string?>(nameof(GroupGuid))!;
      set => base.UpdateProperty(nameof(GroupGuid), value);
    }

    [JsonIgnore]
    public List<string> Tags
    {
      get => GetProperty<List<string>>(nameof(Tags))!;
      set => UpdateProperty(nameof(Tags), value);
    }
  }
}
