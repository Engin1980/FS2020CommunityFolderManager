using CommunityManager.Types;
using CommunityManager.Windows;
using CommunityManagerLib;
using CommunityManagerLib.Addons;
using CommunityManagerLib.StartupConfigurations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommunityManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public Project Project { get; set; }
    private bool closingShouldQuitApplication = true;

    public MainWindow()
    {
      InitializeComponent();
    }

    public MainWindow(Project project) : this()
    {
      this.Project = project;
      this.Project.StartupConfigurations.ListChanged += StartupConfigurations_ListChanged;
      RebuildGrid();
    }

    private void RebuildGrid()
    {
      int cnt = this.Project.StartupConfigurations.Count;
      int rowCount = cnt <= 1 ? 1 : cnt <= 4 ? 2 : cnt <= 9 ? 3 : 4;
      int colCount = cnt <= 2 ? 1 : cnt <= 4 ? 2 : cnt <= 9 ? 3 : 4;

      grd.Children.Clear();
      grd.RowDefinitions.Clear();
      grd.ColumnDefinitions.Clear();

      for (int i = 0; i < rowCount; i++)
        grd.RowDefinitions.Add(new RowDefinition());
      for (int i = 0; i < colCount; i++)
        grd.ColumnDefinitions.Add(new ColumnDefinition());

      for (int i = 0; i < cnt; i++)
      {
        int rowIndex = i / colCount;
        int colIndex = i % colCount;
        StartupConfiguration sc = Project.StartupConfigurations[i];
        Button btn = new Button()
        {
          Name = "btnStartSc",
          Content = sc.Title,
          Tag = sc
        };
        btn.Click += btnStartSc_Click;

        Grid.SetRow(btn, rowIndex);
        Grid.SetColumn(btn, colIndex);
        grd.Children.Add(btn);
      }
    }

    private void btnStartSc_Click(object sender, RoutedEventArgs e)
    {
      this.closingShouldQuitApplication = false;
      Button btn = (Button)sender;
      StartupConfiguration sc = (StartupConfiguration)btn.Tag;
      if (sc.AskTagsOnRun)
      {
        RunTagSelect frmTagSelect = new RunTagSelect();

        var allTags = Project.GetAllTags();
        frmTagSelect.Init(sc.Tags, allTags, (tags) =>
        {
          new Windows.Run(Project, sc, tags).Show();
          frmTagSelect.Hide();
        });
        frmTagSelect.Show();
        this.Hide();
      }
      else
      {
        new Windows.Run(Project, sc, sc.Tags).Show();
        this.Hide();
      }
    }

    private void StartupConfigurations_ListChanged(object? sender, ListChangedEventArgs e)
    {
      RebuildGrid();
    }

    private void btnQuit_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnAddonOverview_Click(object sender, RoutedEventArgs e)
    {
      new AddonOverview(Project).Show();
      this.Hide();
    }

    private void LoadProject()
    {
      Project p;
      if (!Project.AnyDataFileExists())
      {
        Message.ShowDialog(
          this,
          "No data exists.",
          "There are no saved config data. Probably new app instalation? A new empty project will be created.\n\n" +
          "Remember to set FS2020 Community folder location in settings.",
          Types.DialogResult.Ok);
        p = new Project()
        {
          Addons = new BindingList<CommunityManagerLib.Addons.AddonView>(),
          Programs = new BindingList<CommunityManagerLib.Programs.Program>(),
          Settings = new CommunityManagerLib.Settings.Settings(),
          StartupConfigurations = new BindingList<StartupConfiguration>()
        };
        GuiUtils.SaveSettings(p, false);
        GuiUtils.SaveAddons(p, false);
        GuiUtils.SavePrograms(p, false);
        GuiUtils.SaveStartupConfigurations(p, false);
        GuiUtils.SaveFavouriteRunTags(p, false);
      }
      else
      {
        p = new Project();
        GuiUtils.ReloadSettings(p, false, false);
        GuiUtils.ReloadAddons(p, false, false);
        GuiUtils.ReloadPrograms(p, false, false);
        GuiUtils.ReloadStartupConfigurations(p, false, false);
        GuiUtils.ReloadFavouriteRunTags(p, false, false);
      }
      this.Project = p;
      RebuildGrid();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.Project == null)
      {
        LoadProject();
      }
    }

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
      new SettingsOverview(Project).Show();
      this.Hide();
    }

    private void btnProgramOverview_Click(object sender, RoutedEventArgs e)
    {
      new ProgramOverview(this.Project).Show();
      this.Hide();
    }

    private void btnStartupConfigOverview_Click(object sender, RoutedEventArgs e)
    {
      new StartupConfigurationOverview(this.Project).Show();
      this.Hide();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      if (this.closingShouldQuitApplication)
        Application.Current.Shutdown();
    }
  }
}
