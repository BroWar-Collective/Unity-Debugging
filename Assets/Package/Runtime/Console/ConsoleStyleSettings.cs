using System;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    [Serializable]
    public class ConsoleStyleSettings
    {
        [SerializeField]
        private Rect initialRect;
        [SerializeField]
        private Color textColor = Color.white;
        [SerializeField]
        private Color mainColor = Color.black;

        public Rect InitialRect { get => initialRect; set => initialRect = value; }
        public Color TextColor { get => textColor; set => textColor = value; }
        public Color MainColor { get => mainColor; set => mainColor = value; }
    }
}