using System;
using System.Collections.Generic;
using System.Reflection;

namespace DouduckLib.Save
{
    public static class SaveMapper
    {
        static readonly Dictionary<(Type, Type), List<(FieldInfo, FieldInfo)>> _fieldMapCache = new();
        static readonly Dictionary<(Type, Type), List<(PropertyInfo, PropertyInfo)>> _propertyMapCache = new();

        static string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            return name.TrimStart('_').ToLower();
        }

        public static void Copy(object source, object target)
        {
            if (source == null || target == null) return;

            var sourceType = source.GetType();
            var targetType = target.GetType();
            var key = (sourceType, targetType);

            // 1. Copy Fields
            if (!_fieldMapCache.TryGetValue(key, out var fieldMappings))
            {
                fieldMappings = new List<(FieldInfo, FieldInfo)>();
                var sourceFields = sourceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var targetFields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                var targetFieldDict = new Dictionary<string, FieldInfo>();
                foreach (var tf in targetFields)
                {
                    var normalized = NormalizeName(tf.Name);
                    targetFieldDict[normalized] = tf;
                }

                foreach (var sf in sourceFields)
                {
                    var normalized = NormalizeName(sf.Name);
                    if (targetFieldDict.TryGetValue(normalized, out var tf))
                    {
                        if (tf.FieldType.IsAssignableFrom(sf.FieldType))
                        {
                            fieldMappings.Add((sf, tf));
                        }
                    }
                }

                _fieldMapCache[key] = fieldMappings;
            }

            foreach (var mapping in fieldMappings)
            {
                var val = mapping.Item1.GetValue(source);
                mapping.Item2.SetValue(target, val);
            }

            // 2. Copy Properties
            if (!_propertyMapCache.TryGetValue(key, out var propertyMappings))
            {
                propertyMappings = new List<(PropertyInfo, PropertyInfo)>();
                var sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var targetProperties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                var targetPropDict = new Dictionary<string, PropertyInfo>();
                foreach (var tp in targetProperties)
                {
                    var normalized = NormalizeName(tp.Name);
                    targetPropDict[normalized] = tp;
                }

                foreach (var sp in sourceProperties)
                {
                    var normalized = NormalizeName(sp.Name);
                    if (sp.CanRead && targetPropDict.TryGetValue(normalized, out var tp))
                    {
                        if (tp.CanWrite && tp.PropertyType.IsAssignableFrom(sp.PropertyType))
                        {
                            if (sp.GetIndexParameters().Length == 0 && tp.GetIndexParameters().Length == 0)
                            {
                                propertyMappings.Add((sp, tp));
                            }
                        }
                    }
                }

                _propertyMapCache[key] = propertyMappings;
            }

            foreach (var mapping in propertyMappings)
            {
                try
                {
                    var val = mapping.Item1.GetValue(source, null);
                    mapping.Item2.SetValue(target, val, null);
                }
                catch (Exception)
                {
                    // Ignore exceptions during property copying
                }
            }
        }
    }
}
