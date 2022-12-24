namespace BroWar.Debugging.Console
{
    public class CommandResult
    {
        public CommandResult(string message, MessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }

        public static CommandResult Empty => new CommandResult(string.Empty, MessageType.Log);

        public string Message { get; private set; }
        public MessageType MessageType { get; private set; }
    }
}