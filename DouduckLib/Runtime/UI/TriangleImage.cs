using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI {
    [AddComponentMenu ("UI/Effects/TriangleImage", 16)]
    [RequireComponent (typeof (Image))]
    public class TriangleImage : BaseMeshEffect {
        protected TriangleImage () { }

        // GC Friendly
        private static Vector3[] fourCorners = new Vector3[4];
        private static UIVertex vertice = new UIVertex ();
        private RectTransform rectTransform = null;
        private Image image = null;
        public override void ModifyMesh (VertexHelper vh) {
            if (!isActiveAndEnabled) return;

            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform> ();
            }
            if (image == null) {
                image = GetComponent<Image> ();
            }
            if (image.type != Image.Type.Simple) {
                return;
            }
            if (vh.currentVertCount != 4) {
                return;
            }

            rectTransform.GetLocalCorners (fourCorners);
            var vertices = new List<UIVertex> (3);
            for (int i = 0; i < 3; i++) {
                vertice.position = fourCorners[i];
                vertice.color = image.color;
                vertice.uv0 = GetUV (i);
                vertices.Add (vertice);
            }

            var triangles = new List<int> (3);
            for (int i = 0; i < 3; i++) {
                triangles.Add (i);
            }

            vh.Clear ();
            vh.AddUIVertexStream (vertices, triangles);
        }

        private Vector2 GetUV (int index) {
            switch (index) {
                case 0:
                    return new Vector2 (0, 0);
                case 1:
                    return new Vector2 (0, 1);
                case 2:
                    return new Vector2 (1, 1);
                case 3:
                    return new Vector2 (1, 0);
                default:
                    return new Vector2 ();
            }
        }
    }
}