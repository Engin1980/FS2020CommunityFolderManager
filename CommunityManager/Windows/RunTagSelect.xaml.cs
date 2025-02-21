using CommunityManagerLib;
using CommunityManagerLib.StartupConfigurations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  /// Interaction logic for RunTagSelect.xaml
  /// </summary>
  public partial class RunTagSelect : Window
  {
    public class VM : NotifyPropertyChangedBase
    {
      public StartupConfiguration StartupConfiguration
      {
        get { return base.GetProperty<StartupConfiguration>(nameof(StartupConfiguration))!; }
        set { base.UpdateProperty(nameof(StartupConfiguration), value); }
      }

      public ObservableCollection<string> EnabledTags
      {
        get { return base.GetProperty<ObservableCollection<string>>(nameof(EnabledTags))!; }
        set { base.UpdateProperty(nameof(EnabledTags), value); }
      }

      public ObservableCollection<string> DisabledTags
      {
        get { return base.GetProperty<ObservableCollection<string>>(nameof(DisabledTags))!; }
        set { base.UpdateProperty(nameof(DisabledTags), value); }
      }

      public VM()
      {
        this.EnabledTags = new();
        this.DisabledTags = new();
      }

    }

    private readonly VM vm;
    private Action<List<string>> starter;
    private List<string> originalEnabledTags;
    private List<string> allTags;
    private Project project;
    public RunTagSelect()
    {
      InitializeComponent();
      this.DataContext = vm = new VM();
    }

    internal void Init(Project project, List<string> enabledTags, Action<List<string>> starter)
    {
      this.allTags = project.GetAllTags();
      this.originalEnabledTags = enabledTags;
      this.starter = starter;
      SetTags();
    }

    private void SetTags()
    {
      if (chkOnlyFavourites.IsChecked)
      {

      }
      else
      {
        allTags.Where(q => originalEnabledTags.Contains(q) == false).ForEach(q => this.vm.DisabledTags.Add(q));
        originalEnabledTags.ForEach(q => this.vm.EnabledTags.Add(q));
      }
    }

    private void Label_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Label label = (Label)sender;
      string tag = (string)label.Tag;

      if (vm.EnabledTags.Contains(tag))
      {
        vm.EnabledTags.Remove(tag);
        vm.DisabledTags.Add(tag);
      }
      else if (vm.DisabledTags.Contains(tag))
      {
        vm.DisabledTags.Remove(tag);
        vm.EnabledTags.Add(tag);
      }
      if (project.FavouriteRunTags.Contains(tag) == false)
        project.FavouriteRunTags.Add(tag);
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
      GuiUtils.SaveFavouriteRunTags(project, false);
      this.starter(this.vm.EnabledTags.ToList());
    }

    private void chkOnlyFavourites_Click(object sender, RoutedEventArgs e)
    {
      SetTags();
    }
  }
}
