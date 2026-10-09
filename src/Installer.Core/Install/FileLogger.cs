using System;
using System.Globalization;
using System.IO;
using System.Text;
using Installer.Abstractions.Platform;

namespace Installer.Core.Install
{
    /// <summary>
    /// 把日志写到文件（以及控制台）。安装过程必须可追溯 —— 现状全靠空 catch。
    /// </summary>
    public sealed class FileLogger : ILogger
    {
        private readonly string _path;
        private readonly bool _echoToConsole;
        private readonly object _gate = new object();

        /// <summary>构造。</summary>
        /// <param name="path">日志文件路径；为 null 则只写控制台。</param>
        /// <param name="echoToConsole">是否同时写控制台。</param>
        public FileLogger(string path, bool echoToConsole = true)
        {
            _path = path;
            _echoToConsole = echoToConsole;

            if (!string.IsNullOrEmpty(_path))
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            Write("INFO ", message, null);
        }

        /// <inheritdoc />
        public void Warn(string message)
        {
            Write("WARN ", message, null);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception = null)
        {
            Write("ERROR", message, exception);
        }

        private void Write(string level, string message, Exception ex)
        {
            var line = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}",
                DateTimeOffset.Now, level, message);

            if (ex != null)
            {
                line += Environment.NewLine + ex;
            }

            lock (_gate)
            {
                if (!string.IsNullOrEmpty(_path))
                {
                    try
                    {
                        File.AppendAllText(_path, line + Environment.NewLine, new UTF8Encoding(false));
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                if (_echoToConsole)
                {
                    try
                    {
                        Console.WriteLine(line);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }
    }
}
