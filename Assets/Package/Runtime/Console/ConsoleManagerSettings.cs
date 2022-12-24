using System;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    [Serializable]
    public class ConsoleManagerSettings
    {
        [field: SerializeField, InLineEditor]
        public CommandsRepository CommandsRepository { get; set; }
        [field: SerializeField]
        public int HistoryBufferSize { get; set; } = 64;
        [field: SerializeField]
        public string MultiargumentEncapsulationCharacter { get; set; } = "\"";
    }
}