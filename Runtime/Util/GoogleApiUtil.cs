#if DDLIB_GOOGLE_SHEETS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DouduckLib.Wrapper
{
    [Serializable]
    public class GoogleSheetsService
    {
        [SerializeField] string _applicationName;
        [SerializeField] string _apiKey;
        [SerializeField] string _spreadsheetId;
        [SerializeField] string _spreadsheetRange;

        SheetsService _service;

        public string SpreadsheetId => _spreadsheetId;
        public string SpreadsheetRange => _spreadsheetRange;

        public static GoogleSheetsService Create(string applicationName, string apiKey, string spreadsheetId, string spreadsheetRange = null)
        {
            var service = new GoogleSheetsService {
                _applicationName = applicationName,
                _apiKey = apiKey,
                _spreadsheetId = spreadsheetId,
                _spreadsheetRange = spreadsheetRange
            };
            return service;
        }

        public void InitializeIfNeeded()
        {
            _service ??= new SheetsService(new BaseClientService.Initializer {
                ApplicationName = _applicationName,
                ApiKey = _apiKey,
            });
        }

        // Pure Connection I/O: get raw value range from sheet
        public ValueRange GetValueRange(string tabName, string range = null)
        {
            InitializeIfNeeded();

            if (string.IsNullOrEmpty(range))
            {
                range = _spreadsheetRange;
            }

            var tabAndRange = tabName;
            if (!string.IsNullOrEmpty(range))
            {
                tabAndRange = $"{tabName}!{range}";
            }

            var request = _service.Spreadsheets.Values.Get(_spreadsheetId, tabAndRange);
            return request.Execute();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SheetColumnAttribute : Attribute
    {
        public string ColumnName { get; }
        public SheetColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    public interface ISheetRowBuilder<T>
    {
        string[] Header { get; }
        T Build(string[] values);
    }

    public static class GoogleSheetsServiceExtension
    {
        // Public Overload 1: Clean builder class version (useful for dynamic arrays like localization)
        public static IEnumerable<T> GetSheetRow<T>(this ValueRange valueRange, ISheetRowBuilder<T> builder)
        {
            if (valueRange == null || builder == null)
            {
                return Enumerable.Empty<T>();
            }
            return valueRange.GetSheetRowInternal(builder.Header, builder.Build);
        }

        // Public Overload 2: Reflection zero-parameter version (useful for item databases with [SheetColumn], header-free)
        public static IEnumerable<T> GetSheetRow<T>(this ValueRange valueRange) where T : new()
        {
            if (valueRange == null || valueRange.Values == null || valueRange.Values.Count == 0)
            {
                return Enumerable.Empty<T>();
            }

            var type = typeof(T);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Collect all column names specified by [SheetColumn] attribute
            var headerList = new List<string>();
            var columnMappings = new Dictionary<string, Action<T, string>>();

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof(SheetColumnAttribute), true)
                                .FirstOrDefault() as SheetColumnAttribute;
                if (attr != null)
                {
                    headerList.Add(attr.ColumnName);
                    columnMappings[attr.ColumnName.ToLower()] = (obj, val) => {
                        try
                        {
                            var parsedVal = Convert.ChangeType(val, field.FieldType);
                            field.SetValue(obj, parsedVal);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error converting value '{val}' to type {field.FieldType} for field {field.Name}: {ex.Message}");
                        }
                    };
                }
            }

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(SheetColumnAttribute), true)
                               .FirstOrDefault() as SheetColumnAttribute;
                if (attr != null && prop.CanWrite)
                {
                    headerList.Add(attr.ColumnName);
                    columnMappings[attr.ColumnName.ToLower()] = (obj, val) => {
                        try
                        {
                            var parsedVal = Convert.ChangeType(val, prop.PropertyType);
                            prop.SetValue(obj, parsedVal, null);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error converting value '{val}' to type {prop.PropertyType} for property {prop.Name}: {ex.Message}");
                        }
                    };
                }
            }

            var header = headerList.ToArray();

            return valueRange.GetSheetRowInternal(header,
                (values) => {
                    var data = new T();
                    for (int i = 0; i < header.Length; i++)
                    {
                        var columnName = header[i];
                        if (string.IsNullOrEmpty(columnName)) continue;

                        var key = columnName.ToLower();
                        if (columnMappings.ContainsKey(key))
                        {
                            columnMappings[key](data, values[i]);
                        }
                    }
                    return data;
                }
            );
        }

        // Private Core parsing engine
        internal static IEnumerable<T> GetSheetRowInternal<T>(this ValueRange valueRange, string[] header, Func<string[], T> builder)
        {
            if (header == null || valueRange == null || valueRange.Values == null || valueRange.Values.Count == 0)
            {
                yield break;
            }

            var firstRow = valueRange.Values.First().Cast<string>().Select(s => s.ToLower()).ToList();
            var indexes = new int[header.Length];
            for (int i = 0; i < header.Length; i++)
            {
                var columnName = header[i];
                if (!string.IsNullOrEmpty(columnName))
                {
                    columnName = columnName.ToLower();
                    indexes[i] = firstRow.FindIndex(s => s == columnName);
                }
                else
                {
                    indexes[i] = -1;
                }
            }

            var values = new string[header.Length];
            var rows = valueRange.Values.Skip(1).ToList();
            foreach (var row in rows)
            {
                var rowData = row.Select(o => o as string);
                for (int i = 0; i < header.Length; i++)
                {
                    values[i] = rowData.ElementAtOrDefault(indexes[i]);
                }
                var data = builder.Invoke(values);
                yield return data;
            }
        }
    }
}
#endif
