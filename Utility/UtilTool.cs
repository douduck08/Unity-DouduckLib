using UnityEngine;
using System.Collections;

namespace DouduckGame.Util
{
	public partial class UtilTool {
		public static void ShowDeviceInfomation() {
			Debug.Log("[SysInfo] Device Type: " + SystemInfo.deviceType);
			Debug.Log("[SysInfo] Memory Size: " + SystemInfo.systemMemorySize);
			Debug.Log("[SysInfo] Accelerometer Support: " + SystemInfo.supportsAccelerometer);
			Debug.Log("[SysInfo] Gyroscope Support: " + SystemInfo.supportsGyroscope);
			Debug.Log("[SysInfo] Local IP: " + Network.NetworkUtil.LocalIP);
		}

        public static string ReadTextFile(string sPath, string sFileName) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            sPath = Application.dataPath + "/Resources/" + sPath;
            System.IO.StreamReader sr_ = null;
            sr_ = System.IO.File.OpenText(sPath + sFileName);

#elif UNITY_ANDROID || UNITY_IPHONE
	        sFileName = System.IO.Path.GetFileNameWithoutExtension(sFileName);
	        TextAsset logFile = Resources.Load<TextAsset>(sPath + sFileName);
	        System.IO.StringReader sr_ = null;
	        sr_ = new System.IO.StringReader(logFile.text);
#endif
            if (sr_ == null) {
                return null;
            }
            string sText_ = sr_.ReadToEnd();
            sr_.Close();
            sr_.Dispose();
            return sText_;
        }
    }
}
