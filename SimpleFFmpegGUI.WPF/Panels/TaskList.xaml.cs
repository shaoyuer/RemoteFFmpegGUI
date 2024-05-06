﻿using FzLib.WPF;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using ModernWpf.FzExtension.CommonDialog;
using SimpleFFmpegGUI.Manager;
using SimpleFFmpegGUI.Model;
using SimpleFFmpegGUI.WPF;
using SimpleFFmpegGUI.WPF.Model;
using SimpleFFmpegGUI.WPF.Pages;
using SimpleFFmpegGUI.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TaskStatus = SimpleFFmpegGUI.Model.TaskStatus;

namespace SimpleFFmpegGUI.WPF.Panels
{
    public partial class TaskList : UserControl
    {
        public TaskList()
        {
            InitializeComponent();
            ViewModel = this.SetDataContext<TaskListViewModel>();
        }


        public static readonly DependencyProperty ShowAllTasksProperty = DependencyProperty.Register(
            nameof(ShowAllTasks), typeof(bool), typeof(TaskList));

        public bool ShowAllTasks
        {
            get => (bool)GetValue(ShowAllTasksProperty);
            set => SetValue(ShowAllTasksProperty, value);
        }

        public TaskListViewModel ViewModel { get; }

        private void UpdateDetailHeight()
        {
            bdDetail.Height = App.ServiceProvider.GetService<MainWindow>().IsUiCompressMode && !ShowAllTasks ? 108 : double.NaN;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDetailHeight();
            App.ServiceProvider.GetService<MainWindow>().UiCompressModeChanged +=
                (s, e) => UpdateDetailHeight();
        }
    }
}