namespace BroWar.Debugging.Console
{
    public interface IConsoleManager : IConsoleHistoryDisposer
    {
        object InvokeCommand(string input);
        void AppendCommand(ConsoleCommand command);
        void RemoveCommand(ConsoleCommand command);
    }
}