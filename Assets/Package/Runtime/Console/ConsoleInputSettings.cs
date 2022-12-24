using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BroWar.Debugging.Console
{
    [Serializable]
    public class ConsoleInputSettings
    {
        [SerializeField]
        private Key triggerKey = Key.Backquote;
        [SerializeField]
        private Key confirmationKey = Key.Enter;
        [SerializeField]
        private Key prevCommandKey = Key.UpArrow;
        [SerializeField]
        private Key nextCommandKey = Key.DownArrow;

        public Key TriggerKey { get => triggerKey; set => triggerKey = value; }
        public Key ConfirmationKey { get => confirmationKey; set => confirmationKey = value; }
        public Key PrevCommandKey { get => prevCommandKey; set => prevCommandKey = value; }
        public Key NextCommandKey { get => nextCommandKey; set => nextCommandKey = value; }
    }
}