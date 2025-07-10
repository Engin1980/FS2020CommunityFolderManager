using System;
using System.Collections.Generic;
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

    public override string SourceName => this.Addon.Source.SourceName;

    public override string ToString() => $"{this.Title} (addon)";

    public override List<string> Tags
    {
      get => this.Addon.State.Tags;
      set => this.Addon.State.Tags = value;
    }

    public override string Title
    {
      get => this.Addon.State.CustomTitle ?? this.Addon.Source.SourceName;
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

    public override bool IsActive => this.Addon.IsActive;
    public override bool IsNew => this.Addon.IsNew;
    public override bool IsGrouped => false;
    public override string? Note
    {
      get => this.Addon.State.Note;
      set
      {
        this.Addon.State.Note = value;
        base.InvokePropertyChanged(nameof(Note));
      }
    }

    public override DateTime CreationDateTime => this.Addon.Manifest.CreationDateTime;
  }
}
