using System;
using System.Collections.Generic;
using UnityEngine;
using DouduckLib;

namespace DouduckLib.Localization
{
    public class Localization : Singleton<Localization>
    {
        public event Action OnLanguageChanged;

        LocalizationSettings _settingAsset;
        string _currentLanguageCode;
        readonly List<StringTable> _registeredStringTables = new();
        readonly Dictionary<string, string> _stringMappings = new();

        public string GetDefaultLanguageCode() => _settingAsset.GetDefaultLanguageCode();
        public string GetCurrentLanguageCode() => _currentLanguageCode;
        public string GetLanguageCode(SystemLanguage systemLanguage) => _settingAsset.GetLanguageCode(systemLanguage);
        public string GetLanguageDisplayName(string languageCode) => _settingAsset.GetLanguageDisplayName(languageCode);
        public IEnumerable<string> GetAllLanguageCode() => _settingAsset.GetAllLanguageCode();

        public Localization Initialize(LocalizationSettings settingAsset, string languageCode = null)
        {
            if (_settingAsset != null)
            {
                UnregisterAllStringTable();
            }

            _settingAsset = settingAsset;
            if (languageCode != null)
            {
                SwitchToLanguage(languageCode);
            }
            else if (_currentLanguageCode != null)
            {
                SwitchToLanguage(_currentLanguageCode);
            }
            return this;
        }

        public void SwitchToSystemLanguage()
        {
            SwitchToLanguage(Application.systemLanguage);
        }

        public void SwitchToLanguage(SystemLanguage systemLanguage)
        {
            if (_settingAsset == null)
            {
                throw new InvalidOperationException("Localization system has not been initialized");
            }
            SwitchToLanguage(GetLanguageCode(systemLanguage));
        }

        public void SwitchToLanguage(string languageCode)
        {
            if (_settingAsset == null)
            {
                throw new InvalidOperationException("Localization system has not been initialized");
            }

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = GetDefaultLanguageCode();
            }

            _currentLanguageCode = languageCode;
            var stringTables = _settingAsset.FindStringTables(languageCode);
            if (stringTables != null)
            {
                UnregisterAllStringTable();
                foreach (var stringTable in stringTables)
                {
                    RegisterStringTable(stringTable);
                }
                OnLanguageChanged?.Invoke();
            }
        }

        public bool RegisterStringTable(StringTable stringTable)
        {
            if (stringTable == null || _registeredStringTables.Contains(stringTable))
            {
                return false;
            }

            _registeredStringTables.Add(stringTable);
            var ns = stringTable.Namespace;
            foreach (var stringData in stringTable.GetStringDatas())
            {
                var fullKey = $"{ns}.{stringData.key}";
                if (!_stringMappings.TryAdd(fullKey, stringData.content))
                {
                    Debug.LogError($"String table has key conflict: {fullKey}");
                }
            }
            return true;
        }

        public bool UnregisterStringTable(StringTable stringTable)
        {
            if (stringTable == null || !_registeredStringTables.Contains(stringTable))
            {
                return false;
            }

            _registeredStringTables.Remove(stringTable);
            var ns = stringTable.Namespace;
            foreach (var stringData in stringTable.GetStringDatas())
            {
                var fullKey = $"{ns}.{stringData.key}";
                _stringMappings.Remove(fullKey);
            }
            return true;
        }

        public void UnregisterAllStringTable()
        {
            _registeredStringTables.Clear();
            _stringMappings.Clear();
        }

        public string GetString(string ns, string key)
        {
            return GetString($"{ns}.{key}");
        }

        public string GetString(string fullkey)
        {
            if (_stringMappings.ContainsKey(fullkey))
            {
                return _stringMappings[fullkey];
            }
            return $"<missing key: {fullkey}>";
        }

        public bool IsValidKey(string fullkey)
        {
            return _stringMappings.ContainsKey(fullkey);
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _stringMappings.Keys;
        }
    }
}
