using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console.Commands
{
    [CreateAssetMenu(fileName = "Multiple Arguments Command", menuName = "BroWar/Debugging/Commands/Multiple Arguments")]
    public class MultipleArgumentsCommand : ConsoleCommand, IAutocompleteOptionProvider
    {
        public void ModifyArray(string arg1, string arg2)
        { }

        void IAutocompleteOptionProvider.GetParameterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList)
        {
            switch (wordIndex)
            {
                case 1:
                    optionList.Add("insert");
                    optionList.Add("remove");
                    break;
                case 2:
                    optionList.Add("item");
                    optionList.Add("range");
                    break;
            }
        }
    }
}