using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console.Commands
{
    [CreateAssetMenu(fileName = "Clear Command", menuName = "BroWar/Debugging/Commands/Clear")]
    public class ClearCommand : ConsoleCommand
    {
        private const string wrongArgumentsMessage = "Couldn't parse arguments.\nAvailable options:\n<color=yellow>-h,--history</color> <color=white>- clearing history</color>";

        public void Clear()
        {
            var consoleGui = FindObjectOfType<ConsoleGui>();
            consoleGui.Clear();
        }

        public CommandResult Clear(string arg)
        {
            if (arg.Equals("-h") || arg.Equals("--history"))
            {
                Clear();
                var consoleManager = FindObjectOfType<ConsoleManager>();
                consoleManager.ClearHistory();
                return CommandResult.Empty;
            }
            else
            {
                return new CommandResult(wrongArgumentsMessage, MessageType.Error);
            }
        }
    }
}