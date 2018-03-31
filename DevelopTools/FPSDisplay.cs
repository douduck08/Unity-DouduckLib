using UnityEngine;

namespace DouduckLib.DevelopTool {
    public class FPSDisplay : MonoBehaviour {
        public Color textColor = new Color (1.0f, 0.0f, 0.0f, 1.0f);
        [Range (0f, 1f)]
        public float textSize = 1f;

        private float m_deltaTime = 0.0f;
        private GUIStyle m_style;
        private Rect m_rect;

        void Awake () {
            float h = Mathf.Lerp (Screen.height / 50f, Screen.height / 10f, textSize);
            m_rect = new Rect (0, 0, Screen.width, h);
            m_style = new GUIStyle ();
            m_style.alignment = TextAnchor.UpperLeft;
            m_style.fontSize = (int)h;
            m_style.normal.textColor = textColor;
        }

        void Update () {
            m_deltaTime += (Time.deltaTime - m_deltaTime) * 0.1f;
        }

        void OnGUI () {
            string text = string.Format ("{0:0.0} ms ({1:0.} fps)", m_deltaTime * 1000.0f, 1.0f / m_deltaTime);
            GUI.Label (m_rect, text, m_style);
        }
    }
}