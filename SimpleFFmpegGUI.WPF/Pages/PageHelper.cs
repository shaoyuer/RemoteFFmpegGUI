﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFFmpegGUI.WPF.Pages
{
    public static class PageHelper
    {
        public static string GetTitle(Type type)
        {
            return type.Name switch
            {
                nameof(AddTaskPage) => "新增任务",
                nameof(MediaInfoPage) => "媒体信息",
                nameof(PresetsPage) => "所有预设",
                nameof(TasksPage) => "所有任务",
                nameof(LogsPage) => "日志",
                nameof(SettingPage) => "设置",
                nameof(FFmpegOutputPage) => "FFmpeg输出命令行",
                _ => throw new NotImplementedException(),
            };
        }
    }
}