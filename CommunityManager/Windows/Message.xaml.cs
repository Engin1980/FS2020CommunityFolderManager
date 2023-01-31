using CommunityManager.Types;
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
  /// Interaction logic for Message.xaml
  /// </summary>
  public partial class Message : Window
  {
    public class Data
    {
      public Data(string title, string prompt, params DialogResult[] dialogResults)
      {
        Title = title;
        Prompt = prompt;
        AvailableDialogResults = dialogResults;
      }

      public DialogResult[] AvailableDialogResults { get; set; }

      public string Title { get; set; }
      public string Prompt { get; set; }
      public int WindowWidth { get; set; } = 400;
      public int WindowHeight { get; set; } = 250;
      public Types.DialogResult DialogResult { get; set; } = Types.DialogResult.Cancel;
    }

    public Message()
    {
      InitializeComponent();
    }

    public void Bind(Data data)
    {
      this.DataContext = data ?? throw new ArgumentNullException(nameof(data));
    }

    private void btn_Click(object sender, RoutedEventArgs e)
    {
      Button btn = (Button)sender;
      (this.DataContext as Data)!.DialogResult = (Types.DialogResult)btn.Tag;
      this.Hide();
    }

    public static DialogResult ShowDialog(string title, string prompt, params DialogResult[] availableResults)
    {
      Data data = new Data(title, prompt, availableResults);
      Message message = new Message();
      message.Bind(data);
      message.ShowDialog();
      return data.DialogResult;
    }
  }
}
