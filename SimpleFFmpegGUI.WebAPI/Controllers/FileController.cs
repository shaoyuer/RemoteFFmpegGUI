﻿using Furion.FriendlyException;
using FzLib.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleFFmpegGUI.Dto;
using SimpleFFmpegGUI.WebAPI.Dto;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFFmpegGUI.WebAPI.Controllers
{
    public class FileController : FFmpegControllerBase
    {
        public static ConcurrentDictionary<string, string> Guid2File { get; } = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> File2Guid { get; } = new ConcurrentDictionary<string, string>();

        public FileController(ILogger<MediaInfoController> logger,
            IConfiguration config,
        PipeClient pipeClient) : base(logger, config, pipeClient) { }

        [HttpGet]
        [Route("List/Input")]
        public async Task<List<string>> GetInputFiles()
        {
            return (CanAccessInputDir() ?
                Directory.EnumerateFiles(GetInputDir()) : await pipeClient.InvokeAsync(p => p.GetFiles(GetInputDir())))
            .Select(p => Path.GetFileName(p)).ToList();
        }

        [HttpGet]
        [Route("List/Output")]
        public async Task<List<FileInfoDto>> GetOutputFiles()
        {
            if (CanAccessOutputDir())
            {
                return Directory.EnumerateFiles(GetOutputDir()).Select(p => new FileInfoDto(p)).ToList();
            }
            else
            {
                return await pipeClient.InvokeAsync(p => p.GetFileDetails(GetOutputDir()));
            }
        }

        [HttpGet]
        [Route("Download")]
        public IActionResult Download(string name)
        {
            if (!CanAccessOutputDir())
            {
                throw Oops.Oh("无法访问输出文件夹，请使用其他方式下载");
            }
            string path = Path.Combine(GetOutputDir(), name);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            var stream = System.IO.File.OpenRead(path);
            return File(stream, "application/octet-stream", Path.GetFileName(path));
        }

        [HttpPost]
        [Route("Ftp/Input/On")]
        public async Task OpenInput()
        {
            await pipeClient.InvokeAsync(p => p.OpenFtp(1, GetInputDir(), config.GetValue("InputFtpPort", 0)));
        }

        [HttpPost]
        [Route("Ftp/Input/Off")]
        public async Task CloseInput()
        {
            await pipeClient.InvokeAsync(p => p.CloseFtp(1));
        }

        [HttpPost]
        [Route("Ftp/Output/On")]
        public async Task OpenOutput()
        {
            await pipeClient.InvokeAsync(p => p.OpenFtp(2, GetInputDir(), config.GetValue("InputFtpPort", 0)));
        }

        [HttpPost]
        [Route("Ftp/Output/Off")]
        public async Task CloseOutput()
        {
            await pipeClient.InvokeAsync(p => p.CloseFtp(2));
        }

        /// <summary>
        /// 获取FTP状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Ftp/Status")]
        public async Task<FtpStatusDto> GetStatus()
        {
            var input = await pipeClient.InvokeAsync(p => p.GetFtpPort(1));
            var output = await pipeClient.InvokeAsync(p => p.GetFtpPort(2));
            var status = new FtpStatusDto()
            {
                InputOn = input != null,
                OutputOn = output != null,
                InputPort = input ?? 0,
                OutputPort = output ?? 0
            };
            return status;
        }

        [HttpPost]
        [Route("Upload")]
        [HttpOptions]
        [DisableRequestSizeLimit]
        public async Task UploadFile([FromQuery] IFormFile file)
        {
            //if (!CanAccessInputDir())
            //{
            //    throw Oops.Oh("无法访问输入文件夹，请使用其他方式上传");
            //}
            if (file.Length > 0)
            {
                string name = Path.Combine(GetInputDir(), file.FileName);
                name = FileSystem.GetNoDuplicateFile(name);
                using var stream = System.IO.File.Create(name);
                await file.CopyToAsync(stream);
            }
        }
    }
}