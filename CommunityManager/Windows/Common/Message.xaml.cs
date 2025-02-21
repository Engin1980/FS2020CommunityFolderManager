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
        : this(null, title, prompt, dialogResults)
      {
      }

      public Data(Window? owner, string title, string prompt, params DialogResult[] dialogResults)
      {
        Owner = owner;
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
      public Window? Owner { get; set; }
    }

    public Message()
    {
      InitializeComponent();
    }

    public Message(Data data) : this()
    {
      this.DataContext = data ?? throw new ArgumentNullException(nameof(data));
      this.Owner = data?.Owner;
      if (this.Owner != null)
      {
        double w = (this.Owner.Width - data!.WindowWidth) / 2;
        this.Left = Math.Max(0, this.Owner.Left + w);
        double t = (this.Owner.Height - data!.WindowHeight) / 2;
        this.Top = Math.Max(0, t);
      }
      this.Width = data!.WindowWidth;
      this.Height = data!.WindowHeight;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void btn_Click(object sender, RoutedEventArgs e)
    {
      Button btn = (Button)sender;
      (this.DataContext as Data)!.DialogResult = (Types.DialogResult)btn.Tag;
      this.Hide();
    }

    public static DialogResult ShowDialog(Window? owner, string title, string prompt, params DialogResult[] availableResults)
    {
      Data data = new(owner, title, prompt, availableResults);
      var dialog = new Message(data)
      {
        Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)
      };
      dialog.ShowDialog();
      return data.DialogResult;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      itmButtons.Focus();
    }
  }
}
