using System.Collections.Generic;

namespace BroWar.Debugging.Console
{
    public class AutocompleteHandler
    {
        private IList<string> optionList = new List<string>();

        private string GetShortestValidMatchStarting(string word)
        {
            string bestMatch = string.Empty;
            word = word.ToLower();
            foreach (var option in optionList)
            {
                var optionFormatted = option.ToLower();
                if (optionFormatted.StartsWith(word) && !optionFormatted.Equals(word)
                    && (string.IsNullOrEmpty(bestMatch) || optionFormatted.Length < bestMatch.Length))
                {
                    bestMatch = option;
                }
            }

            return bestMatch;
        }

        private string GetShortestValidMatchContaining(string word)
        {
            string bestMatch = string.Empty;
            word = word.ToLower();
            foreach (var option in optionList)
            {
                var optionFormatted = option.ToLower();
                if (optionFormatted.Contains(word) && !optionFormatted.Equals(word)
                    && (string.IsNullOrEmpty(bestMatch) || optionFormatted.Length < bestMatch.Length))
                {
                    bestMatch = option;
                }
            }

            return bestMatch;
        }

        public void RefreshOptions(IAutocompleteOptionProvider optionProvider, string[] words, int wordIndex)
        {
            optionList.Clear();
            optionProvider.GetParameterAutocompleteOptions(words, wordIndex, optionList);
        }

        public string GetBestMatch(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }

            string bestMatch = GetShortestValidMatchStarting(word);
            if (!string.IsNullOrEmpty(bestMatch))
            {
                return bestMatch;
            }

            bestMatch = GetShortestValidMatchContaining(word);
            if (!string.IsNullOrEmpty(bestMatch))
            {
                return bestMatch;
            }

            return string.Empty;
        }
    }
}