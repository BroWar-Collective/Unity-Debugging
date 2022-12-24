#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace BroWar.Debugging
{
    [AddComponentMenu("BroWar/Debugging/Game Killer")]
    public class GameKiller : MonoBehaviour
    {
        [SerializeField, SearchableEnum]
        private Key exitKey = Key.Escape;

        private void Update()
        {
            if (Keyboard.current[exitKey].wasPressedThisFrame)
            {
                KillGame();
            }
        }

        private void KillGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}