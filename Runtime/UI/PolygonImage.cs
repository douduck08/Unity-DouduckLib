using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI
{
    [AddComponentMenu("UI/Others/PolygonImage", 16)]
    [RequireComponent(typeof(Image))]
    public class PolygonImage : BaseMeshEffect, ICanvasRaycastFilter
    {
        protected PolygonImage() { }

        // GC Friendly
        static Vector3[] _fourCorners = new Vector3[4];
        static UIVertex _vertice = new UIVertex();

        RectTransform _rectTransform = null;
        Image _image = null;
        Vector2[] _spriteVertices;
        ushort[] _spriteTriangles;

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
            Sprite sprite = _image.overrideSprite;
            if (sprite == null || sprite.triangles.Length == 6)
            {
                // only 2 triangles
                return;
            }

            // Kanglai: at first I copy codes from Image.GetDrawingDimensions
            // to calculate Image's dimensions. But now for easy to read, I just take usage of corners.
            if (vh.currentVertCount != 4)
            {
                return;
            }

            _rectTransform.GetLocalCorners(_fourCorners);

            // Kanglai: recalculate vertices from Sprite!
            _spriteVertices = sprite.vertices;
            int len = _spriteVertices.Length;
            var vertices = new List<UIVertex>(len);
            Vector2 Center = sprite.bounds.center;
            Vector2 invExtend = new Vector2(1 / sprite.bounds.size.x, 1 / sprite.bounds.size.y);
            for (int i = 0; i < len; i++)
            {
                // normalize
                float x = (sprite.vertices[i].x - Center.x) * invExtend.x + 0.5f;
                float y = (sprite.vertices[i].y - Center.y) * invExtend.y + 0.5f;
                // lerp to position
                _vertice.position = new Vector2(Mathf.Lerp(_fourCorners[0].x, _fourCorners[2].x, x), Mathf.Lerp(_fourCorners[0].y, _fourCorners[2].y, y));
                _vertice.color = _image.color;
                _vertice.uv0 = sprite.uv[i];
                vertices.Add(_vertice);

                _spriteVertices[i] = _vertice.position; // for IsRaycastLocationValid usage
            }

            _spriteTriangles = sprite.triangles;
            len = _spriteTriangles.Length;
            var triangles = new List<int>(len);
            for (int i = 0; i < len; i++)
            {
                triangles.Add(sprite.triangles[i]);
            }

            vh.Clear();
            vh.AddUIVertexStream(vertices, triangles);
        }

        public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
        {
            var rectTransform = (RectTransform)transform;
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screenPos, eventCamera, out localPos);

            // normalize local coordinates
            var normalized = new Vector2(
                (localPos.x + rectTransform.pivot.x * rectTransform.rect.width) / rectTransform.rect.width,
                (localPos.y + rectTransform.pivot.y * rectTransform.rect.height) / rectTransform.rect.width
            );

            for (int i = 0; i < _spriteTriangles.Length; i += 3)
            {
                if (PointInTriangle(localPos, _spriteVertices[_spriteTriangles[i]], _spriteVertices[_spriteTriangles[i + 1]], _spriteVertices[_spriteTriangles[i + 2]]))
                {
                    return true;
                }
            }
            return false;
        }

        bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }
    }
}
