using UnityEngine;

namespace DouduckLib
{
    public class FPSDisplay : MonoBehaviour
    {
        public Color textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        [Range(0f, 1f)]
        public float textSize = 1f;

        public bool disableVSync;
        public int targetFPS;

        float _deltaTime = 0.0f;
        GUIStyle _style;
        Rect _rect;

        void Awake()
        {
            if (disableVSync)
                QualitySettings.vSyncCount = 0;
            if (targetFPS > 0)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFPS;
            }
        }

        void Start()
        {
            float h = Mathf.Lerp(Screen.height / 50f, Screen.height / 10f, textSize);
            _rect = new Rect(0, 0, Screen.width, h);
            _style = new GUIStyle();
            _style.alignment = TextAnchor.UpperLeft;
            _style.fontSize = (int)h;
            _style.normal.textColor = textColor;
        }

        void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", _deltaTime * 1000.0f, 1.0f / _deltaTime);
            GUI.Label(_rect, text, _style);
        }
    }
}
