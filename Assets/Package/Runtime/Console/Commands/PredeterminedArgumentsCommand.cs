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

        void IAutocompleteOptionProvider.GetParameterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList)
        {
            switch (wordIndex)
            {
                case 1:
                    optionList.Add("Wroclawski");
                    optionList.Add("Wroclaw");
                    optionList.Add("Warsaw");
                    optionList.Add("Krakow");
                    break;
                case 2:
                    optionList.Add("jest");
                    optionList.Add("byl");
                    optionList.Add("bedzie");
                    break;
                case 3:
                    optionList.Add("piekny");
                    optionList.Add("brzydki");
                    optionList.Add("zwykly");
                    break;
            }
        }
    }
}