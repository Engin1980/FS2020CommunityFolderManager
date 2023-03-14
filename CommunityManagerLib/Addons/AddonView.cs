using CommunityManagerLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Addons
{
  public abstract class AddonView : NotifyPropertyChangedBase
  {
    public abstract string Author { get; }
    public abstract string SourceName { get; }
    public abstract List<string> Tags { get; set; }
    public abstract string Title { get; set; }
    public abstract bool IsActive { get; }
    public abstract bool IsNew { get; }
    public abstract bool IsGrouped { get; }
    public abstract string? Note { get; set; }
  }
}
