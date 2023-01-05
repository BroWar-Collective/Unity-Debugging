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
            foreach (var option in optionList)
            {
                if (option.ToLower().Contains(currentInput.ToLower()))
                {
                    return option;
                }
            }

            return string.Empty;
        }
    }
}