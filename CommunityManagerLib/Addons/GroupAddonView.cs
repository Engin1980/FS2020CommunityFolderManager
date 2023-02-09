using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace CommunityManagerLib.Addons
{
  public class GroupAddonView : AddonView
  {

    public List<SingleAddonView> Addons { get; set; }

    public override string Author => string.Join(", ", Addons.Select(q => q.Addon.Manifest.GetAuthor()));

    public override string Source => $"(group of {this.Addons.Count} addons)";

    public override BindingList<string> Tags => this.Addons.First().Tags;

    public override string Title
    {
      get => this.Addons.First().Addon.State.GroupGuid!;
      set => this.Addons.ForEach(q => q.Addon.State.GroupGuid = value);
    }

    public GroupAddonView(List<SingleAddonView> addons, string key)
    {
      Trace.Assert(addons is not null);
      Trace.Assert(addons!.Count() > 1);
      this.Addons = addons!;
      this.Addons.ForEach(q => q.Addon.State.GroupGuid = key);
    }
  }
}
