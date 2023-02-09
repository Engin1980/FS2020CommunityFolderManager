using CommunityManager.Converters;
using CommunityManager.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

      public bool AcceptsEmptyString { get; set; } = false;
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
      txtInput.SelectAll();
      txtInput.Focus();
    }

    public Input(Data data) : this()
    {
      this.DataContext = data ?? throw new ArgumentNullException(nameof(data));
      if (data.AcceptsEmptyString == false)
      {
        Binding binding = new(nameof(Data.Value))
        {
          UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
          Converter = new StringNonEmptyToBoolConverter()
        };
        btnApply.SetBinding(IsEnabledProperty, binding);
      }
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

    private void txtInput_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        btnApply_Click(this, new RoutedEventArgs());
      else if (e.Key == Key.Escape)
        btnCancel_Click(this, new RoutedEventArgs());
    }

    public static DialogResult ShowDialog(string title, string prompt, string value)
    {
      Data data = new(title, prompt, value);
      new Input(data)
      {
        Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)
      }.ShowDialog();
      return data.DialogResult;
    }
  }
}
