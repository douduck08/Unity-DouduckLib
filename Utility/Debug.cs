using UnityEngine;
using System.Collections;

#if DEBUG_SWITCH
public static class Debug {
	public static bool DebugMode = false;

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
#endif
