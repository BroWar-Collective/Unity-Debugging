using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BroWar.Debugging
{
    [AddComponentMenu("BroWar/Debugging/Scenes Loader")]
    public class ScenesLoader : MonoBehaviour
    {
        [SerializeField, ReorderableList]
        private SerializedScene[] scenes;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private IEnumerator Start()
        {
            foreach (var scene in scenes)
            {
                var operation = SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);
                if (operation == null)
                {
                    continue;
                }

                while (!operation.isDone)
                {
                    yield return null;
                }

                Debug.Log($"[Debugging] Scene loaded: {scene.SceneName}");
            }
        }

        private void HandleFirst(SerializedScene scene)
        {
            var targetScene = SceneManager.GetSceneByBuildIndex(scene.BuildIndex);
            SceneManager.SetActiveScene(targetScene);
        }

        private void HandleFirst(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scenes.Length == 0)
            {
                return;
            }

            var targetScene = scenes[0];
            if (targetScene.BuildIndex == scene.buildIndex)
            {
                HandleFirst(scene);
            }
        }
    }
}