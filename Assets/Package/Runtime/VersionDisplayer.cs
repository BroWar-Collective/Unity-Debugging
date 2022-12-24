using UnityEngine;

namespace BroWar.Debugging
{
    [AddComponentMenu("BroWar/Debugging/Version Displayer")]
    public class VersionDisplayer : MonoBehaviour
    {
        private void OnGUI()
        {
            var labelPosition = new Rect(10, Screen.height - 20, 100, 40);
            GUI.Label(labelPosition, $"v{Application.version}");
        }
    }
}