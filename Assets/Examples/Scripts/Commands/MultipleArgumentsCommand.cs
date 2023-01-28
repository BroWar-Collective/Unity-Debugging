using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Examples.Commands
{
    using BroWar.Debugging.Console;

    [CreateAssetMenu(fileName = "Multiple Arguments Command", menuName = "Examples/Commands/Multiple Arguments")]
    public class MultipleArgumentsCommand : ConsoleCommand, IAutocompleteOptionProvider
    {
        public void ModifyArray(string arg1, string arg2)
        { }

        void IAutocompleteOptionProvider.GetParameterAutocompleteOptions(string[] words, int wordIndex, IList<string> options)
        {
            switch (wordIndex)
            {
                case 1:
                    options.Add("insert");
                    options.Add("remove");
                    break;
                case 2:
                    options.Add("item");
                    options.Add("range");
                    break;
            }
        }
    }
}