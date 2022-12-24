namespace BroWar.Debugging.Console
{
    public interface IConsoleHistoryDisposer
    {
        void SaveHistory();
        void LoadHistory();
        void ClearHistory();
        void AddEntry(string entry);
        bool TryGetNextEntryFromHistory(out string entry);
        bool TryGetPrevEntryFromHistory(out string entry);
    }
}