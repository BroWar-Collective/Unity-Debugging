using UnityEngine;

namespace BroWar.Debugging.Console
{
    /// <summary>
    /// Base class for console commands.
    /// Every public functions defined in inherting classes will be accessible through console.
    /// </summary>
    public abstract class ConsoleCommand : ScriptableObject
    { }
}