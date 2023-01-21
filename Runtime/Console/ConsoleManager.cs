using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace BroWar.Debugging.Console
{
    /// <summary>
    /// Manager for registering/unregistering and invoking console commands and storing input history.
    /// Uses commands stored in <see cref="commandsRepository"/> as an initial commands set.
    /// </summary>
    [AddComponentMenu("BroWar/Debugging/Console/Console Manager")]
    public class ConsoleManager : MonoBehaviour, IConsoleManager, IAutocompleteOptionProvider
    {
        [Title("Settings")]
        [SerializeField, FormerlySerializedAs("consoleManagerSettings"), IgnoreParent]
        private ConsoleManagerSettings settings;

        private Dictionary<string, HashSet<MethodInfo>> namesToMethods;
        private Dictionary<string, ConsoleCommand> namesToInstances;

        private ConsoleHistory inputHistory;

        private bool isInitialized;

        private void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            InitializeCache();
            ValidateSettings();
            InitializeHistory();
            InitializeCommands();
        }

        private void ValidateSettings()
        {
            Assert.IsNotNull(settings.CommandsRepository, "[Console] Commands repository not set.");
            Assert.AreNotEqual(settings.MultiargumentEncapsulationCharacter, string.Empty, "[Console] Missing multiArgumentEncapsulationCharacter.");
        }

        private void InitializeCache()
        {
            namesToMethods = new Dictionary<string, HashSet<MethodInfo>>();
            namesToInstances = new Dictionary<string, ConsoleCommand>();
        }

        private void InitializeHistory()
        {
            var bufferSize = settings.HistoryBufferSize;
            inputHistory = new ConsoleHistory(bufferSize);
            inputHistory.LoadHistory();
        }

        private void InitializeCommands()
        {
            var commandsRepository = settings.CommandsRepository;
            var commandsList = commandsRepository.commands;
            for (int i = 0; i < commandsList.Count; i++)
            {
                var command = commandsList[i];
                AppendCommand(command);
            }
        }

        private string ExtractCommandName(string input)
        {
            var clearedInput = input.Trim();
            var splittedInput = clearedInput.Split(' ');
            return splittedInput.Length == 0
                ? input.ToLower()
                : splittedInput[0].ToLower();
        }

        private bool TryParseParameters(string[] arguments, ParameterInfo[] methodParameters, out object[] parsedArguments)
        {
            parsedArguments = new object[arguments.Length];
            try
            {
                for (var i = 0; i < arguments.Length; i++)
                {
                    var converter = TypeDescriptor.GetConverter(methodParameters[i].ParameterType);
                    parsedArguments[i] = converter.ConvertFromInvariantString(arguments[i]);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GetAvailableCommandsText(string commandName)
        {
            var sb = new StringBuilder();
            var commandMethods = namesToMethods[commandName];
            foreach (var method in commandMethods)
            {
                var methodDisplayName = ConsoleUtility.GetMethodDefinitionString(method);
                sb.AppendLine(methodDisplayName);
            }

            return sb.ToString();
        }

        private bool TryMatchMethod(string commandNameLowerCased, string[] arguments, HashSet<MethodInfo> commandMethods, out MethodInfo matchedMethod, out object[] parsedArguments)
        {
            foreach (var method in commandMethods)
            {
                var methodName = method.Name;
                var methodNameLowerCased = methodName.ToLower();
                if (!methodNameLowerCased.Equals(commandNameLowerCased))
                {
                    continue;
                }

                var methodParameters = method.GetParameters();
                if (arguments.Length != methodParameters.Length)
                {
                    continue;
                }

                var areArgumentsMatched = TryParseParameters(arguments, methodParameters, out parsedArguments);
                if (!areArgumentsMatched)
                {
                    continue;
                }

                matchedMethod = method;
                return true;
            }

            parsedArguments = null;
            matchedMethod = null;
            return false;
        }

        private void RegistorMethodFromCommand(ConsoleCommand command, MethodInfo method)
        {
            var commandName = method.Name.ToLower();
            if (namesToMethods.TryGetValue(commandName, out var methodsGroup))
            {
                if (methodsGroup.Contains(method))
                {
                    Debug.LogWarning($"[Console] There is a command {command.name} registered with the same method definition {method}");
                }
                else
                {
                    methodsGroup.Add(method);
                }
            }
            else
            {
                namesToMethods.Add(commandName, new HashSet<MethodInfo>()
                {
                    method
                });
                namesToInstances.Add(commandName, command);
            }
        }

        public void AppendCommand(ConsoleCommand command)
        {
            EnsureInitialized();

            var commandType = command.GetType();
            var commandInfo = commandType.GetTypeInfo();
            foreach (var method in commandInfo.DeclaredMethods.Where(method => method.IsPublic))
            {
                RegistorMethodFromCommand(command, method);
            }
        }

        public void RemoveCommand(ConsoleCommand command)
        {
            EnsureInitialized();

            var commandType = command.GetType();
            foreach (var method in commandType.GetMethods())
            {
                if (namesToMethods.TryGetValue(method.Name, out var methodsSet))
                {
                    methodsSet.Remove(method);
                }
            }
        }

        public object InvokeCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                Debug.LogWarning($"[Console] Cannot process invalid input - '{input}'");
                return null;
            }

            EnsureInitialized();

            inputHistory.AddEntry(input);
            try
            {
                var commandName = ExtractCommandName(input);
                if (namesToMethods.TryGetValue(commandName, out var commandMethods))
                {
                    var arguments = ConsoleUtility.ExtractArguments(input, settings.MultiargumentEncapsulationCharacter);
                    if (TryMatchMethod(commandName, arguments, commandMethods, out var method, out var parsedArguments))
                    {
                        var command = namesToInstances[commandName];
                        return method.Invoke(command, parsedArguments);
                    }

                    return new CommandResult($"Couldn't find a matching method. Available {commandName} methods:\n<color=yellow>{GetAvailableCommandsText(commandName)}</color>", MessageType.Error);
                }

                return new CommandResult($"Command {commandName} isn't registered in the console.", MessageType.Error);
            }
            catch (Exception e)
            {
                return new CommandResult(e.ToString(), MessageType.Error);
            }
        }

        public void ClearHistory()
        {
            InputHistory.ClearHistory();
        }

        public bool TryGetNextEntryFromHistory(out string entry)
        {
            return InputHistory.TryGetNextEntryFromHistory(out entry);
        }

        public bool TryGetPrevEntryFromHistory(out string entry)
        {
            return InputHistory.TryGetPrevEntryFromHistory(out entry);
        }

        public void AddEntry(string entry)
        {
            InputHistory.AddEntry(entry);
        }

        public void SaveHistory()
        {
            InputHistory.SaveHistory();
        }

        public void LoadHistory()
        {
            InputHistory.LoadHistory();
        }

        void IAutocompleteOptionProvider.GetParemeterAutocompleteOptions(string[] words, int wordIndex, IList<string> optionList)
        {
            EnsureInitialized();

            if (wordIndex == 0)
            {
                foreach (var commandName in RegisteredMethods.Keys.ToArray())
                {
                    if (optionList.Contains(commandName))
                    {
                        continue;
                    }

                    optionList.Add(commandName);
                }
            }
            else
            {
                if (!namesToInstances.ContainsKey(words[0]))
                {
                    return;
                }

                var command = namesToInstances[words[0]];
                if (command is IAutocompleteOptionProvider commandAutocompleteProvider)
                {
                    commandAutocompleteProvider.GetParemeterAutocompleteOptions(words, wordIndex, optionList);
                }
            }
        }

        private ConsoleHistory InputHistory
        {
            get
            {
                EnsureInitialized();
                return inputHistory;
            }
        }

        public IReadOnlyDictionary<string, HashSet<MethodInfo>> RegisteredMethods => namesToMethods;
    }
}