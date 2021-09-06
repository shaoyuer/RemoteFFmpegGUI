﻿using Enterwell.Clients.Wpf.Notifications;
using FzLib;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using SimpleFFmpegGUI.Manager;
using SimpleFFmpegGUI.Model;
using SimpleFFmpegGUI.WPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace SimpleFFmpegGUI.WPF
{
    public class LogsWindowViewModel : INotifyPropertyChanged
    {
        public LogsWindowViewModel()
        {
            Tasks = TaskManager.GetTasks().List.Adapt<List<UITaskInfo>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Log> logs;

        public List<Log> Logs
        {
            get => logs;
            set => this.SetValueAndNotify(ref logs, value, nameof(Logs));
        }

        private DateTime from = DateTime.Now.AddDays(-1);

        public DateTime From
        {
            get => from;
            set => this.SetValueAndNotify(ref from, value, nameof(From));
        }

        private DateTime to = DateTime.Now;

        public DateTime To
        {
            get => to;
            set => this.SetValueAndNotify(ref to, value, nameof(To));
        }

        private List<UITaskInfo> tasks;

        public List<UITaskInfo> Tasks
        {
            get => tasks;
            set => this.SetValueAndNotify(ref tasks, value, nameof(Tasks));
        }

        private UITaskInfo selectedTask;

        public UITaskInfo SelectedTask
        {
            get => selectedTask;
            set => this.SetValueAndNotify(ref selectedTask, value, nameof(SelectedTask));
        }

        private string type;

        public string Type
        {
            get => type;
            set => this.SetValueAndNotify(ref type, value, nameof(Type));
        }

        public void FillLogs()
        {
            Logs = LogManager.GetLogs(taskId: SelectedTask.Id, from: From, to: To).List;
        }
    }

    /// <summary>
    /// Interaction logic for LogsWindow.xaml
    /// </summary>
    public partial class LogsWindow : Window
    {
        public LogsWindowViewModel ViewModel { get; set; }

        public LogsWindow(LogsWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FillLogs();
        }
    }
}