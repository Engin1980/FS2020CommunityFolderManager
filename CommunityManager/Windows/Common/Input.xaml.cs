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
  /// Interaction logic for Input.xaml
  /// </summary>
  public partial class Input : Window
  {
    public class Data
    {
      public Data(string title, string prompt, string value)
      {
        Title = title;
        Prompt = prompt;
        Value = value;
      }

      public string Title { get; set; }
      public string Prompt { get; set; }
      public string Value { get; set; }
      public int WindowWidth { get; set; } = 400;
      public int WindowHeight { get; set; } = 250;
      public Types.DialogResult DialogResult { get; set; } = Types.DialogResult.Cancel;
    }

    public Input()
    {
      InitializeComponent();
    }

    public Input(Data data):this()
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
  }
}
