using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Ref: https://forum.unity.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1753530
namespace DouduckLib.UI {
    [AddComponentMenu ("UI/TextWithEvents", 12)]
    [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class InteractableText : Text, IPointerClickHandler {

        [System.Serializable]
        public class StringEvent : UnityEvent<string> { }
        public StringEvent onUrlClick;

        [TextArea (3, 10)]
        public string nonParsedStr;
        public new string text {
            get { return base.text; }
            set {
                if (string.IsNullOrEmpty (value)) {
                    if (string.IsNullOrEmpty (this.text)) {
                        return;
                    }
                    base.text = string.Empty;
                    nonParsedStr = string.Empty;
                    m_links.Clear ();
                    m_interactArea.Clear ();
                    this.SetVerticesDirty ();
                } else {
                    if (nonParsedStr != value) {
                        base.text = OnBeforeValueChange (value);
                        nonParsedStr = value;
                        this.SetAllDirty ();
                    }
                }
            }
        }

        #if UNITY_EDITOR
        protected override void OnValidate () {
            base.OnValidate ();
            base.text = OnBeforeValueChange (nonParsedStr);
        }
        #endif

        // compiled regex eat relatively a lot time on start but gain performance later. it require .NET 2.0 instead of .NET 2.0 subset
        static Regex m_regex = new Regex (@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline /*| RegexOptions.Compiled*/);
        StringBuilder m_sb = new StringBuilder ();
        struct StrInfo {
            public int start;
            public int end;
            public StrInfo (int start, int end) {
                this.start = start;
                this.end = end;
            }
        }
        Dictionary<string, List<StrInfo>> m_links = new Dictionary<string, List<StrInfo>> ();
        Dictionary<Rect, string> m_interactArea = new Dictionary<Rect, string> ();

        private string OnBeforeValueChange (string strToParse) {
            var splittedStr = m_regex.Split (strToParse);
            m_sb.Length = 0;
            m_sb.EnsureCapacity (strToParse.Length);
            m_links.Clear ();

            var i = 0;
            foreach (var str in splittedStr) {
                if (i + 2 < splittedStr.Length && splittedStr[i + 2] == "</a>") {
                    if (m_links.ContainsKey (str))
                        m_links[str].Add (new StrInfo (m_sb.Length, m_sb.Length + splittedStr[i + 1].Length - 1));
                    else {
                        m_links.Add (str, new List<StrInfo> () { new StrInfo (m_sb.Length, m_sb.Length + splittedStr[i + 1].Length - 1) });
                    }
                } else if (str != "</a>" && str != string.Empty) {
                    m_sb.Append (str);
                }
                i++;
            }
            return m_sb.ToString ();
        }

        protected override void OnPopulateMesh (VertexHelper vertexHelper) {
            base.OnPopulateMesh (vertexHelper);

            if (m_links.Count == 0) return;

            //check if UI is normal or layout and alias them
            var cTextGen = cachedTextGenerator;
            if (cTextGen == null) cTextGen = cachedTextGeneratorForLayout;

            m_interactArea.Clear();
            foreach (var link in m_links) {
                foreach (var strInfo in link.Value) {
                    // Check for truncated text (doesn't generate verts for all characters)
                    int startVertIdx = strInfo.start * 4;
                    int endVertIdx = strInfo.end * 4 + 3 >= cTextGen.vertexCount ? cTextGen.vertexCount - 4 : strInfo.end * 4;
                    if (cTextGen.lineCount > 1 && cTextGen.verts[startVertIdx + 2].position.y > cTextGen.verts[endVertIdx].position.y) {  // multi-line
                        for (var lineId = 0; lineId < cTextGen.lineCount - 1; lineId++) {
                            if (strInfo.start > cTextGen.lines[lineId].startCharIdx && strInfo.start < cTextGen.lines[lineId + 1].startCharIdx) {
                                Vector3 pos = cTextGen.verts[startVertIdx + 3].position;
                                int lineEndIdx = cTextGen.lines[lineId + 1].startCharIdx - 1;
                                m_interactArea.Add (new Rect (pos.x, pos.y, cTextGen.verts[lineEndIdx * 4 + 1].position.x - pos.x, cTextGen.lines[lineId].height), link.Key);
                            } else if (strInfo.end > cTextGen.lines[lineId].startCharIdx && strInfo.end < cTextGen.lines[lineId + 1].startCharIdx) {
                                Vector3 pos = cTextGen.verts[cTextGen.lines[lineId].startCharIdx * 4 + 3].position;
                                m_interactArea.Add (new Rect (pos.x, pos.y, cTextGen.verts[endVertIdx + 1].position.x - pos.x, cTextGen.lines[lineId].height), link.Key);
                                break;
                            } else if (strInfo.start < cTextGen.lines[lineId + 1].startCharIdx) {
                                Vector3 pos = cTextGen.verts[cTextGen.lines[lineId].startCharIdx * 4 + 3].position;
                                int lineEndIdx = cTextGen.lines[lineId + 1].startCharIdx - 1;
                                m_interactArea.Add (new Rect (pos.x, pos.y, cTextGen.verts[lineEndIdx * 4 + 1].position.x - pos.x, cTextGen.lines[lineId].height), link.Key);
                            }
                            if (lineId == cTextGen.lineCount - 2) {
                                //check if ugui cut last vertices due to fact that text is too long for container
                                if (strInfo.end * 4 + 3 > cTextGen.vertexCount - 1) {
                                    m_interactArea.Add (new Rect (cTextGen.verts[cTextGen.lines[lineId + 1].startCharIdx * 4].position.x, cTextGen.verts[cTextGen.vertexCount - 5].position.y, cTextGen.verts[(cTextGen.vertexCount - 3)].position.x - cTextGen.verts[cTextGen.lines[lineId + 1].startCharIdx * 4].position.x, cTextGen.lines[lineId + 1].height), link.Key);
                                } else {
                                    m_interactArea.Add (new Rect (cTextGen.verts[cTextGen.lines[lineId + 1].startCharIdx * 4].position.x, cTextGen.verts[strInfo.end * 4 + 3].position.y, cTextGen.verts[strInfo.end * 4 + 1].position.x - cTextGen.verts[cTextGen.lines[lineId + 1].startCharIdx * 4].position.x, cTextGen.lines[lineId + 1].height), link.Key);
                                }
                            }
                        }
                    } else {
                        //check if ugui cut last vertices due to fact that text is too long for container
                        Vector3 pos = cTextGen.verts[startVertIdx + 3].position;
                        m_interactArea.Add (new Rect (pos.x, pos.y, cTextGen.verts[endVertIdx + 1].position.x - pos.x, cTextGen.lines[0].height), link.Key);
                    }
                }
            }
        }

        public void OnPointerClick (PointerEventData eventData) {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle (this.rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localPos);
            foreach (var areaInfo in m_interactArea) {
                if (areaInfo.Key.Contains(localPos)) {
                    onUrlClick.Invoke (areaInfo.Value);
                    return;
                }
            }
        }
    }
}