using System.Collections.Generic;

namespace BroWar.Debugging.Console
{
    public interface IAutocompleteOptionProvider
    {
        void GetParameterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList);
    }
}