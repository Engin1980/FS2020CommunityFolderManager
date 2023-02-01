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
    public Project Project { get; set; }

    public MainWindow()
    {
      InitializeComponent();
    }

    private void btnQuit_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void btnAddonOverview_Click(object sender, RoutedEventArgs e)
    {
      var f = new AddonOverview();
      f.Bind(Project);
      f.Show();
      this.Hide();
    }

    public void Bind(Project project)
    {
      this.Project = Project;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var p = Project.Load(out List<string> issues);
      if (issues.Count > 0)
      {
        string prompt = "There were issues during the app startup:\n\n" +
          string.Join("\n\n *)\t", issues);
        var d = new Message.Data("Issues during start up.", prompt, Types.DialogResult.Ok)
        {
          WindowHeight = 400,
          WindowWidth = 600
        };
        var f = new Message();
        f.Bind(d);
        this.Hide();
        f.ShowDialog();
        f.Focus();
        this.Show();
      }

      this.Bind(p);
    }
  }
}
