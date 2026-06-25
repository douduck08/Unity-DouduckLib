using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using DouduckLib.Localization;

namespace DouduckLibEditor.Localization
{
    public class LanguageProgressReport
    {
        public string LanguageCode;
        public string DisplayName;
        public List<NamespaceProgressReport> NamespaceReports = new();
    }

    public class NamespaceProgressReport
    {
        public string Namespace;
        public StringTable TableAsset;
        public int TotalKeys;
        public int TranslatedCount;
        public List<string> MissingKeys = new();
    }

    public class LocalizationProgressWindow : EditorWindow
    {
        [MenuItem("Tools/DouduckLib/Localization Progress Checker", false, 40)]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationProgressWindow>("Localization Progress");
            window.Show();
        }

        public static void ShowWindow(LocalizationSettings settings)
        {
            var window = GetWindow<LocalizationProgressWindow>("Localization Progress");
            window._settings = settings;
            window.AnalyzeProgress();
            window.Show();
        }

        [MenuItem("Tools/DouduckLib/Reload String Tables", false, 40)]
        public static void ReloadStringTablesMenuItem()
        {
            var settings = Resources.Load<LocalizationSettings>("LocalizationSettings");
            if (settings != null)
            {
                settings.ReloadStringTables();
                Debug.Log("Successfully reloaded string tables.");
            }
            else
            {
                var guids = AssetDatabase.FindAssets("t:LocalizationSettings");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var asset = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
                    if (asset != null)
                    {
                        asset.ReloadStringTables();
                        Debug.Log($"Successfully reloaded string tables using asset at: {path}");
                    }
                }
                else
                {
                    Debug.LogError("Cannot find any LocalizationSettings asset in the project.");
                }
            }
        }

        LocalizationSettings _settings;
        Vector2 _scrollPosition;

        Dictionary<string, bool> _languageFoldouts = new();
        Dictionary<string, Dictionary<string, bool>> _namespaceFoldouts = new();
        string _searchQuery = "";

        List<LanguageProgressReport> _reports = new();
        bool _hasAnalyzed;

        void OnEnable()
        {
            if (_settings == null)
            {
                var guids = AssetDatabase.FindAssets("t:LocalizationSettings");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
                }
            }
            AnalyzeProgress();
        }

        void OnGUI()
        {
            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                using (new EditorGUILayout.VerticalScope())
                {
                    DrawMainGUI();
                }
                GUILayout.Space(10);
            }
        }

        void DrawMainGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                var prevSettings = _settings;
                _settings = (LocalizationSettings)EditorGUILayout.ObjectField("Settings Asset", _settings, typeof(LocalizationSettings), false);
                if (prevSettings != _settings)
                {
                    AnalyzeProgress();
                }

                if (GUILayout.Button("Refresh", GUILayout.Width(80)))
                {
                    AnalyzeProgress();
                }
            }

            if (_settings == null)
            {
                EditorGUILayout.HelpBox("Please assign a LocalizationSettings asset.", MessageType.Info);
                return;
            }

            if (!_hasAnalyzed)
            {
                AnalyzeProgress();
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Filter Keys:", GUILayout.Width(70));
                _searchQuery = EditorGUILayout.TextField(_searchQuery);
                if (GUILayout.Button("Clear", GUILayout.Width(50)))
                {
                    _searchQuery = "";
                    GUI.FocusControl(null);
                }
            }

            GUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var report in _reports)
            {
                DrawLanguageReport(report);
                GUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();
        }

        void AnalyzeProgress()
        {
            _reports.Clear();
            _hasAnalyzed = false;

            if (_settings == null)
            {
                return;
            }

            var languageCodes = _settings.GetAllLanguageCode().ToList();

            var field = typeof(LocalizationSettings).GetField("_stringTables", BindingFlags.NonPublic | BindingFlags.Instance);
            var allTables = field?.GetValue(_settings) as List<StringTable> ?? new List<StringTable>();
            allTables = allTables.Where(t => t != null).ToList();

            if (languageCodes.Count == 0 && allTables.Count > 0)
            {
                languageCodes = allTables.Select(t => t.LanguageCode).Distinct().ToList();
            }

            var namespaces = allTables
                .Select(t => t.Namespace)
                .Where(ns => !string.IsNullOrEmpty(ns))
                .Distinct()
                .ToList();

            var keysPerNamespace = new Dictionary<string, HashSet<string>>();
            foreach (var ns in namespaces)
            {
                var keys = new HashSet<string>();
                var nsTables = allTables.Where(t => t.Namespace == ns);
                foreach (var table in nsTables)
                {
                    foreach (var data in table.GetStringDatas())
                    {
                        if (!string.IsNullOrEmpty(data.key))
                        {
                            keys.Add(data.key);
                        }
                    }
                }
                keysPerNamespace[ns] = keys;
            }

            foreach (var code in languageCodes)
            {
                var displayName = _settings.GetLanguageDisplayName(code);
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = code;
                }

                var report = new LanguageProgressReport {
                    LanguageCode = code,
                    DisplayName = displayName
                };

                foreach (var ns in namespaces)
                {
                    var allKeys = keysPerNamespace[ns];
                    var table = allTables.FirstOrDefault(t => t.LanguageCode == code && t.Namespace == ns);

                    var nsReport = new NamespaceProgressReport {
                        Namespace = ns,
                        TableAsset = table,
                        TotalKeys = allKeys.Count
                    };

                    if (table != null)
                    {
                        var translatedKeys = table.GetStringDatas()
                            .Where(d => !string.IsNullOrWhiteSpace(d.content))
                            .Select(d => d.key)
                            .ToHashSet();

                        nsReport.TranslatedCount = translatedKeys.Count;
                        nsReport.MissingKeys = allKeys.Where(k => !translatedKeys.Contains(k)).ToList();
                    }
                    else
                    {
                        nsReport.TranslatedCount = 0;
                        nsReport.MissingKeys = allKeys.ToList();
                    }

                    report.NamespaceReports.Add(nsReport);
                }

                _reports.Add(report);
            }

            _hasAnalyzed = true;
        }

        void DrawLanguageReport(LanguageProgressReport report)
        {
            int totalKeys = report.NamespaceReports.Sum(r => r.TotalKeys);
            int translatedCount = report.NamespaceReports.Sum(r => r.TranslatedCount);
            float progress = totalKeys > 0 ? (float)translatedCount / totalKeys : 1f;

            if (!_languageFoldouts.ContainsKey(report.LanguageCode))
            {
                _languageFoldouts[report.LanguageCode] = false;
            }

            using (new GUILayout.HorizontalScope())
            {
                var title = $"{report.DisplayName} ({report.LanguageCode})";
                _languageFoldouts[report.LanguageCode] = EditorGUILayout.Foldout(_languageFoldouts[report.LanguageCode], title, true, EditorStyles.foldoutHeader);

                GUILayout.Label($"{translatedCount}/{totalKeys} ({(progress * 100f):F1}%)", GUILayout.Width(120));

                var rect = GUILayoutUtility.GetRect(100, 18, GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(rect, progress, "");

                GUILayout.Space(5);
            }

            if (_languageFoldouts[report.LanguageCode])
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (!_namespaceFoldouts.ContainsKey(report.LanguageCode))
                    {
                        _namespaceFoldouts[report.LanguageCode] = new Dictionary<string, bool>();
                    }

                    var nsFoldouts = _namespaceFoldouts[report.LanguageCode];

                    foreach (var nsReport in report.NamespaceReports)
                    {
                        var filteredMissingKeys = nsReport.MissingKeys;
                        if (!string.IsNullOrEmpty(_searchQuery))
                        {
                            filteredMissingKeys = nsReport.MissingKeys
                                .Where(k => k.Contains(_searchQuery.ToLower()))
                                .ToList();
                        }

                        bool isNamespaceMatched = nsReport.Namespace.Contains(_searchQuery.ToLower());
                        if (!string.IsNullOrEmpty(_searchQuery) && !isNamespaceMatched && filteredMissingKeys.Count == 0)
                        {
                            continue;
                        }

                        if (!nsFoldouts.ContainsKey(nsReport.Namespace))
                        {
                            nsFoldouts[nsReport.Namespace] = false;
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            var colorCache = GUI.color;
                            if (nsReport.MissingKeys.Count > 0)
                            {
                                GUI.color = new Color(1f, 0.75f, 0.3f);
                            }
                            else
                            {
                                GUI.color = new Color(0.6f, 1f, 0.6f);
                            }

                            var nsTitle = $"Namespace: {nsReport.Namespace} ({nsReport.TranslatedCount}/{nsReport.TotalKeys})";
                            nsFoldouts[nsReport.Namespace] = EditorGUILayout.Foldout(nsFoldouts[nsReport.Namespace], nsTitle, true);

                            GUI.color = colorCache;

                            if (nsReport.TableAsset != null)
                            {
                                if (GUILayout.Button("Ping Table", GUILayout.Width(80)))
                                {
                                    EditorGUIUtility.PingObject(nsReport.TableAsset);
                                    Selection.activeObject = nsReport.TableAsset;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Create Table", GUILayout.Width(90)))
                                {
                                    CreateStringTableForNamespace(report.LanguageCode, nsReport.Namespace);
                                }
                            }
                        }

                        if (nsFoldouts[nsReport.Namespace])
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                if (nsReport.TableAsset == null)
                                {
                                    EditorGUILayout.HelpBox($"String Table is missing for Language: {report.LanguageCode}, Namespace: {nsReport.Namespace}", MessageType.Warning);
                                }
                                else if (nsReport.MissingKeys.Count == 0)
                                {
                                    EditorGUILayout.HelpBox("All keys are translated!", MessageType.Info);
                                }
                                else
                                {
                                    EditorGUILayout.LabelField($"Missing Keys ({filteredMissingKeys.Count}):", EditorStyles.boldLabel);
                                    foreach (var missingKey in filteredMissingKeys)
                                    {
                                        using (new GUILayout.HorizontalScope())
                                        {
                                            EditorGUILayout.SelectableLabel(missingKey, GUILayout.Height(16));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void CreateStringTableForNamespace(string languageCode, string ns)
        {
            string folderPath = "Assets/Contents/Localization";

            var field = typeof(LocalizationSettings).GetField("_stringTables", BindingFlags.NonPublic | BindingFlags.Instance);
            var allTables = field?.GetValue(_settings) as List<StringTable> ?? new List<StringTable>();
            allTables = allTables.Where(t => t != null).ToList();

            var otherLangTable = allTables.FirstOrDefault(t => t.Namespace == ns);
            if (otherLangTable != null)
            {
                var path = AssetDatabase.GetAssetPath(otherLangTable);
                if (!string.IsNullOrEmpty(path))
                {
                    folderPath = Path.GetDirectoryName(path).Replace("\\", "/");
                }
            }
            else if (allTables.Count > 0)
            {
                var anyTable = allTables.FirstOrDefault();
                if (anyTable != null)
                {
                    var path = AssetDatabase.GetAssetPath(anyTable);
                    if (!string.IsNullOrEmpty(path))
                    {
                        folderPath = Path.GetDirectoryName(path).Replace("\\", "/");
                    }
                }
            }

            var fileName = $"{ns}.{languageCode}";

            var newTable = AssetDatabaseUtil.FindOrCreateScriptableObject<StringTable>(folderPath, fileName);
            if (newTable != null)
            {
                newTable.SetMetaData(languageCode, ns);
                EditorUtility.SetDirty(newTable);

                _settings.AddStringTable(newTable);
                EditorUtility.SetDirty(_settings);

                AssetDatabase.SaveAssets();
                AnalyzeProgress();

                Debug.Log($"Successfully created string table: {fileName} at {folderPath}");
            }
        }
    }
}
