using UnityEngine;
using System.Collections;

namespace DouduckGame.Util {

    public static class UnityConsole {
        public static bool DebugMode = true;

        public static void Log(object message) {
            if (DebugMode) {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void LogWarning(object message) {
            if (DebugMode) {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void LogError(object message) {
            if (DebugMode) {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}
