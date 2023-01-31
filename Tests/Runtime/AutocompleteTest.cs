using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroWar.Debugging.Tests
{
    using BroWar.Debugging.Console;
    using NUnit.Framework;

    public class AutocompleteTest
    {
        private class OptionProvider : IAutocompleteOptionProvider
        {
            public void GetParameterAutocompleteOptions(string[] words, int wordIndex, IList<string> options)
            {
                switch (wordIndex)
                {
                    case 0:
                        options.Add("commandName");
                        break;
                    case 1:
                        options.Add("argument1");
                        break;
                    case 2:
                        options.Add("argument2");
                        break;
                }
            }
        }

        private OptionProvider optionProvider = new OptionProvider();
        private AutocompleteHandler autocompleteHandler = new AutocompleteHandler();

        [TestCase("comm", "commandName")]
        [TestCase("commandName arg", "argument1")]
        [TestCase("commandName argument1, a", "argument2")]
        public void TestAvailableAutocompletion(string input, string expectedCompleted)
        {
            var autocompletedInput = autocompleteHandler.GetBestMatch(optionProvider, input, "\"");
            Assert.AreEqual(autocompletedInput, expectedCompleted);
        }

        [TestCase("")]
        [TestCase("missingCom")]
        public void TestUnavailableAutocompletion(string input)
        {
            var autocompletedInput = autocompleteHandler.GetBestMatch(optionProvider, input, "\"");
            Assert.AreEqual(autocompletedInput, string.Empty);
        }
    }
}