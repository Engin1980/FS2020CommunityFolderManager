using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommunityManagerLib.Addons
{
  public class Addon : NotifyPropertyChangedBase
  {
    [JsonIgnore]
    public bool IsActive
    {
      get => GetProperty<bool>(nameof(IsActive));
      set => UpdateProperty(nameof(IsActive), value);
    }

    [JsonIgnore]
    public bool IsNew
    {
      get => GetProperty<bool>(nameof(IsNew));
      set => UpdateProperty(nameof(IsNew), value);
    }

    [JsonIgnore]
    public AddonManifestData Manifest { get; set; }

    [JsonIgnore]
    public AddonSource Source { get; set; }

    public AddonCustomInfo State { get; set; }

    public Addon(AddonSource source, AddonManifestData manifest, AddonCustomInfo state, bool isActive, bool isNew)
    {
      Source = source ?? throw new ArgumentNullException(nameof(source));
      Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
      State = state ?? throw new ArgumentNullException(nameof(state));
      IsActive = isActive;
      IsNew = isNew;
    }
  }
}
