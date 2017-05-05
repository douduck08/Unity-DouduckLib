using UnityEngine;
using System.Collections;

namespace DouduckGame.Util {
    public partial class UtilTool {
        public static GameObject FindGameobjectWithTag (string sTag) {
            GameObject go_ = GameObject.FindWithTag (sTag);
            if (go_ == null) {
                UnityConsole.LogError ("[UnityTool] Cannot find Gameobject with Tag: " + sTag);
                return null;
            }
            return go_;
        }

        public static GameObject FindGameobject (string sName) {
            GameObject go_ = GameObject.Find (sName);
            if (go_ == null) {
                UnityConsole.LogError ("[UnityTool] Cannot find Gameobject: " + sName);
                return null;
            }
            return go_;
        }

        public static GameObject FindChildGameObject (GameObject oParent, string sName) {
            if (oParent == null) {
                UnityConsole.LogError ("[UnityTool] oParent is null");
                return null;
            }

            Transform oTf_ = null;
            if (oParent.name == sName) {
                oTf_ = oParent.transform;
            } else {
                Transform[] allChildren = oParent.transform.GetComponentsInChildren<Transform> ();
                foreach (Transform child in allChildren) {
                    if (child.name == sName) {
                        if (oTf_ == null) {
                            oTf_ = child;
                        } else {
                            UnityConsole.LogError ("[UnityTool] Find more then one Gameobject[" + sName + "] under " + oParent.name);
                        }
                    }
                }
            }
            if (oTf_ == null) {
                UnityConsole.LogError ("[UnityTool] Find no Gameobject[" + sName + "] under " + oParent.name);
                return null;
            }
            return oTf_.gameObject;
        }
    }
}
