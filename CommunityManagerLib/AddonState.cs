using CommunityManagerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;

namespace CommunityManagerLib
{
  public class AddonState : NotifyPropertyChangedBase
  {
    public Addon Addon { get; }

    public string? CustomTitle
    {
      get => GetProperty<string?>(nameof(CustomTitle));
      set => UpdateProperty(nameof(CustomTitle), value);
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
        if (string.IsNullOrWhiteSpace(CustomTitle) == false)
          ret = CustomTitle;
        else if (string.IsNullOrWhiteSpace(Addon.ManifestTitle) == false)
          ret = Addon.ManifestTitle;
        else
          ret = Addon.Folder;
        return ret;
      }
    }

    public bool IsActive
    {
      get => GetProperty<bool>(nameof(IsActive));
      set => UpdateProperty(nameof(IsActive), value);
    }

    public bool IsNew
    {
      get => GetProperty<bool>(nameof(IsNew));
      set => UpdateProperty(nameof(IsNew), value);
    }

    public List<string> Tags { get => GetProperty<List<string>>(nameof(Tags))!; set => UpdateProperty(nameof(Tags), value); }

    public AddonState(Addon addon)
    {
      this.Tags = new() { "new"};
      this.Addon = addon ?? throw new ArgumentNullException(nameof(addon));
      base.PropertyChanged += AddonState_PropertyChanged;
    }

    private void AddonState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(DisplayTitle))
        base.InvokePropertyChanged(nameof(DisplayTitle));
    }

    public void SetTags(List<string> tags)
    {
      this.Tags = tags;
      base.InvokePropertyChanged(nameof(Tags));
    }
  }
}
