using CommunityManager.Types;
using CommunityManager.Windows;
using CommunityManagerLib;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommunityManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private const string COMMUNITY_FOLDER = "D:\\FS2020\\Community";

    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      LoadData(false);
    }

    private void LoadData(bool prompts = true )
    {
      if (prompts) 
        if (Message.ShowDialog(
          "Load", 
          "You will loose all unsaved changes. Are you sure you would like to reload the data?",
          Types.DialogResult.Yes, Types.DialogResult.Cancel) == Types.DialogResult.Cancel) return;

      AddonScanner scanner = new();
      BindingList<AddonState> lst = scanner.ScanAddons(COMMUNITY_FOLDER).ToBindingList();
      DataContext = lst;
      if (prompts)
        Message.ShowDialog("Reloaded.", "Changes have been reloaded.", Types.DialogResult.Ok);
    }

    private void SaveData()
    {
      AddonScanner scanner = new();
      BindingList<AddonState> lst = (BindingList<AddonState>)this.DataContext;
      scanner.SaveAddonsState(lst.ToList(), COMMUNITY_FOLDER);
      Message.ShowDialog("Saved.", "Changes have been saved.", Types.DialogResult.Ok);
    }

    private void btnCustomTitle_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonStates.SelectedItem is not AddonState addonState) return;

      Input.Data data = new(
        "Adjust Custom Title...",
        "Set the new custom title for the addon:",
        addonState.CustomTitle ?? addonState.Addon.ManifestTitle ?? ""
        )
      {
        WindowHeight = 100,
        WindowWidth = 200
      };
      Input frmInput = new();
      frmInput.Bind(data);
      frmInput.ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonState.CustomTitle = data.Value.Trim();
    }

    private void btnAssignTags_Click(object sender, RoutedEventArgs e)
    {
      if (lstAddonStates.SelectedItem is not AddonState addonState) return;

      var tmp = (BindingList<AddonState>)this.DataContext!;
      BindingList<TagEditor.CheckItem> tags = tmp
        .SelectMany(q => q.Tags)
        .Distinct()
        .OrderBy(q => q)
        .Select(q => new TagEditor.CheckItem(q, false))
        .ToBindingList();

      TagEditor.Data data = new()
      {
        Tags = tags
      };

      TagEditor tagEditor = new();
      tagEditor.Bind(data);
      tagEditor.ShowDialog();

      if (data.DialogResult == Types.DialogResult.Cancel) return;

      addonState.SetTags(data.Tags.Where(q => q.IsChecked).Select(q => q.Label).ToList());
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

    }
  }
}
