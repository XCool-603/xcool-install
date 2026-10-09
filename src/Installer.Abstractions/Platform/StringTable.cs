using System;
using System.Collections.Generic;
using System.Globalization;
using Installer.Abstractions.Model;

namespace Installer.Abstractions.Platform
{
    /// <summary>
    /// 多语言文案解析。
    ///
    /// 回退链：精确语言（zh-Hans）→ 语言主标签（zh）→ 默认语言 → 任意一个。
    /// 现状完全没有这一层：Designer 里 319 处、代码里 187 处中文硬编码，无法翻译。
    /// </summary>
    public sealed class StringTable : IStringTable
    {
        private readonly Dictionary<string, LocalizedText> _strings;
        private readonly string _defaultCulture;
        private readonly List<string> _available = new List<string>();

        /// <summary>构造。</summary>
        /// <param name="strings">文案表。</param>
        /// <param name="requestedCulture">请求的语言；为 null 时用 <paramref name="defaultCulture"/>。</param>
        /// <param name="defaultCulture">默认语言。</param>
        public StringTable(Dictionary<string, LocalizedText> strings, string requestedCulture, string defaultCulture)
        {
            _strings = strings ?? new Dictionary<string, LocalizedText>(StringComparer.Ordinal);
            _defaultCulture = string.IsNullOrEmpty(defaultCulture) ? "zh-Hans" : defaultCulture;

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in _strings)
            {
                foreach (var c in kv.Value.Keys)
                {
                    if (set.Add(c))
                    {
                        _available.Add(c);
                    }
                }
            }

            _available.Sort(StringComparer.OrdinalIgnoreCase);

            var want = string.IsNullOrEmpty(requestedCulture) ? _defaultCulture : requestedCulture;
            Culture = ResolveCultureName(want);
        }

        /// <inheritdoc />
        public string Culture { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<string> AvailableCultures
        {
            get { return _available; }
        }

        /// <inheritdoc />
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            LocalizedText text;
            if (!_strings.TryGetValue(key, out text) || text == null || text.Count == 0)
            {
                return null;
            }

            return Resolve(text, null);
        }

        /// <inheritdoc />
        public string Resolve(LocalizedText text, string fallback)
        {
            if (text == null || text.Count == 0)
            {
                return fallback;
            }

            string v;
            if (text.TryGetValue(Culture, out v))
            {
                return v;
            }

            var dash = Culture.IndexOf('-');
            if (dash > 0)
            {
                var primary = Culture.Substring(0, dash);
                foreach (var kv in text)
                {
                    if (string.Equals(kv.Key, primary, StringComparison.OrdinalIgnoreCase))
                    {
                        return kv.Value;
                    }
                }
            }

            if (text.TryGetValue(_defaultCulture, out v))
            {
                return v;
            }

            // 最后才退到"任意一个"（字典顺序不保证，所以只在前面全部落空时用）
            foreach (var kv in text)
            {
                return kv.Value;
            }

            return fallback;
        }

        /// <summary>在可用语言里找最接近的一个。</summary>
        private string ResolveCultureName(string requested)
        {
            if (string.IsNullOrEmpty(requested))
            {
                return _defaultCulture;
            }

            foreach (var c in _available)
            {
                if (string.Equals(c, requested, StringComparison.OrdinalIgnoreCase))
                {
                    return c;
                }
            }

            var dash = requested.IndexOf('-');
            if (dash > 0)
            {
                var primary = requested.Substring(0, dash);
                foreach (var c in _available)
                {
                    if (string.Equals(c, primary, StringComparison.OrdinalIgnoreCase))
                    {
                        return c;
                    }
                }
            }

            return _defaultCulture;
        }

        /// <summary>按当前 UI 语言推断请求的语言。</summary>
        public static string DetectRequestedCulture()
        {
            try
            {
                return CultureInfo.CurrentUICulture.Name;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
