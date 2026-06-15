using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI
{
    [AddComponentMenu("UI/Others/TriangleImage", 16)]
    [RequireComponent(typeof(Image))]
    public class TriangleImage : BaseMeshEffect
    {
        protected TriangleImage() { }

        // GC Friendly
        static Vector3[] _fourCorners = new Vector3[4];
        static UIVertex _vertice = new UIVertex();
        RectTransform _rectTransform = null;
        Image _image = null;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled)
                return;

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            if (_image.type != Image.Type.Simple)
            {
                return;
            }
            if (vh.currentVertCount != 4)
            {
                return;
            }

            _rectTransform.GetLocalCorners(_fourCorners);
            var vertices = new List<UIVertex>(3);
            for (int i = 0; i < 3; i++)
            {
                _vertice.position = _fourCorners[i];
                _vertice.color = _image.color;
                _vertice.uv0 = GetUV(i);
                vertices.Add(_vertice);
            }

            var triangles = new List<int>(3);
            for (int i = 0; i < 3; i++)
            {
                triangles.Add(i);
            }

            vh.Clear();
            vh.AddUIVertexStream(vertices, triangles);
        }

        Vector2 GetUV(int index)
        {
            switch (index)
            {
                case 0:
                     return new Vector2(0, 0);
                case 1:
                     return new Vector2(0, 1);
                case 2:
                     return new Vector2(1, 1);
                case 3:
                     return new Vector2(1, 0);
                default:
                     return new Vector2();
            }
        }
    }
}
