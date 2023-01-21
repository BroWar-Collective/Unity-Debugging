using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    public class AutocompleteHandler
    {
        private IList<string> optionList = new List<string>();

        private string GetShortestValidMatchStarting(string word)
        {
            string bestMatch = string.Empty;

            foreach (var option in optionList)
            {
                if (option.ToLower().StartsWith(word.ToLower())
                   && !option.Equals(word))
                {
                    if (string.IsNullOrEmpty(bestMatch) || option.Length < bestMatch.Length)
                    {
                        bestMatch = option;
                    }
                }
            }

            return bestMatch;
        }

        private string GetShortestValidMatchContaining(string word)
        {
            string bestMatch = string.Empty;

            foreach (var option in optionList)
            {
                if (option.ToLower().Contains(word.ToLower())
                   && !option.Equals(word))
                {
                    bestMatch = option;
                }
            }

            return bestMatch;
        }

        public void RefreshOptions(IAutocompleteOptionProvider optionProvider, string[] words, int wordIndex)
        {
            optionList.Clear();

            optionProvider.GetParemeterAutocompleteOptions(words, wordIndex, optionList);
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