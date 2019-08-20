using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class SceneRequester : MonoBehaviour {
        [SerializeField, SceneAsset] List<string> sceneAssets;

        void Start () {
            for (int i = 0; i < sceneAssets.Count; i++) {
                new SceneLoader (sceneAssets[i], true).StartLoading (this);
            }
        }
    }
}