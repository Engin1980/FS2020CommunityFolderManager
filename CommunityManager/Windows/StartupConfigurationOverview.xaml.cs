using CommunityManager.Controls;
using CommunityManagerLib;
using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using CommunityManagerLib.StartupConfigurations;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
  /// Interaction logic for StartupConfigurationOverview.xaml
  /// </summary>
  public partial class StartupConfigurationOverview : Window
  {
    private Project Project { get; set; }
    public StartupConfigurationOverview()
    {
      InitializeComponent();
    }

    public StartupConfigurationOverview(Project project) : this()
    {
      this.Project = project;
      this.DataContext = project.StartupConfigurations;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      if (GuiUtils.SaveStartupConfigurations(this.Project, true) == GuiUtils.Result.Success)
        this.Close();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      StartupConfiguration sc = new StartupConfiguration()
      {
        Title = "New Startup Configuration"
      };
      if (lstStartupConfigurations.SelectedIndex >= 0 && lstStartupConfigurations.SelectedIndex < this.Project.StartupConfigurations.Count - 1)
        this.Project.StartupConfigurations.Insert(lstStartupConfigurations.SelectedIndex + 1, sc);
      else
        this.Project.StartupConfigurations.Add(sc);
      UpdateTitle(sc);
    }

    private void UpdateTitle(StartupConfiguration sc)
    {
      var data = new Input.Data("Custom title...", "Select title:", sc.Title)
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      new Input(data).ShowDialog();
      if (data.DialogResult != Types.DialogResult.Ok) return;

      sc.Title = data.Value;
    }

    private void UpdateTags(List<StartupConfiguration> programs)
    {
      Trace.Assert(programs.Count > 0);

      var tmp = (BindingList<StartupConfiguration>)this.DataContext!;
      BindingList<TagEditor.CheckItem> tags = Project.GetAllTags()
        .Select(q => new TagEditor.CheckItem(q, programs.First().Tags.Contains(q)))
        .ToBindingList();

      TagEditor.Data data = new()
      {
        Tags = tags
      };
      new TagEditor(data).ShowDialog();
      if (data.DialogResult == Types.DialogResult.Cancel) return;

      var newTags = data.Tags.Where(q => q.IsChecked).Select(q => q.Label);
      programs.ForEach(q => q.Tags = newTags.ToList());
      int selectedIndex = lstStartupConfigurations.SelectedIndex;
      lstStartupConfigurations.DataContext = null;
      lstStartupConfigurations.DataContext = this.DataContext; //TODO improve how here reset of binding is done
      lstStartupConfigurations.SelectedIndex = selectedIndex;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (Message.ShowDialog(
        this,
        "Delete confirmation",
        "Really delete all selected startup configurations?",
        Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.No) return;

      lstStartupConfigurations.SelectedItems
        .Cast<StartupConfiguration>()
        .ToList()
        .ForEach(q => Project.StartupConfigurations.Remove(q));
    }

    private void btnMoveUp_Click(object sender, RoutedEventArgs e)
    {
      int index = lstStartupConfigurations.SelectedIndex;
      if (index == 0) return;

      var selected = this.Project.StartupConfigurations[index];
      this.Project.StartupConfigurations.RemoveAt(index);
      this.Project.StartupConfigurations.Insert(index - 1, selected);
      lstStartupConfigurations.SelectedIndex = index - 1;
    }

    private void btnMoveDown_Click(object sender, RoutedEventArgs e)
    {
      int index = lstStartupConfigurations.SelectedIndex;
      if (index == this.Project.StartupConfigurations.Count - 1) return;

      var selected = this.Project.StartupConfigurations[index];
      this.Project.StartupConfigurations.RemoveAt(index);
      this.Project.StartupConfigurations.Insert(index + 1, selected);
      lstStartupConfigurations.SelectedIndex = index + 1;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      new MainWindow(this.Project).Show();
    }

    private void lblName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Label lbl = (Label)sender;
      var tag = (string)lbl.Tag;
      StartupConfiguration sc = Project.StartupConfigurations.Single(q => q.Title == tag);
      UpdateTitle(sc);
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      GuiUtils.ReloadStartupConfigurations(this.Project, true, true);
    }

    private void TagPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      TagPanel panel = (TagPanel)sender;
      string tag = (string)panel.Tag;
      var scs = this.Project.StartupConfigurations.Where(q => q.Title == tag).ToList();
      Trace.Assert(scs.Count == 1);
      UpdateTags(scs);
    }

    private void btnTitle_Click(object sender, RoutedEventArgs e)
    {
      if (lstStartupConfigurations.SelectedIndex < 0) return;
      StartupConfiguration sc = (StartupConfiguration)lstStartupConfigurations.SelectedItem;
      UpdateTitle(sc);

    }

    private void btnTags_Click(object sender, RoutedEventArgs e)
    {
      if (lstStartupConfigurations.SelectedIndex < 0) return;
      StartupConfiguration sc = (StartupConfiguration)lstStartupConfigurations.SelectedItem;
      UpdateTags(new List<StartupConfiguration>() { sc });
    }

    private void btnAnalyse_Click(object sender, RoutedEventArgs e)
    {
      StartupConfiguration sc = (StartupConfiguration)lstStartupConfigurations.SelectedItem;
      RunExecutor re = new();

      var addons = re.AnalyseAddons(Project, sc);
      var programs = re.AnalysePrograms(Project, sc);

      var includedAddons = addons.Where(q => q.Value).Select(q => q.Key).ToList();
      var excludedAddons = addons.Where(q => !q.Value).Select(q => q.Key).ToList();
      var includedPrograms = programs.Where(q => q.Value).Select(q => q.Key).ToList();
      var excludedPrograms = programs.Where(q => !q.Value).Select(q => q.Key).ToList();

      new StartupConfigurationAnalysis(
        includedAddons, excludedAddons, includedPrograms, excludedPrograms)
        .ShowDialog();
    }
  }
}
