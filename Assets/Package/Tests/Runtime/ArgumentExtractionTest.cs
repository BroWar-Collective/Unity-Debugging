using NUnit.Framework;

namespace BroWar.Tests.Debugging
{
    using BroWar.Debugging.Console;

    public class ArgumentExtractionTest
    {
        [TestCase("CommandName", new string[] { })]
        [TestCase("CommandName \"arg1\"", new string[] { "arg1" })]
        [TestCase("CommandName \"arg1.1 arg1.2\"", new string[] { "arg1.1 arg1.2" })]
        [TestCase("CommandName arg1 \"arg2.1 arg2.2\"", new string[] { "arg1", "arg2.1 arg2.2" })]
        [TestCase("CommandName \"arg1.1 arg1.2\" arg2", new string[] { "arg1.1 arg1.2", "arg2" })]
        [TestCase("CommandName arg1 \"arg2.1 arg2.2\" arg3", new string[] { "arg1", "arg2.1 arg2.2", "arg3" })]
        [TestCase("CommandName arg1 \"arg2.1 arg2.2\" arg3 arg4", new string[] { "arg1", "arg2.1 arg2.2", "arg3", "arg4" })]
        [TestCase("CommandName arg1 \"arg2.1 arg2.2\" \" arg3.1 arg3.2\"", new string[] { "arg1", "arg2.1 arg2.2", " arg3.1 arg3.2" })]
        public void TestMultiArgumentExtraction(string input, string[] expectedOutput)
        {
            Assert.AreEqual(ConsoleUtility.ExtractArguments(input, "\""), expectedOutput);
        }

        [TestCase("CommandName \"invalid input")]
        [TestCase("CommandName invalid input\"")]
        [TestCase("CommandName \"invalid input\"\"")]
        [TestCase("CommandName invalid input\"\"\"")]
        [TestCase("CommandName \"arg1.1 arg 1.2\" \"arg2.1 arg2.2")]
        [TestCase("CommandName \"arg1.1 arg 1.2\" arg2.1 arg2.2\"")]
        public void TestMultiArgumentExtractionExceptions(string input)
        {
            Assert.Throws<ArgumentExtractionException>(() => ConsoleUtility.ExtractArguments(input, "\""));
        }
    }
}