using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    public interface IAutocompleteOptionProvider
    {
        void GetParemeterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList);
    }
}