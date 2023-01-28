using System.Collections.Generic;
using System.Linq;

namespace BroWar.Debugging.Console
{
    public class AutocompleteHandler
    {
        private readonly IList<string> options = new List<string>();

        private bool TryGetShortestValidMatchStarting(string word, out string bestMatch)
        {
            bestMatch = string.Empty;
            var wordFormatted = word.ToLower();
            foreach (var option in options)
            {
                var optionFormatted = option.ToLower();
                if (optionFormatted.StartsWith(wordFormatted) && !optionFormatted.Equals(wordFormatted)
                    && (string.IsNullOrEmpty(bestMatch) || optionFormatted.Length < bestMatch.Length))
                {
                    bestMatch = option;
                }
            }

            return !string.IsNullOrEmpty(bestMatch);
        }

        private bool TryGetShortestValidMatchContaining(string word, out string bestMatch)
        {
            bestMatch = string.Empty;
            var wordFormatted = word.ToLower();
            foreach (var option in options)
            {
                var optionFormatted = option.ToLower();
                if (optionFormatted.Contains(wordFormatted) && !optionFormatted.Equals(wordFormatted)
                    && (string.IsNullOrEmpty(bestMatch) || optionFormatted.Length < bestMatch.Length))
                {
                    bestMatch = option;
                }
            }

            return !string.IsNullOrEmpty(bestMatch);
        }

        private void RefreshOptions(IAutocompleteOptionProvider optionProvider, string[] splitInput)
        {
            options.Clear();
            optionProvider.GetParameterAutocompleteOptions(splitInput, splitInput.Length - 1, options);
        }

        public string GetBestMatch(IAutocompleteOptionProvider optionProvider, string input, string encapsulationCharacter)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var splitInput = ConsoleUtility.SplitInput(input, encapsulationCharacter);
            RefreshOptions(optionProvider, splitInput);
            if (TryGetShortestValidMatchStarting(splitInput.Last(), out var bestMatch))
            {
                return bestMatch;
            }

            if (TryGetShortestValidMatchContaining(splitInput.Last(), out bestMatch))
            {
                return bestMatch;
            }

            return string.Empty;
        }
    }
}