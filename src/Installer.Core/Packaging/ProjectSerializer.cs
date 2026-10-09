using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Installer.Abstractions.Model;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// 工程文件（<c>.wmpkg.json</c>）的读写。
    ///
    /// 同时接受两种形状：
    /// ① 裸清单（手写方便）
    /// ② <c>{ "manifest": ..., "build": ... }</c>（制作端 GUI 存出来的）
    /// </summary>
    public static class ProjectSerializer
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        /// <summary>从 JSON 文本解析工程（两种形状都认）。</summary>
        public static InstallerProject Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidDataException("工程文件为空。");
            }

            JObject root;
            try
            {
                // 必须显式关掉日期识别：否则 "2026-10-08T12:00:00Z" 会被解析成 DateTime，
                // 再转回字符串就变成了 "10/08/2026 12:00:00"。
                using (var reader = new JsonTextReader(new StringReader(json)))
                {
                    reader.DateParseHandling = DateParseHandling.None;
                    root = JObject.Load(reader);
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("工程文件不是合法的 JSON：" + ex.Message, ex);
            }

            // 有顶层 manifest 属性 → 完整工程；否则整个文件就是清单
            var manifestToken = root["manifest"];
            if (manifestToken != null && manifestToken.Type == JTokenType.Object)
            {
                var project = root.ToObject<InstallerProject>(JsonSerializer.Create(ManifestSerializer.Settings));
                if (project == null)
                {
                    throw new InvalidDataException("工程文件解析后为 null。");
                }

                if (project.Manifest == null)
                {
                    throw new InvalidDataException("工程文件缺少 manifest。");
                }

                if (project.Build == null)
                {
                    project.Build = new ProjectBuildSettings();
                }

                ManifestSerializer.Validate(project.Manifest, false);
                return project;
            }

            var bare = ManifestSerializer.Deserialize(json);
            return new InstallerProject { Manifest = bare, Build = new ProjectBuildSettings() };
        }

        /// <summary>从文件读取工程。</summary>
        public static InstallerProject Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("工程文件不存在：" + path, path);
            }

            return Deserialize(File.ReadAllText(path, Encoding.UTF8));
        }

        /// <summary>把工程序列化成可读、可 diff 的 JSON。</summary>
        public static string Serialize(InstallerProject project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            return JsonConvert.SerializeObject(project, ManifestSerializer.Settings);
        }

        /// <summary>把工程写到文件（UTF-8 无 BOM）。</summary>
        public static void Save(string path, InstallerProject project)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, Serialize(project), Utf8NoBom);
        }
    }
}
