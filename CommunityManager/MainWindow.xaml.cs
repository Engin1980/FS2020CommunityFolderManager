using CommunityManager.Types;
using CommunityManager.Windows;
using CommunityManagerLib;
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
      Button btn = (Button)sender;
      StartupConfiguration sc = (StartupConfiguration)btn.Tag;
      new Windows.Run(Project, sc).Show();
      this.Hide();
    }

    private void StartupConfigurations_ListChanged(object? sender, ListChangedEventArgs e)
    {
      RebuildGrid();
    }

    private void btnQuit_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void btnAddonOverview_Click(object sender, RoutedEventArgs e)
    {
      new AddonOverview(Project).Show();
      this.Hide();
    }

    private void LoadProject()
    {
      var p = Project.Load(out List<string> issues);
      if (issues.Count > 0)
      {
        string prompt = "There were issues during the app startup:\n\n" +
          string.Join("\n\n", issues.Select(q => "\t(*) " + q).ToList());
        var d = new Message.Data("FS2020 Community Manager ... Issues during start up.", prompt, Types.DialogResult.Ok)
        {
          WindowHeight = 400,
          WindowWidth = 600
        };
        this.Hide();
        new Message(d).ShowDialog();
        this.Show();
        p ??= Project.CreateEmpty();
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
  }
}
