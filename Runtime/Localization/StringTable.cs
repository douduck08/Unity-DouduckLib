using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DouduckLib.Localization
{
    [System.Serializable]
    public struct LocalizedStringData
    {
        public string key;
        public string content;
    }

    [CreateAssetMenu(menuName = "Localization/String Table")]
    public class StringTable : ScriptableObject
    {
        [SerializeField] string _languageCode;
        [SerializeField] string _namespace;
        [SerializeField] List<LocalizedStringData> _stringDatas = new();

        public string LanguageCode => _languageCode;
        public string Namespace => _namespace;

#if UNITY_EDITOR
        public void SetMetaData(string languageCode, string ns)
        {
            _languageCode = languageCode;
            _namespace = ns;
        }
#endif

        public IEnumerable<LocalizedStringData> GetStringDatas() => _stringDatas;
        public void ClearString() => _stringDatas.Clear();

        public bool IsValidKeyFormat(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            return Regex.IsMatch(key, @"^[a-z0-9_]+$");
        }

        public void AddString(string key, string content)
        {
            if (!IsValidKeyFormat(key))
            {
                Debug.LogError($"Key '{key}' does not match the required format (lowercase, numbers, and underscores only, no namespace prefix).");
                return;
            }

            if (_stringDatas.FindIndex(x => x.key == key) == -1)
            {
                _stringDatas.Add(new LocalizedStringData { key = key, content = content });
            }
            else
            {
                Debug.LogError($"Has key conflict when AddString(): {key}");
            }
        }
    }
}
