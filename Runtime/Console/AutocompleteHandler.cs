using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    public class AutocompleteHandler
    {
        private IList<string> optionList = new List<string>();

        public void RefreshOptions(IAutocompleteOptionProvider optionProvider, string[] words, int wordIndex)
        {
            optionList.Clear();

            optionProvider.GetParemeterAutocompleteOptions(words, wordIndex, optionList);
        }

        public string GetBestMatch(string currentInput)
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                return optionList.Count > 0 ? optionList[0] : string.Empty;
            }

            foreach (var option in optionList)
            {
                if (option.ToLower().StartsWith(currentInput.ToLower()))
                {
                    return option;
                }
            }

            return string.Empty;
        }
    }
}