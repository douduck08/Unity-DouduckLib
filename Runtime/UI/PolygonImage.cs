using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI
{
    [AddComponentMenu("UI/Effects/PolygonImage", 16)]
    [RequireComponent(typeof(Image))]
    public class PolygonImage : BaseMeshEffect, ICanvasRaycastFilter
    {
        protected PolygonImage() { }

        // GC Friendly
        private static Vector3[] fourCorners = new Vector3[4];
        private static UIVertex vertice = new UIVertex();

        private RectTransform rectTransform = null;
        private Image image = null;
        private Vector2[] spriteVertices;
        private ushort[] spriteTriangles;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled)
                return;

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            if (image.type != Image.Type.Simple)
            {
                return;
            }
            Sprite sprite = image.overrideSprite;
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

            rectTransform.GetLocalCorners(fourCorners);

            // Kanglai: recalculate vertices from Sprite!
            spriteVertices = sprite.vertices;
            int len = spriteVertices.Length;
            var vertices = new List<UIVertex>(len);
            Vector2 Center = sprite.bounds.center;
            Vector2 invExtend = new Vector2(1 / sprite.bounds.size.x, 1 / sprite.bounds.size.y);
            for (int i = 0; i < len; i++)
            {
                // normalize
                float x = (sprite.vertices[i].x - Center.x) * invExtend.x + 0.5f;
                float y = (sprite.vertices[i].y - Center.y) * invExtend.y + 0.5f;
                // lerp to position
                vertice.position = new Vector2(Mathf.Lerp(fourCorners[0].x, fourCorners[2].x, x), Mathf.Lerp(fourCorners[0].y, fourCorners[2].y, y));
                vertice.color = image.color;
                vertice.uv0 = sprite.uv[i];
                vertices.Add(vertice);

                spriteVertices[i] = vertice.position; // for IsRaycastLocationValid usage
            }

            spriteTriangles = sprite.triangles;
            len = spriteTriangles.Length;
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

            for (int i = 0; i < spriteTriangles.Length; i += 3)
            {
                if (PointInTriangle(localPos, spriteVertices[spriteTriangles[i]], spriteVertices[spriteTriangles[i + 1]], spriteVertices[spriteTriangles[i + 2]]))
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
