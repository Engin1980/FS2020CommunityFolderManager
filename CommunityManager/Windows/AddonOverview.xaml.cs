using CommunityManager.Controls;
using CommunityManagerLib;
using CommunityManagerLib.Addons;
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
      new Input(data).ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonInfo.State.CustomTitle = data.Value.Trim();
    }

    private void btnAssignTags_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonStates.SelectedItem is not AddonInfo addonInfo) return;

      var addonInfos = lstAddonStates.SelectedItems.Cast<AddonInfo>().ToList();
      if (addonInfos.Count > 1)
        if (Message.ShowDialog(
          "Adjust multiple items?",
          $"There are {addonInfos.Count} items selected. Adjust tags to all of them?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      UpdateTags(addonInfos);
    }

    private void UpdateTags(List<AddonInfo> addonInfos)
    {
      Trace.Assert(addonInfos.Count > 0);

      var tmp = (BindingList<AddonInfo>)this.DataContext!;
      BindingList<TagEditor.CheckItem> tags = Project.GetAllTags()
        .Select(q => new TagEditor.CheckItem(q, addonInfos.First().State.Tags.Contains(q)))
        .ToBindingList();

      TagEditor.Data data = new()
      {
        Tags = tags
      };
      new TagEditor(data).ShowDialog();
      if (data.DialogResult == Types.DialogResult.Cancel) return;

      var newTags = data.Tags.Where(q => q.IsChecked).Select(q => q.Label);
      addonInfos.ForEach(q => q.State.Tags = newTags.ToList());
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

    private void TagPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      TagPanel panel = (TagPanel)sender;
      string tag = (string)panel.Tag;
      var addonInfos = this.Project.Addons.Where(q => q.Addon.Folder == tag).ToList();
      Trace.Assert(addonInfos.Count == 1);
      UpdateTags(addonInfos);
    }
  }
}
