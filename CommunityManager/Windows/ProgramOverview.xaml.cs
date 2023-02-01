using CommunityManagerLib;
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
  /// Interaction logic for ProgramOverview.xaml
  /// </summary>
  public partial class ProgramOverview : Window
  {
    public ProgramOverview()
    {
      InitializeComponent();
    }

    private Project Project { get; set; }

    public void Bind(Project project)
    {
      Project = project;
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      LoadData();
    }

    private void LoadData()
    {
      if (Message.ShowDialog(
        "Load",
        "You will loose all unsaved changes. Are you sure you would like to reload the data?",
        Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      this.Project.ReloadPrograms();
      Message.ShowDialog("Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      this.Project.SavePrograms();
      Message.ShowDialog("Saved.", "Changes have been saved.", Types.DialogResult.Ok);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      var f = new MainWindow();
      f.Bind(this.Project);
      f.Show();
      this.Close();
    }
  }
}
