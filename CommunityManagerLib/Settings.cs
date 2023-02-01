using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib
{
  public class Settings : NotifyPropertyChangedBase
  {

    public string CommunityFolderPath
    {
      get => base.GetProperty<string>(nameof(CommunityFolderPath))!;
      set => base.UpdateProperty(nameof(CommunityFolderPath), value);
    }
  }
}
