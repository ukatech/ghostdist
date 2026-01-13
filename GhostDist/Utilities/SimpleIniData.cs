using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostDist.Utilities
{
    /// <summary>
    /// INIデータを保持するクラス（IniDataの代替）
    /// </summary>
    public class SimpleIniData
    {
        private Dictionary<string, Dictionary<string, string>> _sections;

        public SimpleIniData()
        {
            _sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// セクションコレクションを取得
        /// </summary>
        public IniSections Sections => new IniSections(_sections);

        /// <summary>
        /// セクションとキーを指定して値を取得・設定
        /// </summary>
        public string this[string section, string key]
        {
            get
            {
                if (_sections.TryGetValue(section, out var sectionData))
                {
                    if (sectionData.TryGetValue(key, out var value))
                    {
                        return value;
                    }
                }
                return null;
            }
            set
            {
                if (!_sections.ContainsKey(section))
                {
                    _sections[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                _sections[section][key] = value;
            }
        }

        /// <summary>
        /// セクションを指定してキー-値のディクショナリを取得
        /// </summary>
        public IniSection this[string section]
        {
            get
            {
                if (!_sections.ContainsKey(section))
                {
                    _sections[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return new IniSection(_sections[section]);
            }
        }

        /// <summary>
        /// セクションを追加
        /// </summary>
        internal void AddSection(string sectionName)
        {
            if (!_sections.ContainsKey(sectionName))
            {
                _sections[sectionName] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// キー-値のペアを追加
        /// </summary>
        internal void AddKeyValue(string sectionName, string key, string value)
        {
            if (!_sections.ContainsKey(sectionName))
            {
                _sections[sectionName] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            _sections[sectionName][key] = value;
        }

        /// <summary>
        /// すべてのセクションを取得
        /// </summary>
        internal Dictionary<string, Dictionary<string, string>> GetAllSections()
        {
            return _sections;
        }
    }

    /// <summary>
    /// INIセクションのコレクション
    /// </summary>
    public class IniSections
    {
        private Dictionary<string, Dictionary<string, string>> _sections;

        internal IniSections(Dictionary<string, Dictionary<string, string>> sections)
        {
            _sections = sections;
        }

        /// <summary>
        /// セクションが存在するかチェック
        /// </summary>
        public bool ContainsSection(string sectionName)
        {
            return _sections.ContainsKey(sectionName);
        }

        /// <summary>
        /// すべてのセクション名を取得
        /// </summary>
        public IEnumerable<string> GetSectionNames()
        {
            return _sections.Keys;
        }
    }

    /// <summary>
    /// INIセクション（キー-値のペアを保持）
    /// </summary>
    public class IniSection
    {
        private Dictionary<string, string> _keyValues;

        internal IniSection(Dictionary<string, string> keyValues)
        {
            _keyValues = keyValues;
        }

        /// <summary>
        /// キーを指定して値を取得・設定
        /// </summary>
        public string this[string key]
        {
            get
            {
                if (_keyValues.TryGetValue(key, out var value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                _keyValues[key] = value;
            }
        }

        /// <summary>
        /// すべてのキーを取得
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetKeyValues()
        {
            return _keyValues;
        }
    }
}
