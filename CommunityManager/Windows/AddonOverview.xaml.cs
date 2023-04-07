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
        addonView.Note!
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
      if (addonViews.Count == 0)
      {
        Message.ShowDialog("Select at least one addon", 
          "At least one addon item must be selected.", Types.DialogResult.Ok);
        return;
      }
      else if ((addonViews.Count > 1 && addonViews.Any(q => q is GroupAddonView))
        || (addonViews.Count == 1 && addonViews.First() is not GroupAddonView))
      {
        Message.ShowDialog("Correct selected addons",
          "There must be exactly one grouped addon selected, or one or more non-group addons selected.",
          Types.DialogResult.Ok);
          return;
      }

      GroupAddonView gav;
      if (addonViews.First() is SingleAddonView)
      {
        var tmp = addonViews.Cast<SingleAddonView>();
        string title = addonViews.First().Title;
        if (addonViews.Count > 1) title += " + " + addonViews.Count.ToString();
        gav = new(tmp, title);
        int index = this.Project.Addons.IndexOf(tmp.First());
        tmp.ForEach(q => this.Project.Addons.Remove(q));
        this.Project.Addons.Insert(index, gav);
      } else
        gav = (GroupAddonView) addonViews.First();


      GroupManager frm = new GroupManager(this.Project.Addons, gav);
      frm.ShowDialog();

      listCollectionView.Refresh();
    }

    private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateFilter();
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

    private void chkNew_Checked(object sender, RoutedEventArgs e)
    {
      UpdateFilter();
    }

    private void UpdateFilter()
    {
      Predicate<object> textFilter;
      Predicate<object> isNewFilter;
      if (txtFilter.Text.Trim().Length > 0)
      {
        textFilter = o =>
        {
          AddonView av = (AddonView)o;
          string txt = txtFilter.Text.ToLower();
          bool ret = av.Title.ToLower().Contains(txt)
            || av.SourceName.ToLower().Contains(txt)
            || av.Tags.Any(q => q.ToLower().Contains(txt));
          return ret;
        };
      }
      else
        textFilter = o => true;

      if (chkNew.IsChecked == true)
        isNewFilter = o =>
        {
          AddonView av = (AddonView)o;
          return av.IsNew;
        };
      else
        isNewFilter = o => true;

      Predicate<object> andFilter = o => textFilter(o) && isNewFilter(o);
      listCollectionView.Filter = andFilter;
    }
  }
}
