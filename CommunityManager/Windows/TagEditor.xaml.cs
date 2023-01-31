using CommunityManager.Types;
using CommunityManagerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
  /// Interaction logic for TagEditor.xaml
  /// </summary>
  public partial class TagEditor : Window
  {
    public class Data
    {
      public BindingList<CheckItem> Tags { get; set; } = new BindingList<CheckItem>();
      public DialogResult DialogResult { get; set; } = DialogResult.Cancel;
    }

    public class CheckItem : NotifyPropertyChangedBase
    {
      public CheckItem(string label, bool isChecked)
      {
        Label = label;
        IsChecked = isChecked;
      }

      public string Label { get => base.GetProperty<string>(nameof(Label))!; set => base.UpdateProperty(nameof(Label), value); }
      public bool IsChecked { get => base.GetProperty<bool>(nameof(IsChecked)); set => base.UpdateProperty(nameof(IsChecked), value); }
    }

    public TagEditor()
    {
      InitializeComponent();
    }

    public void Bind(Data data)
    {
      this.DataContext = data ?? throw new ArgumentNullException(nameof(data));
    }

    private void btnApply_Click(object sender, RoutedEventArgs e)
    {
      (this.DataContext as Data)!.DialogResult = Types.DialogResult.Ok;
      this.Hide();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      (this.DataContext as Data)!.DialogResult = Types.DialogResult.Cancel;
      this.Hide();
    }

    private void btnSelectAll_Click(object sender, RoutedEventArgs e)
    {
      var data = GetData();
      data.Tags.ForEach(q => q.IsChecked = true);
      data.Tags.ResetBindings();
    }

    private void btnSelectNone_Click(object sender, RoutedEventArgs e)
    {
      var data = GetData();
      data.Tags.ForEach(q => q.IsChecked = false);
      data.Tags.ResetBindings();
    }

    private Data GetData() => (this.DataContext as Data)!;

    private void btnAddNewTag_Click(object sender, RoutedEventArgs e)
    {
      string newTag = txtNewTag.Text.Trim();
      var data = GetData();
      data.Tags.Add(new CheckItem(newTag, true));
      txtNewTag.Text = "";
      this.lstTags.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
    }
  }
}
