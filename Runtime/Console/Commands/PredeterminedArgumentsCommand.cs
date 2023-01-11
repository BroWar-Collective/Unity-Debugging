using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Console.Commands
{
    [CreateAssetMenu(fileName = "Predetermined Arguments Command", menuName = "BroWar/Debugging/Commands/Predetermined Arguments")]
    public class PredeterminedArgumentsCommand : ConsoleCommand, IAutocompleteOptionProvider
    {
        public void Execute(string value)
        {

        }

        void IAutocompleteOptionProvider.GetParemeterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList)
        {
            switch (wordIndex)
            {
                case 1:
                    optionList.Add("Wroclaw");
                    optionList.Add("Warsaw");
                    optionList.Add("Krakow");
                    break;
            }
        }
    }
}