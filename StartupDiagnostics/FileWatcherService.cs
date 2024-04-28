using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StartupDiagnostics
{
    /// <summary>
    /// 文件监控
    /// </summary>
    public class FileWatcherService : IHostedService
    {
        private readonly string _watchedFolder;
        private FileSystemWatcher _fileSystemWatcher;
        private readonly IHostApplicationLifetime _appLifetime;
        /// <summary>
        /// 文件监控
        /// </summary>
        /// <param name="appLifetime"></param>
        public FileWatcherService(IHostApplicationLifetime appLifetime)
        {
            _watchedFolder = Path.Combine(Environment.CurrentDirectory, "lib"); //细化指定类型的dll
            _appLifetime = appLifetime;
        }

        /// <summary>
        /// 启动器
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _fileSystemWatcher = new FileSystemWatcher(_watchedFolder);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _fileSystemWatcher.Changed += OnFileChanged;
            _fileSystemWatcher.Created += OnFileChanged;
            _fileSystemWatcher.Deleted += OnFileChanged;
            _fileSystemWatcher.Renamed += OnFileChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止器
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _fileSystemWatcher.Dispose();
            return Task.CompletedTask;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // 文件夹内容发生变化时重新启动应用程序
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"exec \"{System.Reflection.Assembly.GetEntryAssembly().Location}\"",
                UseShellExecute = false
            };


            _appLifetime.ApplicationStopped.Register(() =>
            {
                Process.Start(processStartInfo);
            });
            _appLifetime.StopApplication();

        }
    }
}
