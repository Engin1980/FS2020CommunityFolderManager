using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Settings
{
  public class Settings : NotifyPropertyChangedBase
  {
    public const string INACTIVE_ADDONS_SUBFOLDER = "__inactive";

    public Settings()
    {
      AutoCloseAfterRun = true;
      AutoCloseDelayInSeconds = 15;
    }

    public string CommunityFolderPath
    {
      get => GetProperty<string>(nameof(CommunityFolderPath))!;
      set => UpdateProperty(nameof(CommunityFolderPath), value);
    }


    public bool AutoCloseAfterRun
    {
      get => base.GetProperty<bool>(nameof(AutoCloseAfterRun))!;
      set => base.UpdateProperty(nameof(AutoCloseAfterRun), value);
    }


    public int AutoCloseDelayInSeconds
    {
      get => base.GetProperty<int>(nameof(AutoCloseDelayInSeconds))!;
      set => base.UpdateProperty(nameof(AutoCloseDelayInSeconds), value);
    }

    public bool OnlyFavouriteTags
    {
      get { return base.GetProperty<bool>(nameof(OnlyFavouriteTags))!; }
      set { base.UpdateProperty(nameof(OnlyFavouriteTags), value); }
    }
  }
}
