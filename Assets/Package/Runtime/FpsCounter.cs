using UnityEngine;

namespace BroWar.Debugging
{
    [AddComponentMenu("BroWar/Debugging/FPS Counter")]
    public class FpsCounter : MonoBehaviour
    {
        private readonly Rect labelPosition = new Rect(10, 10, 100, 100);

        [SerializeField, Disable]
        private int value;

        private void OnGUI()
        {
            value = (int)(1.0f / Time.unscaledDeltaTime);
            GUI.Label(labelPosition, $"{value} FPS");
        }
    }
}