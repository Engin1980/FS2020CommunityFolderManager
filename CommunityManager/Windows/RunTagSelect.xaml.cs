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
    public class LBItem : NotifyPropertyChangedBase
    {

      public string Label
      {
        get { return base.GetProperty<string>(nameof(Label))!; }
        set { base.UpdateProperty(nameof(Label), value); }
      }

      public Visibility Visibility
      {
        get { return base.GetProperty<Visibility>(nameof(Visibility))!; }
        set { base.UpdateProperty(nameof(Visibility), value); }
      }
    }

    public class VM : NotifyPropertyChangedBase
    {
      public StartupConfiguration StartupConfiguration
      {
        get { return base.GetProperty<StartupConfiguration>(nameof(StartupConfiguration))!; }
        set { base.UpdateProperty(nameof(StartupConfiguration), value); }
      }

      public ObservableCollection<LBItem> EnabledTags
      {
        get { return base.GetProperty<ObservableCollection<LBItem>>(nameof(EnabledTags))!; }
        set { base.UpdateProperty(nameof(EnabledTags), value); }
      }

      public ObservableCollection<LBItem> DisabledTags
      {
        get { return base.GetProperty<ObservableCollection<LBItem>>(nameof(DisabledTags))!; }
        set { base.UpdateProperty(nameof(DisabledTags), value); }
      }

      public VM()
      {
        this.EnabledTags = new();
        this.DisabledTags = new();
      }

    }

    private readonly VM vm;
    private Action<List<string>> starter = null!;
    private List<string> originalEnabledTags = null!;
    private List<string> allTags = null!;
    private Project project = null!;
    public RunTagSelect()
    {
      InitializeComponent();
      this.DataContext = vm = new VM();
    }

    internal void Init(Project project, List<string> enabledTags, Action<List<string>> starter)
    {
      this.project = project;
      this.allTags = project.GetAllTags();
      this.originalEnabledTags = enabledTags;
      this.starter = starter;
      chkOnlyFavourites.IsChecked = this.project.Settings.OnlyFavouriteTags ? true : false;
      SetTags();
    }

    private void SetTags()
    {
      allTags
      .Where(q => originalEnabledTags.Contains(q) == false)
      .Select(q => new LBItem()
      {
        Label = q,
        Visibility = Visibility.Visible
      })
      .ForEach(q => this.vm.DisabledTags.Add(q));

      originalEnabledTags
        .Select(q => new LBItem()
        {
          Label = q,
          Visibility = Visibility.Visible
        })
        .ForEach(q => this.vm.EnabledTags.Add(q));
      ResetLBItemVisibility();
    }

    private void Label_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Label label = (Label)sender;
      LBItem tag = (LBItem)label.Tag;

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
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
      project.Settings.OnlyFavouriteTags = chkOnlyFavourites.IsChecked ?? false;
      AnalyseForFavouriteTags();
      GuiUtils.SaveSettings(project, false);
      GuiUtils.SaveFavouriteRunTags(project, false);
      List<string> enabledTags = this.vm.EnabledTags.Select(q => q.Label).ToList();
      this.starter(enabledTags);
    }

    private void AnalyseForFavouriteTags()
    {
      // those in enabled which were not enabled
      var newlyEnabled = this.vm.EnabledTags.Select(q => q.Label).Where(q => this.originalEnabledTags.Contains(q) == false).ToList();
      var newlyDisabled = this.vm.DisabledTags.Select(q => q.Label).Where(q => this.originalEnabledTags.Contains(q) == true).ToList();

      newlyEnabled.Where(q => this.project.FavouriteRunTags.Contains(q) == false).ForEach(q => this.project.FavouriteRunTags.Add(q));
      newlyDisabled.Where(q => this.project.FavouriteRunTags.Contains(q) == false).ForEach(q => this.project.FavouriteRunTags.Add(q));
    }

    private void chkOnlyFavourites_Click(object sender, RoutedEventArgs e)
    {
      ResetLBItemVisibility();
    }

    private void ResetLBItemVisibility()
    {
      if (chkOnlyFavourites.IsChecked ?? false)
      {
        this.vm.EnabledTags.ForEach(q => q.Visibility = project.FavouriteRunTags.Contains(q.Label) ? Visibility.Visible : Visibility.Collapsed);
        this.vm.DisabledTags.ForEach(q => q.Visibility = project.FavouriteRunTags.Contains(q.Label) ? Visibility.Visible : Visibility.Collapsed);
      }
      else
      {
        this.vm.EnabledTags.ForEach(q => q.Visibility = Visibility.Visible);
        this.vm.DisabledTags.ForEach(q => q.Visibility = Visibility.Visible);
      }
    }
  }
}
