using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public class UserInterfaceManager {
		private List<IUserInterface> m_UIList;

		public UserInterfaceManager() {
			m_UIList = new List<IUserInterface>();
		}

		public UserInterfaceManager(List<IUserInterface> UIList) {
			m_UIList = UIList;
		}

		public void AddUI(IUserInterface UI) {
			m_UIList.Add (UI);
		}

		public void HideAll() {
			foreach (IUserInterface UI_ in m_UIList) {
				UI_.Hide();
			}
		}

		public void Hide(string sUIName) {
			IUserInterface UI_ = m_UIList.Find(p => p.Name == sUIName);
			if (UI_ == null) {
				Debug.LogError("[UserInterfaceManager] Find no UI named " + sUIName);
			} else {
				UI_.Hide();
			}
		}

		public void Show(string sUIName) {
			IUserInterface UI_ = m_UIList.Find(p => p.Name == sUIName);
			if (UI_ == null) {
				Debug.LogError("[UserInterfaceManager] Find no UI named " + sUIName);
			} else {
				UI_.Show();
			}
		}

		public T GetUI<T>(string sUIName) where T : IUserInterface {
			IUserInterface UI_ = m_UIList.Find(p => p.Name == sUIName);
			if (UI_ == null) {
				Debug.LogError("[UserInterfaceManager] Find no UI named " + sUIName);
				return null;
			} else {
				return (T)UI_;
			}
		}
	}
}
