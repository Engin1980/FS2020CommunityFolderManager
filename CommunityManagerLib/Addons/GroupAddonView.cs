using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace CommunityManagerLib.Addons
{
  public class GroupAddonView : AddonView
  {
    private string? _Note;

    public override string? Note { get => _Note; set => _Note = value; }
    public List<SingleAddonView> Addons { get; set; }

    public override string Author => string.Join(", ", Addons.Select(q => q.Addon.Manifest.GetAuthor()).Distinct());

    public override string SourceName => $"(group of {this.Addons.Count} addons)";

    public override List<string> Tags
    {
      get => this.Addons.First().Tags;
      set => this.Addons.ForEach(q => q.Tags = value);
    }

    public override string Title
    {
      get => this.Addons.First().Addon.State.GroupGuid!;
      set { 
        this.Addons.ForEach(q => q.Addon.State.GroupGuid = value); 
        this.UpdateProperty(nameof(this.Title), value);
      }
    }

    public GroupAddonView(IEnumerable<SingleAddonView> addons, string key)
    {
      Trace.Assert(addons is not null);
      Trace.Assert(addons!.Count() > 1);
      this.Addons = addons!.ToList();
      this.Addons.ForEach(q => q.Addon.State.GroupGuid = key);
    }

    public override bool IsActive => this.Addons.Any(q => q.IsActive);
    public override bool IsNew => this.Addons.Any(q => q.IsNew);
    public override bool IsGrouped => true;

    public override DateTime CreationDateTime => this.Addons
      .Min(q => q.Addon.Manifest.CreationDateTime);
  }
}
