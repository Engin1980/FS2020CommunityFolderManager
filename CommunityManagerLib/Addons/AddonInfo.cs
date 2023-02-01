using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManagerLib.Addons
{
  public class AddonInfo : NotifyPropertyChangedBase
  {
    public AddonInfo(Addon addon, State state)
    {
      Addon = addon ?? throw new ArgumentNullException(nameof(addon));
      State = state ?? throw new ArgumentNullException(nameof(state));
      State.PropertyChanged += State_PropertyChanged;
    }

    private void State_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(State.CustomTitle))
        InvokePropertyChanged(nameof(DisplayTitle));
    }

    public Addon Addon
    {
      get => base.GetProperty<Addon>(nameof(Addon))!;
      set => base.UpdateProperty(nameof(Addon), value);
    }

    public State State
    {
      get => base.GetProperty<State>(nameof(State))!;
      set => base.UpdateProperty(nameof(State), value);
    }

    public string DisplayCreator
    {
      get
      {
        string ret;
        if (string.IsNullOrWhiteSpace(Addon.Creator))
        {
          if (string.IsNullOrWhiteSpace(Addon.Manufacturer))
            ret = "?";
          else
            ret = Addon.Manufacturer;
        }
        else
        {
          if (string.IsNullOrWhiteSpace(Addon.Manufacturer))
            ret = Addon.Creator;
          else
            ret = $"{Addon.Creator} / {Addon.Manufacturer}";
        }
        return ret;
      }
    }

    public string DisplayTitle
    {
      get
      {
        string ret;
        if (string.IsNullOrWhiteSpace(State.CustomTitle) == false)
          ret = State.CustomTitle;
        else if (string.IsNullOrWhiteSpace(Addon.ManifestTitle) == false)
          ret = Addon.ManifestTitle;
        else
          ret = Addon.Folder;
        return ret;
      }
    }
  }
}
