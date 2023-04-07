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
  /// Interaction logic for GroupManager.xaml
  /// </summary>
  public partial class GroupManager : Window
  {
    public class GroupContext : NotifyPropertyChangedBase
    {

      public ListCollectionView AddonsLcv
      {
        get => base.GetProperty<ListCollectionView>(nameof(AddonsLcv))!;
        set => base.UpdateProperty(nameof(AddonsLcv), value);
      }

      public BindingList<GroupAddonView> Groups
      {
        get => base.GetProperty<BindingList<GroupAddonView>>(nameof(Groups))!;
        set => base.UpdateProperty(nameof(Groups), value);
      }

      public GroupAddonView? SelectedGroup
      {
        get => base.GetProperty<GroupAddonView?>(nameof(SelectedGroup))!;
        set
        {
          base.UpdateProperty(nameof(SelectedGroup), value);
          if (this.SelectedGroup is null)
          {
            this.SelectedGroupAddonsLcv = new ListCollectionView(new List<object>());
          }
          else
          {
            this.SelectedGroupAddonsLcv = new ListCollectionView(this.SelectedGroup?.Addons);
            this.SelectedGroupAddonsLcv.SortDescriptions.Add(
              new SortDescription("Title", ListSortDirection.Ascending));
          }
        }
      }

      public ListCollectionView SelectedGroupAddonsLcv
      {
        get => base.GetProperty<ListCollectionView>(nameof(SelectedGroupAddonsLcv))!;
        set => base.UpdateProperty(nameof(SelectedGroupAddonsLcv), value);
      }
    }

    public GroupContext Context { get; set; }
    private BindingList<AddonView> addons;

    public GroupManager()
    {
      InitializeComponent();
    }

    public GroupManager(BindingList<AddonView> addons, GroupAddonView? selectedGroupAddonView) : this()
    {
      this.addons = addons;
      var grps = addons.Where(q => q is GroupAddonView)
        .Cast<GroupAddonView>()
        .OrderBy(q => q.Title)
        .ToBindingList();
      //var others = addons.Where(q => q is SingleAddonView)
      //  .Cast<SingleAddonView>()
      //  .OrderBy(q => q.Title)
      //  .ToBindingList();

      this.Context = new GroupContext()
      {
        AddonsLcv = new ListCollectionView(addons),
        Groups = grps,
        SelectedGroup = selectedGroupAddonView
      };
      this.Context.AddonsLcv.SortDescriptions.Add(
        new SortDescription("Title", ListSortDirection.Ascending));
      this.Context.AddonsLcv.Filter = q => ((AddonView)q).IsGrouped == false;
      this.DataContext = this.Context;
    }

    private void btnMoveRight_Click(object sender, RoutedEventArgs e)
    {
      var selected = lstAll.SelectedItems.Cast<SingleAddonView>().ToList();
      selected.ForEach(q => Context.AddonsLcv.Remove(q));
      selected.ForEach(q => Context.SelectedGroupAddonsLcv.AddNewItem(q));
      Context.SelectedGroupAddonsLcv.CommitNew();
    }

    private void btnMoveLeft_Click(object sender, RoutedEventArgs e)
    {
      var selected = lstGroup.SelectedItems.Cast<SingleAddonView>().ToList(); //.Cast<SingleAddonView>().ToList();
      selected.ForEach(q => Context.SelectedGroupAddonsLcv.Remove(q));
      selected.ForEach(q => Context.AddonsLcv.AddNewItem(q));
      Context.AddonsLcv.CommitNew();
    }

    private void txtAllFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (txtAllFilter.Text.Length > 0)
        this.Context.AddonsLcv.Filter = q =>
        ((SingleAddonView)q).Title.ToLower().Contains(txtAllFilter.Text.ToLower());
      else
        this.Context.AddonsLcv.Filter = null;
    }

    private void txtGroupFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (txtGroupFilter.Text.Length > 0)
        this.Context.SelectedGroupAddonsLcv.Filter = q =>
        ((SingleAddonView)q).Title.ToLower().Contains(txtGroupFilter.Text.ToLower());
      else
        this.Context.AddonsLcv.Filter = null;
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      var selected = lstAll.SelectedItems.Cast<SingleAddonView>().ToList();
      selected.ForEach(q => Context.AddonsLcv.Remove(q));

      GroupAddonView groupAddonView = new(selected, "<new_group>");
      this.Context.Groups.Add(groupAddonView);
      this.Context.SelectedGroup = groupAddonView;
    }

    private void btnDissolve_Click(object sender, RoutedEventArgs e)
    {
      if (Context.SelectedGroup is null) return;
      if (Message.ShowDialog(
        "Dissolve group?",
        "The selected group will be dissolved. Are you sure?",
        Types.DialogResult.Yes, Types.DialogResult.No) == Types.DialogResult.No)
        return;

      GroupAddonView gav = Context.SelectedGroup;
      Context.SelectedGroup = null;
      Context.Groups.Remove(gav);
      Context.AddonsLcv.Remove(gav);
      gav.Addons.ForEach(q => Context.AddonsLcv.AddNewItem(q));
      Context.AddonsLcv.CommitNew();
    }
  }
}
