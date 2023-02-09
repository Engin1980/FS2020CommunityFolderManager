using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommunityManager.Windows
{
  /// <summary>
  /// Interaction logic for StartupConfigurationAnalysis.xaml
  /// </summary>
  public partial class StartupConfigurationAnalysis : Window
  {
    public StartupConfigurationAnalysis()
    {
      InitializeComponent();
    }

    public StartupConfigurationAnalysis(
      List<AddonView> includedAddons,
      List<AddonView> excludedAddons,
      List<Program> includedPrograms,
      List<Program> excludedPrograms
      ) : this()
    {
      this.lstIncludedAddons.ItemsSource= includedAddons;
      this.lstExcludedAddons.ItemsSource = excludedAddons;
      this.lstIncludedPrograms.ItemsSource = includedPrograms;
      this.lstExcludedPrograms.ItemsSource = excludedPrograms;
    }
  }
}
