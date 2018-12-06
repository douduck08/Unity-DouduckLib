﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Object = System.Object;

namespace DouduckLib {
    public static class AssemblyUtil {

        static IEnumerable<Type> m_AssemblyTypes;

        public static IEnumerable<Type> GetAllAssemblyTypes () {
            if (m_AssemblyTypes == null) {
                m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies ()
                    .SelectMany (t => {
                        // Ugly hack to handle mis-versioned dlls
                        var innerTypes = new Type[0];
                        try {
                            innerTypes = t.GetTypes ();
                        } catch { }
                        return innerTypes;
                    });
            }
            return m_AssemblyTypes;
        }

        public static void RunTaskForAllFields<T> (Object instance, Action<string, T> task, string rootName = "unknown", bool includeNested = true, int depthLimit = 10) {
            RunTaskForAllFields_Internal (instance, new HashSet<Object> (), task, rootName, includeNested, depthLimit);
        }

        static void RunTaskForAllFields_Internal<T> (Object instance, HashSet<Object> travledObjects, Action<string, T> task, string pathName, bool includeNested, int depthLimit) {
            if (depthLimit < 1) return;
            if (travledObjects.Contains (instance)) return;

            travledObjects.Add (instance);

            // var type = typeof (T);
            // var enumerableType = typeof (IEnumerable<T>);
            var instanceType = instance.GetType ();
            var fields = instanceType.GetFields (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields) {
                if (typeof (IEnumerable<T>).IsAssignableFrom (field.FieldType)) {
                    var collection = field.GetValue (instance) as IList<T>;
                    int index = 0;
                    foreach (var item in collection) {
                        task.Invoke (string.Format ("{0}/{1}[{2}]", pathName, field.Name, index), item);
                        index++;
                    }
                } else if (typeof (T).IsAssignableFrom (field.FieldType)) {
                    var item = (T) field.GetValue (instance);
                    task.Invoke (string.Format ("{0}/{1}", pathName, field.Name), item);
                } else if (includeNested) {
                    var isEnumerable = field.FieldType.GetInterfaces ().Any (x => x.IsGenericType && x.GetGenericTypeDefinition () == typeof (IEnumerable<>));
                    var isCollection = field.FieldType.GetInterfaces ().Any (x => x.IsGenericType && x.GetGenericTypeDefinition () == typeof (ICollection<>));
                    if (isEnumerable && isCollection) {
                        var fieldObject = field.GetValue (instance);
                        var count = (int) field.FieldType.GetProperty ("Count").GetValue (fieldObject, null);
                        for (int index = 0; index < count; index++) {
                            var item = field.FieldType.GetProperty ("Item").GetValue (fieldObject, new Object[] { index });
                            RunTaskForAllFields_Internal (item, travledObjects, task, string.Format ("{0}/{1}[{2}]", pathName, field.Name, index), includeNested, depthLimit - 1);
                        }
                    } else {
                        RunTaskForAllFields_Internal (field.GetValue (instance), travledObjects, task, string.Format ("{0}/{1}", pathName, field.Name), includeNested, depthLimit - 1);
                    }
                }
            }
        }

    }

    public static class AssemblyExtenstion {
        public static T GetAttribute<T> (this Type type) where T : Attribute {
            Assert.IsTrue (type.IsDefined (typeof (T), false), "Attribute not found");
            return (T) type.GetCustomAttributes (typeof (T), false) [0];
        }
    }
}