using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace DouduckLib
{
    public static class CollectionExtension
    {
        public static void AddIfNotNull<T>(this List<T> list, T? element) where T : class
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (element != null)
            {
                list.Add(element);
            }
        }

        public static void AddIfNotNull<T>(this List<T> list, T? element) where T : struct
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (element.HasValue)
            {
                list.Add(element.Value);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            // null return true
            // array use Length, list use Count for better performance
            return source switch {
                null => true,
                T[] array => array.Length == 0,
                ICollection<T> collection => collection.Count == 0,
                IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count == 0,
                _ => !source.Any()
            };
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static bool TryGetRandom<T>(this IList<T> list, out T result)
        {
            if (list == null || list.Count == 0)
            {
                result = default!;
                return false;
            }
            result = list[UnityEngine.Random.Range(0, list.Count)];
            return true;
        }
    }
}
