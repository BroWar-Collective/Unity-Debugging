using System.Text;
using UnityEngine;

namespace BroWar.Debugging.Console.Commands
{
    [CreateAssetMenu(fileName = "Help Command", menuName = "BroWar/Debugging/Commands/Help")]
    public class HelpCommand : ConsoleCommand
    {
        public CommandResult Help()
        {
            var consoleManager = FindObjectOfType<ConsoleManager>();
            var registeredMethods = consoleManager.RegisteredMethods;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<color=\"red\">Available commands:</color>");
            foreach (var methodItem in registeredMethods)
            {
                var methods = methodItem.Value;
                foreach (var method in methods)
                {
                    var methodDisplay = ConsoleUtility.GetMethodDefinitionString(method);
                    stringBuilder.AppendLine(methodDisplay);
                }
            }

            var message = stringBuilder.ToString();
            return new CommandResult(message, MessageType.Warning);
        }
    }
}