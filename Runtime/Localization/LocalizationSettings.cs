using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DouduckLib;

namespace DouduckLib.Localization
{
    public static class LocalizationInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitRuntime()
        {
            LoadAndRegister();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitEditor()
        {
            LoadAndRegister();
        }
#endif

        static void LoadAndRegister()
        {
            var asset = Resources.Load<LocalizationSettings>("LocalizationSettings");
            if (asset)
            {
                asset.RegisterIfNeeded();
            }
        }
    }

    [CreateAssetMenu(menuName = "DouduckLib/Localization/Localization Settings")]
    public sealed class LocalizationSettings : ScriptableObject
    {
        static LocalizationSettings _instance;

        [System.Serializable]
        public struct LanguageCodeSetting
        {
            public string languageCode;
            public string displayName;
            public SystemLanguage[] systemLanguages;
        }

        [Header("Localization Settings")]
        [SerializeField] string _defaultLanguageCode = "en-us";
        [SerializeField] List<LanguageCodeSetting> _languageCodeMappings = new();
        [SerializeField] List<StringTable> _stringTables = new();

        void OnEnable()
        {
            RegisterIfNeeded();
        }

        public void RegisterIfNeeded()
        {
            if (_instance == null)
            {
                _instance = this;
                Localization.Get().Initialize(_instance, _defaultLanguageCode);
            }
        }

        public string GetDefaultLanguageCode() => _defaultLanguageCode;
        public IEnumerable<string> GetAllLanguageCode() => _languageCodeMappings.Select(mapping => mapping.languageCode);

        public string GetLanguageCode(SystemLanguage systemLanguage)
        {
            foreach (var languageCodeSetting in _languageCodeMappings)
            {
                if (languageCodeSetting.systemLanguages != null && languageCodeSetting.systemLanguages.Contains(systemLanguage))
                {
                    return languageCodeSetting.languageCode;
                }
            }
            return null;
        }

        public string GetLanguageDisplayName(string languageCode)
        {
            foreach (var languageCodeSetting in _languageCodeMappings)
            {
                if (languageCodeSetting.languageCode == languageCode)
                {
                    return languageCodeSetting.displayName;
                }
            }
            return default;
        }

        public IEnumerable<StringTable> FindStringTables(string languageCode)
        {
            return _stringTables.Where(table => table != null && table.LanguageCode == languageCode);
        }

#if UNITY_EDITOR
        public void ClearStringTable()
        {
            _stringTables.Clear();
        }

        public void AddStringTable(StringTable stringTable)
        {
            if (stringTable != null && !_stringTables.Contains(stringTable))
            {
                _stringTables.Add(stringTable);
            }
        }

        public void AddStringTable(IEnumerable<StringTable> stringTables)
        {
            foreach (var stringTable in stringTables)
            {
                AddStringTable(stringTable);
            }
        }

        [Button]
        public void ReloadStringTables()
        {
            _instance = this;
            Localization.Get().Initialize(_instance);
        }
#endif
    }
}
