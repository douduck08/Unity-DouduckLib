using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;

namespace DouduckGame {
	public delegate void AccepterDelegate(TcpClient oClient);
	public abstract class IAccepter {

		public string Name;
		protected IPAddress m_LocalIPAddress;
		protected TcpListener m_oListener = null;
		protected TcpListener Listener {
			get {
				if (m_oListener == null) {
					Debug.LogError("[Accepter] Listener is null");
				}
				return m_oListener;
			}
		}
		protected bool m_bActive;
		public  bool IsActive {
			get {
				return m_bActive;
			}
		}

		public IAccepter(string sName, string sLocalIP, int iListenPort) {
			m_LocalIPAddress = IPAddress.Parse(sLocalIP);
			m_oListener = new TcpListener(m_LocalIPAddress, iListenPort);

			Listener.ExclusiveAddressUse = false;
			m_bActive = false;
			Debug.Log("[Accepter] " + Name + ": Ready to work...(" + sLocalIP + ":" + iListenPort + ")");
		}

		public virtual void StartListen() {
			if (!IsActive) {
				Listener.Start();
				m_bActive = true;
				Debug.Log ("[Accepter] " + Name + ": Start to listen");
				WaitForClientConnect();
			}
		}
		public virtual void StopListen() {
			if (IsActive) {
				Listener.Stop();
				m_bActive = false;
				Debug.Log ("[Accepter] " + Name + ": Stop listening");
			}
		}
		protected abstract void WaitForClientConnect();
	}
}