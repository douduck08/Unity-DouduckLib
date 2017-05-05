using UnityEngine;

namespace DouduckGame.Util {
	public partial class UtilTool {
		public static void ShowDeviceInfomation() {
			UnityConsole.Log("[SysInfo] Device Type: " + SystemInfo.deviceType);
            UnityConsole.Log("[SysInfo] Memory Size: " + SystemInfo.systemMemorySize);
            UnityConsole.Log("[SysInfo] Accelerometer Support: " + SystemInfo.supportsAccelerometer);
            UnityConsole.Log("[SysInfo] Gyroscope Support: " + SystemInfo.supportsGyroscope);
            UnityConsole.Log("[SysInfo] Local IP: " + Network.NetworkUtil.LocalIP);
		}
    }
}
