using System;
using System.ComponentModel;

namespace CommunityManagerLib.Addons
{
  public class SingleAddonView : AddonView
  {
    public Addon Addon
    {
      get => base.GetProperty<Addon>(nameof(Addon))!;
      set => base.UpdateProperty(nameof(Addon), value);
    }

    public override string Author => this.Addon.Manifest.GetAuthor()!;

    public override string Source => this.Addon.Source.Source;

    public override BindingList<string> Tags => this.Addon.State.Tags.ToBindingList();

    public override string Title
    {
      get => this.Addon.State.CustomTitle ?? this.Addon.Source.Source;
      set
      {
        this.Addon.State.CustomTitle = value;
        base.InvokePropertyChanged(nameof(Title));
      }
    }

    public SingleAddonView(Addon addon)
    {
      Addon = addon ?? throw new ArgumentNullException(nameof(addon));
    }
  }
}
