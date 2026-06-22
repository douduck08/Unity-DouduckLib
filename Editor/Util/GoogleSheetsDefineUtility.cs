using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DouduckLibEditor
{
    [InitializeOnLoad]
    public static class GoogleSheetsDefineUtility
    {
        const string DefineSymbol = "DDLIB_GOOGLE_SHEETS";

        static GoogleSheetsDefineUtility()
        {
            EditorApplication.delayCall += CheckAndConfigureDefineSymbol;
        }

        static void CheckAndConfigureDefineSymbol()
        {
            var hasSheetsApi = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name == "Google.Apis.Sheets.v4");

            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (targetGroup == BuildTargetGroup.Unknown)
            {
                return;
            }

            var namedTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var definesString = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
            var defines = definesString.Split(';')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            var hasDefine = defines.Contains(DefineSymbol);

            if (hasSheetsApi && !hasDefine)
            {
                defines.Add(DefineSymbol);
                var newDefines = string.Join(";", defines);
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, newDefines);
                Debug.Log($"[DouduckLib] Added scripting define symbol '{DefineSymbol}' because Google Sheets API was detected.");
            }
            else if (!hasSheetsApi && hasDefine)
            {
                defines.Remove(DefineSymbol);
                var newDefines = string.Join(";", defines);
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, newDefines);
                Debug.Log($"[DouduckLib] Removed scripting define symbol '{DefineSymbol}' because Google Sheets API was not detected.");
            }
        }
    }
}
