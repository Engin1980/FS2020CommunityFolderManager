using System;
using System.Windows.Documents;
using System.Collections.Generic;

namespace CommunityManagerLib.Addons
{
    public class Addon
    {
        public Addon(string folder)
        {
            Folder = folder;
        }

        public string Folder { get; set; }
        public string? Type { get; set; }
        public string? ManifestTitle { get; set; }
        public string? Manufacturer { get; set; }
        public string? Creator { get; set; }
        public List<AddonDependency> Dependencies { get; } = new();
    }
}
