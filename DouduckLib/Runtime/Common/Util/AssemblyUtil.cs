using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        public static T GetAttribute<T> (this Type type) where T : Attribute {
            Assert.IsTrue (type.IsDefined (typeof (T), false), "Attribute not found");
            return (T) type.GetCustomAttributes (typeof (T), false) [0];
        }
    }
}