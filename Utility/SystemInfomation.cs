using UnityEngine;
using System.Collections;

namespace DouduckGame.Utility
{
	public static class SystemInfomation
	{

		public static void ShowInfomation() {
			Debug.Log("[SysInfo] Device Type: " + SystemInfo.deviceType);
			Debug.Log("[SysInfo] Memory Size: " + SystemInfo.systemMemorySize);
			Debug.Log("[SysInfo] Accelerometer Support: " + SystemInfo.supportsAccelerometer);
			Debug.Log("[SysInfo] Gyroscope Support: " + SystemInfo.supportsGyroscope);
			Debug.Log("[SysInfo] Local IP: " + Network.NetworkUtil.LocalIP);
		}
	}
}
