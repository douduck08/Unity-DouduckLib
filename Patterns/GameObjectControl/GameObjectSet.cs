using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public class GameObjectSet : MonoBehaviour {

        public enum HideMode {
            // Destroy,
            Inactive,
            OutOfViewFrustum
        }

        [SerializeField]
        private string m_SetName = "";
        public string Name {
            get {
                if (string.IsNullOrEmpty (m_SetName)) {
                    m_SetName = this.name;
                }
                return m_SetName;
            }
        }

        [SerializeField][ReadOnly]
        protected bool m_isVisible = true;
        public bool IsVisible {
            get {
                return m_isVisible;
            }
        }
        public HideMode m_HideMode = HideMode.Inactive;

        private Vector3 m_originalPosition;

        public void Show () {
            switch (m_HideMode) {
                case HideMode.Inactive:
                    OnShow ();
                    m_isVisible = true;
                    this.gameObject.SetActive (true);
                    break;
                case HideMode.OutOfViewFrustum:
                    OnShow ();
                    m_isVisible = true;
                    this.transform.position = m_originalPosition = this.transform.position;
                    break;
            }
        }

        public void Hide () {
            switch (m_HideMode) {
                case HideMode.Inactive:
                    OnHide ();
                    m_isVisible = false;
                    this.gameObject.SetActive (false);
                    break;
                case HideMode.OutOfViewFrustum:
                    OnHide ();
                    m_isVisible = false;
                    m_originalPosition = this.transform.position;
                    this.transform.position = new Vector3 (4000, 0, 0); // TODO: use general position
                    break;
            }
        }

        protected virtual void OnShow () { }
        protected virtual void OnHide () { }
    }
}