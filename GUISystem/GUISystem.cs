using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public sealed class GUISystem : IGameSystemMono {

		private List<IGUIUnit> m_UIUnitList;

		public override void StartGameSystem () {
			m_UIUnitList = new List<IGUIUnit> ();
		}

		public override void DestoryGameSystem () {

		}

		public void AddGUIUnit(IGUIUnit Unit) {
			m_UIUnitList.Add (Unit);
		}

		public void AddGUIUnits(IGUIUnit[] newUnitList) {
			for (int i = 0; i < newUnitList.Length; i++) {
				m_UIUnitList.Add(newUnitList[i]);
			}
		}

		public void RemoveNull() {
			for (int i = m_UIUnitList.Count - 1; i >= 0; i--) {
				if (m_UIUnitList [i] == null) {
					m_UIUnitList.RemoveAt(i);
				}
			}
		}

		public T GetGUIUnit<T>(string sUIName) where T : IGUIUnit {
			IGUIUnit unit_ = m_UIUnitList.Find(p => p.Name == sUIName);
			if (unit_ == null) {
				Debug.LogError("[GUISystem] Find no UI named " + sUIName);
				return null;
			} else {
				return (T)unit_;
			}
		}
			
		public void Instantiate (GameObject prefab) {
			// TODO
		}

		public void Destroy (string sUIName) {
			// TODO
		}

		public void Active(string sUIName) {
			IGUIUnit unit_ = m_UIUnitList.Find(p => p.Name == sUIName);
			if (unit_ == null) {
				Debug.LogError("[GUISystem] Find no UI named " + sUIName);
			} else {
				unit_.Active();
			}
		}

		public void Inactive(string sUIName) {
			IGUIUnit unit_ = m_UIUnitList.Find(p => p.Name == sUIName);
			if (unit_ == null) {
				Debug.LogError("[GUISystem] Find no UI named " + sUIName);
			} else {
				unit_.Inactive();
			}
		}

		public void InactiveAll() {
			foreach (IGUIUnit unit_ in m_UIUnitList) {
				unit_.Inactive();
			}
		}

		public void Show() {
			// TODO
		}

		public void Hide() {
			// TODO
		}

		public void HideAll() {
			// TODO
		}
	}
}