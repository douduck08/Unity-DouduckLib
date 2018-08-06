using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.UI {
    public class EmptyUI : MaskableGraphic {
        protected EmptyUI () {
            useLegacyMeshGeneration = false;
        }
        protected override void OnPopulateMesh (VertexHelper toFill) {
            toFill.Clear();
        }
    }
}