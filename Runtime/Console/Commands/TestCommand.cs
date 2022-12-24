using UnityEngine;

namespace BroWar.Debugging.Console.Commands
{
    [CreateAssetMenu(fileName = "Test Command", menuName = "BroWar/Debugging/Commands/Test")]
    public class TestCommand : ConsoleCommand
    {
        public float Add(float i, int j)
        {
            return (i + j);
        }

        public float Add(float i, int j, int k)
        {
            return (i + j + k);
        }

        public double Add(double i, bool flag)
        {
            return (flag ? i : -i);
        }

        public string Print(string text)
        {
            return text;
        }
    }
}