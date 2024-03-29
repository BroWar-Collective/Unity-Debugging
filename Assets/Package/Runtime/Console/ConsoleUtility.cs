﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BroWar.Debugging.Console
{
    internal static class ConsoleUtility
    {
        private static string TypeToSimpleTypeString(Type type)
        {
            switch (type.Name)
            {
                case "Int32":
                    return "int";
                case "Single":
                    return "float";
                case "Double":
                    return "double";
                case "Boolean":
                    return "bool";
                case "String":
                    return "string";
                default:
                    return type.Name;
            }
        }

        public static string[] SplitInput(string input, string encapsulationCharacter)
        {
            var words = new List<string>();
            var commandName = Regex.Match(input, @"^([\w\-]+)").Value;
            words.Add(commandName);
            if (TryExtractArguments(input, encapsulationCharacter, out var arguments))
            {
                words.AddRange(arguments);
            }

            return words.ToArray();
        }

        public static bool TryExtractArguments(string input, string encapsulationCharacter, out string[] arguments)
        {
            var encapsulatingCharactersCount = Regex.Matches(input, encapsulationCharacter).Count;
            if (encapsulatingCharactersCount % 2 != 0)
            {
                arguments = Array.Empty<string>();
                return false;
            }

            var argumentsList = new List<string>();
            var clearedInput = input.Trim();
            var matches = Regex.Matches(clearedInput, $"\"([^{encapsulationCharacter}]*)\"|([^\\s]+)");
            for (var i = 1; i < matches.Count; i++)
            {
                var match = matches[i];
                var groups = match.Groups;
                for (var j = 1; j < groups.Count; j++)
                {
                    var group = match.Groups[j];
                    if (group.Success)
                    {
                        argumentsList.Add(group.Value);
                    }
                }
            }

            arguments = argumentsList.ToArray();
            return true;
        }

        public static string GetMethodDefinitionString(MethodInfo method)
        {
            var sb = new StringBuilder();
            sb.Append(method.Name);
            var parameters = method.GetParameters();
            foreach (var param in parameters)
            {
                var paramName = param.Name;
                var paramType = param.ParameterType;
                sb.Append($" <color=\"white\">{paramName}({TypeToSimpleTypeString(paramType)})</color>");
            }

            return sb.ToString();
        }
    }
}