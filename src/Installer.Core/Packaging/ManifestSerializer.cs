using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;

namespace Installer.Core.Packaging
{
    /// <summary>清单的序列化设置。写入端与读取端共用，保证往返一致。</summary>
    public static class ManifestSerializer
    {
        /// <summary>统一的 JSON 设置：缩进（可 diff）、不写 null、不转义中文。</summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Culture = CultureInfo.InvariantCulture,
            StringEscapeHandling = StringEscapeHandling.Default,
        };

        /// <summary>序列化为 UTF-8（无 BOM）字节。</summary>
        public static byte[] SerializeToUtf8Bytes(InstallerManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            var json = JsonConvert.SerializeObject(manifest, Settings);
            return new UTF8Encoding(false).GetBytes(json);
        }

        /// <summary>序列化为字符串。</summary>
        public static string Serialize(InstallerManifest manifest)
        {
            return JsonConvert.SerializeObject(manifest, Settings);
        }

        /// <summary>从 UTF-8 字节反序列化。</summary>
        public static InstallerManifest Deserialize(byte[] utf8Bytes)
        {
            if (utf8Bytes == null)
            {
                throw new ArgumentNullException("utf8Bytes");
            }

            var json = new UTF8Encoding(false).GetString(utf8Bytes);
            return Deserialize(json);
        }

        /// <summary>从 JSON 文本反序列化，并做基本校验。</summary>
        public static InstallerManifest Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidDataException("清单为空。");
            }

            InstallerManifest manifest;
            try
            {
                manifest = JsonConvert.DeserializeObject<InstallerManifest>(json, Settings);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("清单不是合法的 JSON：" + ex.Message, ex);
            }

            if (manifest == null)
            {
                throw new InvalidDataException("清单反序列化后为 null。");
            }

            // 宽松校验：这里可能是**工程文件**（.wmpkg.json），它的 files 本来就由打包器填充。
            // 真正的"必须是安装包清单"的严格校验由 ContainerFormat.ReadManifest 负责。
            Validate(manifest, false);
            return manifest;
        }

        /// <summary>校验清单的基本合法性。</summary>
        public static void Validate(InstallerManifest m)
        {
            Validate(m, true);
        }

        /// <summary>
        /// 校验清单的基本合法性。
        /// </summary>
        /// <param name="m">清单。</param>
        /// <param name="requireFiles">
        /// 是否要求 <c>files</c> 非空。**打包阶段应传 false** ——
        /// 工程文件里本来就不写 files（由打包器扫描源目录后填充）。
        /// </param>
        public static void Validate(InstallerManifest m, bool requireFiles)
        {
            if (m == null)
            {
                throw new InvalidDataException("清单为 null。");
            }

            if (m.SchemaVersion <= 0)
            {
                throw new InvalidDataException("清单缺少 schemaVersion。");
            }

            if (m.SchemaVersion > InstallerManifestSchema.Current)
            {
                throw new NotSupportedException(
                    "清单 schemaVersion=" + m.SchemaVersion +
                    " 高于本程序支持的 " + InstallerManifestSchema.Current + "，请升级安装器。");
            }

            if (string.IsNullOrWhiteSpace(m.ProductCode))
            {
                throw new InvalidDataException("清单缺少 productCode。");
            }

            if (m.Product == null || m.Product.Name == null || m.Product.Name.Count == 0)
            {
                throw new InvalidDataException("清单缺少 product.name。");
            }

            if (string.IsNullOrWhiteSpace(m.Scope))
            {
                throw new InvalidDataException("清单缺少 scope。");
            }

            if (m.Scope != InstallScopes.PerUser && m.Scope != InstallScopes.PerMachine)
            {
                throw new InvalidDataException("scope 只能是 \"" + InstallScopes.PerUser + "\" 或 \"" +
                                               InstallScopes.PerMachine + "\"，实际为 \"" + m.Scope + "\"。");
            }

            if (requireFiles && (m.Files == null || m.Files.Count == 0))
            {
                throw new InvalidDataException("清单的 files 为空 —— 没有文件可安装。");
            }

            if (m.DefaultCulture == null)
            {
                m.DefaultCulture = "zh-Hans";
            }

            if (m.Cultures == null || m.Cultures.Count == 0)
            {
                m.Cultures = new System.Collections.Generic.List<string> { m.DefaultCulture };
            }
        }
    }

    /// <summary>清单 schema 的版本常量。</summary>
    public static class InstallerManifestSchema
    {
        /// <summary>本程序支持的最高 schemaVersion。</summary>
        public const int Current = 2;
    }
}
