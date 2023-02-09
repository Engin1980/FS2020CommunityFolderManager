using CommunityManager.Controls;
using CommunityManagerLib;
using CommunityManagerLib.Addons;
using CommunityManagerLib.Programs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
  /// Interaction logic for ProgramOverview.xaml
  /// </summary>
  public partial class ProgramOverview : Window
  {
    public ProgramOverview()
    {
      InitializeComponent();
    }

    public ProgramOverview(Project project) : this()
    {
      InitializeComponent();
      Project = project;
      this.DataContext = Project.Programs;
    }

    private Project Project { get; set; }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      GuiUtils.ReloadPrograms(this.Project,true, true);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      GuiUtils.SavePrograms(this.Project, true);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      new MainWindow(this.Project).Show();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      CommonOpenFileDialog dialog = new()
      {
        EnsureFileExists = true,
        EnsurePathExists = true,
        Multiselect = false,
        Title = "Select executable file..."
      };
      dialog.Filters.Add(new CommonFileDialogFilter("Executable file", "exe"));
      dialog.Filters.Add(new CommonFileDialogFilter("Batch file", "bat"));
      dialog.Filters.Add(new CommonFileDialogFilter("All files", "*"));
      var res = dialog.ShowDialog();
      if (res != CommonFileDialogResult.Ok) return;

      Program program = new(dialog.FileName);
      this.Project.Programs.Add(program);
    }

    private void lblName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var path = (string)((Label)sender).Tag;
      Program program = Project.Programs.Single(q => q.Path == path);
      UpdateCustomTitle(program);
    }

    private void UpdateCustomTitle(Program program)
    {
      var data = new Input.Data("Custom title...", "Select custom title (or set empty to delete the current one):", "")
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      new Input(data).ShowDialog();
      if (data.DialogResult != Types.DialogResult.Ok) return;


      if (data.Value.Trim() == "")
        program.CustomName = null;
      else
        program.CustomName = data.Value;
    }

    private void TagPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      TagPanel panel = (TagPanel)sender;
      string tag = (string)panel.Tag;
      Program program = this.Project.Programs.Single(q => q.Path == tag);
      UpdateTags(new List<Program>() { program });
    }

    private void UpdateTags(List<Program> programs)
    {
      Trace.Assert(programs.Count > 0);

      var tmp = (BindingList<Program>)this.DataContext!;
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
      int selectedIndex = lstPrograms.SelectedIndex;
      lstPrograms.DataContext = null;
      lstPrograms.DataContext = this.DataContext; //TODO improve how here reset of binding is done
      lstPrograms.SelectedIndex = selectedIndex;
    }

    private void btnCustomTitle_Click(object sender, RoutedEventArgs e)
    {
      if (lstPrograms.SelectedIndex < 0) return;
      Program program = (Program)lstPrograms.SelectedItem;
      UpdateCustomTitle(program);

    }

    private void btnTags_Click(object sender, RoutedEventArgs e)
    {
      if (lstPrograms.SelectedIndex < 0) return;
      Program program = (Program)lstPrograms.SelectedItem;
      UpdateTags(new List<Program>() { program });
    }

    private void txtStartupDelay_TextChanged(object sender, TextChangedEventArgs e)
    {
      TextBox txt = (TextBox)sender;
      bool fail = false;
      fail = int.TryParse(txt.Text, out int val) == false;
      fail |= val < 0;
      txt.Background = new SolidColorBrush(
        fail ? Colors.Red : SystemColors.WindowColor);
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (Message.ShowDialog(
        "Delete confirmation",
        "Really delete all selected programs?",
        Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.No) return;

      lstPrograms.SelectedItems
        .Cast<Program>()
        .ToList()
        .ForEach(q => Project.Programs.Remove(q));
    }
  }
}
