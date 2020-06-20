using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DouduckLib {
    public class SceneRequester : MonoBehaviour {

        [SerializeField, SceneAsset] string activeScene;
        [SerializeField, SceneAsset] List<string> sceneAssets;
        [SerializeField] bool loadOnStart;

        void Start () {
            if (loadOnStart) {
                LoadAllSceneInList ();
            }
        }

        public void LoadAllSceneInList () {
            var scenesToLoad = new List<string> (sceneAssets);
            foreach (var scene in UnityUtil.GetAllLoadedScenes ()) {
                scenesToLoad.Remove (scene.path);
            }
            for (int i = 0; i < scenesToLoad.Count; i++) {
                new SceneLoader (scenesToLoad[i], true, scenesToLoad[i] == activeScene).StartProcessing (this);
            }
        }

        public void UnloadSceneNotInList () {
            var scenesToUnload = UnityUtil.GetAllLoadedScenes ().Select (p => p.path).ToList ();
            foreach (var scenePath in sceneAssets) {
                scenesToUnload.Remove (scenePath);
            }
            for (int i = 0; i < scenesToUnload.Count; i++) {
                new SceneLoader ("", scenesToUnload[i]).StartProcessing (this);
            }
        }
    }
}