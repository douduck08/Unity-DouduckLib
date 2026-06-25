#if DDLIB_GOOGLE_SHEETS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using DouduckLib;
using DouduckLib.Localization;
using DouduckLib.Wrapper;

namespace DouduckLibEditor.Localization
{
    [CreateAssetMenu(menuName = "DouduckLib/Localization - String Table Processor")]
    public class StringTableProcessor : ScriptableObject
    {
        [Serializable]
        public struct LanguageColumnConfig
        {
            public string columnName;
            public string languageCode;
        }

        [Serializable]
        public struct TabConfig
        {
            public string tabName;
            [DisplayName("Namespace")] public string ns;
        }

        [SerializeField] GoogleSheetsService _googleSheetsSource;

        [Header("Data Table Config")]
        [SerializeField] string _keyColumnName = "id";
        [SerializeField] List<LanguageColumnConfig> _languageColumnConfigs = new();
        [SerializeField] List<TabConfig> _tabs = new();

        [Header("Output")]
        [SerializeField] LocalizationSettings _localizationSettings;
        [SerializeField] string _stringTableOutputFolder = "Assets/Contents/Localization";

        class RowData
        {
            public string id;
            public string[] texts;
        }

        class RowDataBuilder : ISheetRowBuilder<RowData>
        {
            public string[] Header { get; }

            public RowDataBuilder(string keyHeader, IEnumerable<string> columnNames)
            {
                var headerList = new List<string> { keyHeader };
                headerList.AddRange(columnNames);
                Header = headerList.ToArray();
            }

            public RowData Build(string[] values)
            {
                return new RowData() {
                    id = values[0]?.Trim(),
                    texts = values.Skip(1).ToArray()
                };
            }
        }

        [Button("Process")]
        public void ProcessButton() => Process();

        [Button]
        public void CleanProcess()
        {
            if (_localizationSettings != null)
            {
                _localizationSettings.ClearStringTable();
            }
            Process();
        }

        public void Process(bool saveAsset = true)
        {
            if (_googleSheetsSource == null)
            {
                Debug.LogError("Sheets Service has not been configured!");
                return;
            }

            if (_localizationSettings == null)
            {
                Debug.LogError("Localization Settings has not been configured!");
                return;
            }

            var stringTables = new List<StringTable>();

            foreach (var tabConfig in _tabs)
            {
                var tabName = tabConfig.tabName;
                var ns = tabConfig.ns.ToLower();

                // 1. Sanity Check: Read raw header first to verify configuration
                var valueRange = _googleSheetsSource.GetValueRange(tabName);
                if (valueRange == null || valueRange.Values == null || valueRange.Values.Count == 0)
                {
                    Debug.LogError($"Cannot read spreadsheet values for tab: {tabName}");
                    continue;
                }

                var sheetHeader = valueRange.Values.First().Cast<string>().Select(s => s.ToLower()).ToList();

                // Verify Key Header
                if (!sheetHeader.Contains(_keyColumnName.ToLower()))
                {
                    Debug.LogError($"Spreadsheet tab '{tabName}' does not contain key header: '{_keyColumnName}'");
                    continue;
                }

                // Verify Language Columns
                var hasMissedHeader = false;
                foreach (var config in _languageColumnConfigs)
                {
                    if (!sheetHeader.Contains(config.columnName.ToLower()))
                    {
                        Debug.LogError($"Spreadsheet tab '{tabName}' does not contain language column: '{config.columnName}'");
                        hasMissedHeader = true;
                    }
                }
                if (hasMissedHeader)
                {
                    continue;
                }

                // 2. Generic Reading & Parsing using custom RowDataBuilder
                var builder = new RowDataBuilder(_keyColumnName, _languageColumnConfigs.Select(c => c.columnName));
                var rowDatas = valueRange.GetSheetRow(builder)
                                         .Where(data => !string.IsNullOrEmpty(data.id));

                if (rowDatas != null)
                {
                    for (int index = 0; index < _languageColumnConfigs.Count; ++index)
                    {
                        var config = _languageColumnConfigs[index];
                        var languageCode = config.languageCode;
                        if (string.IsNullOrEmpty(languageCode))
                        {
                            continue;
                        }

                        var fileName = $"{tabName}.{languageCode}";
                        var stringTableAsset = AssetDatabaseUtil.FindOrCreateScriptableObject<StringTable>(_stringTableOutputFolder, fileName);
                        if (stringTableAsset != null)
                        {
                            stringTableAsset.SetMetaData(languageCode, ns);
                            stringTableAsset.ClearString();

                            foreach (var rowData in rowDatas)
                            {
                                var key = rowData.id.ToLower();
                                var content = rowData.texts[index];
                                if (!string.IsNullOrEmpty(content))
                                {
                                    stringTableAsset.AddString(key, content);
                                }
                            }
                            EditorUtility.SetDirty(stringTableAsset);

                            stringTables.Add(stringTableAsset);
                        }
                    }
                }
            }

            _localizationSettings.AddStringTable(stringTables);

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(_localizationSettings);
            if (saveAsset)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif
