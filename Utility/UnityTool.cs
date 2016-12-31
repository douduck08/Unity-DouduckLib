using UnityEngine;
using System.Collections;

namespace DouduckGame.Utility
{
	public static class UnityTool
	{
		public static GameObject FindGameobjectWithTag(string sTag) {
			Debug.Log("[UnityTool] Find Gameobject with Tag: " + sTag);
			GameObject go_ = GameObject.FindWithTag(sTag);
			if (go_ == null) {
				Debug.LogError("[UnityTool] Cannot find Gameobject with Tag: " + sTag);
				return null;
			}
			return go_;
		}

		public static GameObject FindGameobject(string sName) {
			GameObject go_ = GameObject.Find(sName);
			if(go_==null) {
				Debug.LogError("[UnityTool] Cannot find Gameobject: " + sName);
				return null;
			}
			return go_;
		}

		public static GameObject FindChildGameObject(GameObject oParent, string sName) {
			if (oParent == null) {
				Debug.LogError("[UnityTool] oParent is null");
				return null;
			}

			Transform oTf_ = null;
			if(oParent.name == sName) {
				oTf_ = oParent.transform;
			} else {
				Transform[] allChildren = oParent.transform.GetComponentsInChildren<Transform>();
				foreach (Transform child in allChildren) {
					if (child.name == sName) {
						if(oTf_ == null) {
							oTf_ = child;
						} else {
							Debug.LogError("[UnityTool] Find more then one Gameobject[" + sName + "] under " + oParent.name);
						}
					}
				}
			}
			if (oTf_ == null) {
				Debug.LogError("[UnityTool] Find no Gameobject[" + sName + "] under " + oParent.name);
				return null;
			}
			return oTf_.gameObject;
		}


		
	}
}
