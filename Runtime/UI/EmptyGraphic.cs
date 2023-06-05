using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI
{
    [AddComponentMenu("UI/Others/EmptyGraphic", 16)]
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyGraphic : MaskableGraphic
    {
        protected EmptyGraphic()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}
