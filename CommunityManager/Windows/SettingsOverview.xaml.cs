using CommunityManagerLib;
using Microsoft.WindowsAPICodePack.Dialogs;
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
  /// Interaction logic for SettingsOverview.xaml
  /// </summary>
  public partial class SettingsOverview : Window
  {
    private Project Project { get; set; }
    private bool isSettingsUpdated = false;
    public SettingsOverview()
    {
      InitializeComponent();
    }

    public void Bind(Project project)
    {
      this.Project = project;
      this.Project.Settings.PropertyChanged += Settings_PropertyChanged;
      this.DataContext = this.Project.Settings;
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Project.Settings.CommunityFolderPath)) isSettingsUpdated = true;
    }

    private void SaveData()
    {
      this.Project.SaveSettings();
      Message.ShowDialog("Saved.", "Changes have been saved.", Types.DialogResult.Ok);
    }

    private void LoadData()
    {
      if (Message.ShowDialog(
        "Load",
        "You will loose all unsaved changes. Are you sure you would like to reload the data?",
        Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      this.Project.ReloadSettings();
      this.Project.Settings.PropertyChanged += Settings_PropertyChanged;
      Message.ShowDialog("Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      SaveData();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      
      this.Close();
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      LoadData();
    }

    private void btnBrowseCommunityFolder_Click(object sender, RoutedEventArgs e)
    {
      CommonOpenFileDialog dialog = new()
      {
        IsFolderPicker=true,
        InitialDirectory = Project.Settings.CommunityFolderPath
      };
      if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
      {
        Project.Settings.CommunityFolderPath = dialog.FileName;
      }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      if (isSettingsUpdated)
      {
        if (Message.ShowDialog("Reload addons?",
          "Community folder path seems to be updated. Reload addons?",
          Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.Yes)
        {
          Project.ReloadAddons();
        }
      }

      var f = new MainWindow();
      f.Bind(this.Project);
      f.Show();
    }
  }
}
