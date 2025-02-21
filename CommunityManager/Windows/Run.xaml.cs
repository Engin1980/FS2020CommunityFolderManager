using CommunityManagerLib;
using CommunityManagerLib.RunProcedure;
using CommunityManagerLib.StartupConfigurations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
  /// Interaction logic for Run.xaml
  /// </summary>
  public partial class Run : Window
  {
    public Run()
    {
      InitializeComponent();
    }

    private Project project;
    private StartupConfiguration startupConfiguration;
    private List<string> tags;
    private BindingList<RunTask> ToDoList { get; set; } = new();
    private BindingList<RunTask> DoneList { get; set; } = new();

    public Run(Project project, StartupConfiguration startupConfiguration, List<string> activeTags) : this()
    {
      this.project = project;
      this.startupConfiguration = startupConfiguration;
      this.tags = activeTags;
    }

    public async void DoProcess()
    {
      RunTaskBuilder runTaskBuilder = new();
      runTaskBuilder.EnsureFoldersExist(project);
      List<RunTask> tasks = runTaskBuilder.BuildTasks(project, startupConfiguration);
      prgBar.Maximum = tasks.Count;
      prgBar.Value = 0;

      tasks.ForEach(q => ToDoList.Add(q));

      this.lstToDo.ItemsSource = ToDoList;
      this.lstDone.ItemsSource = DoneList;

      RunTask? runTask;
      while ((runTask = tasks.FirstOrDefault(q => q.State == RunTask.RunTaskState.Ready)) != null)
      {
        if (runTask is ProgramRunTask && runTaskBuilder.StartTimeReferenceValue.Value.Ticks == 0)
          runTaskBuilder.StartTimeReferenceValue.Value = DateTime.Now;

        Task task = Task.Run(runTask.Run);
        await task;

        ToDoList.Remove(runTask);
        DoneList.Add(runTask);
        prgBar.Value++;
      }


      tbcTabs.SelectedIndex = 1;
      chkAutoClose.IsChecked = DoneList.All(q => q.State == RunTask.RunTaskState.Done);

      if (project.Settings.AutoCloseAfterRun) StartAutocloseTime();
    }

    private Timer? autocloseTimer = null;
    private void StartAutocloseTime()
    {
      this.autocloseTimer = new Timer(AutocloseTimer_Callback,
                null,
                project.Settings.AutoCloseDelayInSeconds * 1000,
                Timeout.Infinite);
    }

    private void AutocloseTimer_Callback(object? state)
    {
      Dispatcher.Invoke(
        () =>
        {
          if (chkAutoClose.IsChecked == true)
            Application.Current.Shutdown();
        });
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DoProcess();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}
