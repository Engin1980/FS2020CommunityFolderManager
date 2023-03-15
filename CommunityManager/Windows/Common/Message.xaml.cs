using CommunityManager.Types;
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
using System.Windows.Shapes;

namespace CommunityManager.Windows
{
  /// <summary>
  /// Interaction logic for Message.xaml
  /// </summary>
  public partial class Message : Window
  {
    public class Data : NotifyPropertyChangedBase
    {
      public Data(string title, string prompt, params DialogResult[] dialogResults)
      {
        Title = title;
        Prompt = prompt;
        AvailableDialogResults = dialogResults;
        WindowWidth = 400;
        WindowHeight = 250;
      }

      public DialogResult[] AvailableDialogResults { get; set; }

      public string Title { get; set; }
      public string Prompt { get; set; }

      public int WindowHeight
      {
        get => base.GetProperty<int>(nameof(WindowHeight))!;
        set => base.UpdateProperty(nameof(WindowHeight), value);
      }
      public int WindowWidth
      {
        get => base.GetProperty<int>(nameof(WindowWidth))!;
        set => base.UpdateProperty(nameof(WindowWidth), value);
      }
      public Types.DialogResult DialogResult { get; set; } = Types.DialogResult.Cancel;
    }

    public Message()
    {
      InitializeComponent();
    }

    public Message(Data data) : this()
    {
      this.DataContext = data ?? throw new ArgumentNullException(nameof(data));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void btn_Click(object sender, RoutedEventArgs e)
    {
      Button btn = (Button)sender;
      (this.DataContext as Data)!.DialogResult = (Types.DialogResult)btn.Tag;
      this.Hide();
    }

    public static DialogResult ShowDialog(string title, string prompt, params DialogResult[] availableResults)
    {
      Data data = new(title, prompt, availableResults);
      new Message(data)
      {
        Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)
      }.ShowDialog();
      return data.DialogResult;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      itmButtons.Focus();
    }

    private void Window_Initialized(object sender, EventArgs e)
    {
      this.GetBindingExpression(Window.WidthProperty).UpdateTarget();
      this.GetBindingExpression(Window.HeightProperty).UpdateTarget();
    }
  }
}
