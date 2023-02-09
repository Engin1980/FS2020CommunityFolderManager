using CommunityManagerLib;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Addons
{
  public abstract class AddonView : NotifyPropertyChangedBase
  {

    public abstract string Author { get; }
    public abstract string Source { get; }
    public abstract BindingList<string> Tags { get; }
    public abstract string Title { get; set; }
  }
}
