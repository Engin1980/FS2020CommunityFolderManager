using CommunityManagerLib;
using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommunityManager.Windows
{
  /// <summary>
  /// Interaction logic for AddonOverview.xaml
  /// </summary>
  public partial class AddonOverview : Window
  {
    private Project Project { get; set; }
    public AddonOverview()
    {
      InitializeComponent();
    }

    public AddonOverview(Project project) : this()
    {
      this.Project = project;
      this.DataContext = project.Addons;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void LoadData()
    {
      if (Message.ShowDialog(
         "Load",
         "You will loose all unsaved changes. Are you sure you would like to reload the data?",
         Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      this.Project.ReloadAddons();
      Message.ShowDialog("Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
    }

    private void SaveData()
    {
      this.Project.SaveAddons();
      Message.ShowDialog("Saved.", "Changes have been saved.", Types.DialogResult.Ok);
    }

    private void btnCustomTitle_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonStates.SelectedItem is not AddonInfo addonInfo) return;

      UpdateCustomTitle(addonInfo);
    }

    private void UpdateCustomTitle(AddonInfo addonInfo)
    {
      Input.Data data = new(
        "Adjust Custom Title...",
        "Set the new custom title for the addon:",
        addonInfo.State.CustomTitle ?? addonInfo.Addon.ManifestTitle ?? ""
        )
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      Input frmInput = new();
      frmInput.Bind(data);
      frmInput.ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonInfo.State.CustomTitle = data.Value.Trim();
    }

    private void btnAssignTags_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonStates.SelectedItem is not AddonInfo addonInfo) return;

      UpdateTags(addonInfo);
    }

    private void UpdateTags(AddonInfo addonInfo)
    {
      var tmp = (BindingList<AddonInfo>)this.DataContext!;
      BindingList<TagEditor.CheckItem> tags = tmp
        .SelectMany(q => q.State.Tags)
        .Distinct()
        .OrderBy(q => q)
        .Select(q => new TagEditor.CheckItem(q, false))
        .ToBindingList();

      TagEditor.Data data = new()
      {
        Tags = tags
      };
      new TagEditor(data).ShowDialog();
      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonInfo.State.Tags = data.Tags.Where(q => q.IsChecked).Select(q => q.Label).ToList();
      int selectedIndex = lstAddonStates.SelectedIndex;
      lstAddonStates.DataContext = null;
      lstAddonStates.DataContext = this.DataContext; //TODO improve how here reset of binding is done
      lstAddonStates.SelectedIndex = selectedIndex;
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      LoadData();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      SaveData();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {

      this.Close();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      new MainWindow(this.Project).Show();
    }

    private void lblDisplayTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Label label = (Label)sender;
      string tag = (string)label.Tag;
      AddonInfo addonInfo = this.Project.Addons.Single(q => q.Addon.Folder == tag);
      UpdateCustomTitle(addonInfo);

    }

    private void lblTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Label label = (Label)sender;
      string tag = (string)label.Tag;
      AddonInfo addonInfo = this.Project.Addons.Single(q => q.Addon.Folder == tag);
      UpdateTags(addonInfo);
    }
  }
}
