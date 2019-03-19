using System;
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

        public static void FindAllInstances<T> (Object root, Action<T, string> callback, Func<Object, string, bool> validate = null, string path = "root", int depthLimit = 10) where T : class {
            FindAllInstances_Internal (root, new HashSet<Object> (), callback, validate, path, depthLimit);
        }

        static void FindAllInstances_Internal<T> (Object instance, HashSet<Object> travledObjects, Action<T, string> callback, Func<Object, string, bool> validate, string path, int depthLimit) where T : class {
            if (instance == null) return;
            if (depthLimit < 1) return;
            if (travledObjects.Contains (instance)) return;

            travledObjects.Add (instance);

            var target = instance as T;
            if (target != null) {
                callback.Invoke (target, path);
            }

            if (validate != null && !validate.Invoke (instance, path)) {
                return;
            }

            var collection = instance as IEnumerable;
            if (collection != null) {
                int index = 0;
                foreach (var item in collection) {
                    FindAllInstances_Internal (item, travledObjects, callback, validate, string.Format ("{0}[{1}]", path, index), depthLimit - 1);
                    index++;
                }
            } else {
                var instanceType = instance.GetType ();
                var fields = instanceType.GetFields (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields) {
                    FindAllInstances_Internal (field.GetValue (instance), travledObjects, callback, validate, string.Format ("{0}/{1}", path, field.Name), depthLimit - 1);
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