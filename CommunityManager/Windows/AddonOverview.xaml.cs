using CommunityManager.Controls;
using CommunityManagerLib;
using CommunityManagerLib.Addons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
    private ListCollectionView listCollectionView;
    public AddonOverview()
    {
      InitializeComponent();
    }

    public AddonOverview(Project project) : this()
    {
      this.Project = project;
      this.listCollectionView = new(project.Addons);
      this.listCollectionView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
      this.DataContext = this.listCollectionView; //project.Addons;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void btnCustomTitle_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonViews.SelectedItem is not AddonView addonView) return;

      UpdateCustomTitle(addonView);
    }

    private void UpdateCustomTitle(AddonView addonView)
    {
      Input.Data data = new(
        "Adjust Custom Title...",
        "Set the new custom title for the addon:",
        addonView.Title
        )
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      new Input(data).ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonView.Title = data.Value.Trim();
    }

    private void UpdateNote(SingleAddonView addonView)
    {
      Input.Data data = new(
        "Adjust Note...",
        "Set the note for the addon:",
        addonView.Note
        )
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      new Input(data).ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonView.Note = data.Value.Trim();
    }

    private void btnAssignTags_Click(object sender, RoutedEventArgs e)
    {
      var addonInfos = lstAddonViews.SelectedItems.Cast<AddonView>().ToList();
      if (addonInfos.Count > 1)
        if (Message.ShowDialog(
          "Adjust multiple items?",
          $"There are {addonInfos.Count} items selected. Adjust tags to all of them?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      UpdateTags(addonInfos);
    }

    private void UpdateTags(List<AddonView> addonViews)
    {
      Trace.Assert(addonViews.Count > 0);

      BindingList<TagEditor.CheckItem> tags = Project.GetAllTags()
        .Select(q => new TagEditor.CheckItem(q, addonViews.First().Tags.Contains(q)))
        .ToBindingList();

      TagEditor.Data data = new()
      {
        Tags = tags
      };
      new TagEditor(data).ShowDialog();
      if (data.DialogResult == Types.DialogResult.Cancel) return;

      var newTags = data.Tags.Where(q => q.IsChecked).Select(q => q.Label);
      addonViews.ForEach(q => q.Tags = newTags.ToList());
      int selectedIndex = lstAddonViews.SelectedIndex;
      lstAddonViews.DataContext = null;
      lstAddonViews.DataContext = this.DataContext; //TODO improve how here reset of binding is done
      lstAddonViews.SelectedIndex = selectedIndex;
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      GuiUtils.ReloadAddons(this.Project, true, true);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      if (GuiUtils.SaveAddons(this.Project, true) == GuiUtils.Result.Success)
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
      AddonView addonInfo = this.Project.Addons.Single(q => q.SourceName == tag);
      UpdateCustomTitle(addonInfo);
    }

    private void TagPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      TagPanel panel = (TagPanel)sender;
      string tag = (string)panel.Tag;
      var addonViews = this.Project.Addons.Where(q => q.SourceName == tag).ToList();
      Trace.Assert(addonViews.Count == 1);
      UpdateTags(addonViews);
    }

    private void btnGroup_Click(object sender, RoutedEventArgs e)
    {
      var addonViews = this.lstAddonViews.SelectedItems.Cast<AddonView>().ToList();
      switch (addonViews.Count)
      {
        case 0:
          Message.ShowDialog("Unable.", "At least item must be selected.", Types.DialogResult.Ok);
          break;
        case 1:
          if (addonViews.Single() is not GroupAddonView)
            Message.ShowDialog("Unable.", "The only selected item is not grouped set of addons to be ungrouped.", Types.DialogResult.Ok);
          else
          {
            if (Message.ShowDialog("Really?", $"You are going ungroup {addonViews.Count} addons. Are you sure?",
              Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.Yes)
            {
              GroupAddonView gav = (GroupAddonView)addonViews.Single();
              var tmp = gav.Addons;
              int index = Project.Addons.IndexOf(gav);
              Project.Addons.Remove(gav);
              tmp.Reversed().ForEach(q => Project.Addons.Insert(index, q));
              Message.ShowDialog("Done", $"Addons ungrouped.");
            }
          }
          break;
        default:
          if (addonViews.Any(q => q is GroupAddonView))
            Message.ShowDialog("Unable.", "The selected set contains already grouped items. Cannot group grouped items again.", Types.DialogResult.Ok);
          else
          {
            if (Message.ShowDialog("Really?", $"You are going to group together {addonViews.Count} addons. Their tags will be intersected. Are you sure?",
              Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.Yes)
            {
              string groupName = $"{addonViews.First().Title} + {addonViews.Count - 1} other";
              Input.Data id = new("New group name", "Enter the name of the new group:", groupName);
              new Input(id).ShowDialog();
              if (id.DialogResult == Types.DialogResult.Ok)
                groupName = id.Value;
              var tmp = addonViews.Cast<SingleAddonView>();
              GroupAddonView gav = new(tmp, groupName);
              int index = this.Project.Addons.IndexOf(tmp.First());
              tmp.ForEach(q => this.Project.Addons.Remove(q));
              this.Project.Addons.Insert(index, gav);
              Message.ShowDialog("Done", $"Group with name '{gav.Title}' created.", Types.DialogResult.Ok);
            }
          }
          break;
      }
      listCollectionView.Refresh();
    }

    private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (txtFilter.Text.Trim().Length > 0)
      {
        Predicate<object> filter = o =>
          {
            AddonView av = (AddonView)o;
            string txt = txtFilter.Text.ToLower();
            bool ret = av.Title.ToLower().Contains(txt)
              || av.SourceName.ToLower().Contains(txt)
              || av.Tags.Any(q => q.ToLower().Contains(txt));
            return ret;
          };
        listCollectionView.Filter = filter;
      }
      else
        listCollectionView.Filter = null;
    }

    private void btnDescription_Click(object sender, RoutedEventArgs e)
    {
      AddonView addonInfo = (AddonView)lstAddonViews.SelectedItem;
      if (addonInfo is not SingleAddonView) return;
      UpdateNote((SingleAddonView)addonInfo);
    }

    private void txtFilter_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape) txtFilter.Text = "";
    }
  }
}
