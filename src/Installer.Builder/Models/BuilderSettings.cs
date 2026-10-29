using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Installer.Builder.Models
{
    /// <summary>
    /// 制作端的**界面级**设置（和具体工程无关，跟着这台机器走）。
    ///
    /// 目前只有一项：上次把安装包生成到哪个目录。
    /// 用户反馈：选了生成目录，重启后又变回桌面 —— 因为这个目录以前只存在
    /// "当前工程"里，而且每次改产品名/版本都会被重新推导覆盖掉。
    /// </summary>
    public sealed class BuilderSettings
    {
        /// <summary>设置文件路径：%APPDATA%\安装包制作助手\settings.json。</summary>
        public static string GetPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "安装包制作助手",
                "settings.json");
        }

        /// <summary>上次选择的生成目录（null / 空 = 还没选过，用默认的桌面）。</summary>
        [JsonProperty("outputDir", NullValueHandling = NullValueHandling.Ignore)]
        public string OutputDir { get; set; }

        /// <summary>读设置；文件不存在或坏了都返回一个空设置（绝不因此让制作端打不开）。</summary>
        public static BuilderSettings Load()
        {
            try
            {
                var path = GetPath();
                if (!File.Exists(path))
                {
                    return new BuilderSettings();
                }

                var json = File.ReadAllText(path, Encoding.UTF8);
                var settings = JsonConvert.DeserializeObject<BuilderSettings>(json);
                return settings ?? new BuilderSettings();
            }
            catch (Exception)
            {
                return new BuilderSettings();
            }
        }

        /// <summary>写设置；失败静默（这是"记住上次选择"，写不进去也不该打断用户）。</summary>
        public void Save()
        {
            try
            {
                var path = GetPath();
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.UTF8);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 记住某个输出文件所在的**目录**。
        ///
        /// 只记目录、不记文件名：文件名要跟着产品名和版本变，用户选的是"生成到哪儿"。
        /// </summary>
        public void RememberOutputDir(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return;
            }

            var dir = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                return;
            }

            OutputDir = dir;
        }
    }
}
