using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    public class ConsoleHistory : IConsoleHistoryDisposer
    {
        private const string playerPrefsConsoleHistoryCount = "consoleHistoryList_count";
        private const string playerPrefsConsoleHistoryElementFormat = "consoleHistoryList_{0}";

        private readonly int historyBufferSize;

        private readonly string[] inputHistoryBuffer;
        private readonly Queue<string> inputHistoryQueue;

        public ConsoleHistory(int historyBufferSize)
        {
            this.historyBufferSize = historyBufferSize;
            inputHistoryQueue = new Queue<string>(historyBufferSize);
            inputHistoryBuffer = new string[historyBufferSize];
        }

        public void ClearHistory()
        {
            inputHistoryQueue.Clear();
            for (var i = 0; i < inputHistoryBuffer.Length; i++)
            {
                inputHistoryBuffer[i] = null;
            }

            CurrentEntryIndex = 0;
        }

        public void SaveHistory()
        {
            var historyCount = inputHistoryQueue.Count;
            PlayerPrefs.SetInt(playerPrefsConsoleHistoryCount, historyCount);
            for (var i = 0; i < historyCount; i++)
            {
                var inputLine = inputHistoryBuffer[i];
                PlayerPrefs.SetString(string.Format(playerPrefsConsoleHistoryElementFormat, i), inputLine);
            }
        }

        public void LoadHistory()
        {
            inputHistoryQueue.Clear();
            var historyCount = PlayerPrefs.GetInt(playerPrefsConsoleHistoryCount, 0);
            historyCount = Mathf.Min(historyCount, historyBufferSize);
            for (var i = 0; i < historyCount; i++)
            {
                var inputLine = PlayerPrefs.GetString(string.Format(playerPrefsConsoleHistoryElementFormat, i), string.Empty);
                inputHistoryBuffer[i] = inputLine;
                inputHistoryQueue.Enqueue(inputLine);
            }

            CurrentEntryIndex = historyCount;
        }

        public void AddEntry(string entry)
        {
            inputHistoryQueue.Enqueue(entry);
            if (inputHistoryQueue.Count > historyBufferSize)
            {
                inputHistoryQueue.Dequeue();
            }

            inputHistoryQueue.CopyTo(inputHistoryBuffer, 0);
            CurrentEntryIndex = inputHistoryQueue.Count;
        }

        public bool TryGetNextEntryFromHistory(out string entry)
        {
            if (inputHistoryQueue.Count == 0 || CurrentEntryIndex >= inputHistoryQueue.Count - 1)
            {
                CurrentEntryIndex = inputHistoryQueue.Count;
                entry = string.Empty;
                return false;
            }

            CurrentEntryIndex++;
            entry = inputHistoryBuffer[CurrentEntryIndex];
            return true;
        }

        public bool TryGetPrevEntryFromHistory(out string entry)
        {
            if (CurrentEntryIndex == 0)
            {
                entry = inputHistoryBuffer[CurrentEntryIndex];
                return false;
            }

            CurrentEntryIndex--;
            entry = inputHistoryBuffer[CurrentEntryIndex];
            return true;
        }

        public int CurrentEntryIndex { get; private set; }
        public int CurrentHistorySize => inputHistoryQueue.Count;
    }
}