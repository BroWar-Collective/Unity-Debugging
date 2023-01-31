using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BroWar.Debugging.Console
{
    [AddComponentMenu("BroWar/Debugging/Console/Console GUI")]
    [RequireComponent(typeof(ConsoleManager))]
    public class ConsoleGui : MonoBehaviour
    {
        private static class Style
        {
            internal static readonly GUIStyle consoleWindowStyle = new GUIStyle(GUI.skin.window)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            internal static readonly GUIStyle consoleButtonStyle = new GUIStyle(GUI.skin.button);
            internal static readonly GUIStyle consoleTextStyle = new GUIStyle(GUI.skin.textField);
            internal static readonly GUIStyle consoleBodyStyle = new GUIStyle(GUI.skin.textArea)
            {
                richText = true
            };
        }

        private const string inputFieldControlName = "ConsoleGUI_InputField";

        [SerializeField]
        private ConsoleManager consoleManager;
        [Title("Settings")]
        [SerializeField]
        private bool forceInputFocus;
        [SerializeField]
        private ConsoleInputSettings inputSettings;
        [SerializeField]
        private ConsoleStyleSettings styleSettings;
        [SerializeField]
        private float minDistanceFromEdge;

        private Rect windowRect;
        private Vector2 scrollPosition;

        private string printedText;
        private string currentInput;
        private string currentAutocompleteMatch;
        private StringBuilder printedTextBuilder;

        private bool updateCursorPosition;
        private bool isInitialized;

        private Dictionary<Key, Action> inputKeysToActions;

        private readonly AutocompleteHandler autocompleteHandler = new AutocompleteHandler();

        private void Update()
        {
            if (IsKeyPressed(inputSettings.TriggerKey))
            {
                Toggle();
            }

            if (!IsShown)
            {
                return;
            }

            foreach (var item in inputKeysToActions)
            {
                var inputKey = item.Key;
                if (IsKeyPressed(inputKey))
                {
                    var action = item.Value;
                    action.Invoke();
                }
            }
        }

        private void OnGUI()
        {
            if (!IsShown)
            {
                return;
            }

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = styleSettings.MainColor;

            var requiredWindowSize = GetRequiredWindowSize();
            var validWindowPosition = GetValidWindowPosition();

            windowRect = new Rect(validWindowPosition, requiredWindowSize);
            windowRect = GUI.Window(0, windowRect, DrawWindow, "Console Window", Style.consoleWindowStyle);
            GUI.backgroundColor = prevColor;
        }

        private void DrawWindow(int windowId)
        {
            Style.consoleTextStyle.normal.textColor = styleSettings.TextColor;
            Style.consoleBodyStyle.normal.textColor = styleSettings.TextColor;
            using (var scrollView = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                GUILayout.Label(printedText, Style.consoleBodyStyle, GUILayout.ExpandHeight(true));
            }

            using (new GUILayout.HorizontalScope())
            {
                GUI.SetNextControlName(inputFieldControlName);
                CurrentInput = GUILayout.TextField(CurrentInput, Style.consoleTextStyle);
                if (forceInputFocus)
                {
                    GUI.FocusControl(inputFieldControlName);
                }

                if (updateCursorPosition)
                {
                    GUI.FocusControl(inputFieldControlName);
                    var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.OnFocus();
                    editor.cursorIndex = CurrentInput.Length;
                    updateCursorPosition = false;
                }

                var buttonContent = new GUIContent("Apply");
                var size = Style.consoleButtonStyle.CalcSize(buttonContent);
                if (GUILayout.Button(buttonContent, GUILayout.Width(size.x)))
                {
                    InvokeInputString();
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (!string.IsNullOrEmpty(currentAutocompleteMatch))
                {
                    GUILayout.Label($"{currentAutocompleteMatch} <color=yellow>(Press {inputSettings.AutocompleteKey} to apply)</color>");
                }
            }

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        private void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            CurrentInput = string.Empty;
            printedTextBuilder = new StringBuilder();
            inputKeysToActions = new Dictionary<Key, Action>()
            {
                { inputSettings.ConfirmationKey, InvokeInputString },
                { inputSettings.NextCommandKey, FetchNextCommand },
                { inputSettings.PrevCommandKey, FetchPrevCommand },
                { inputSettings.AutocompleteKey, PerformAutocomplete }
            };

            windowRect = styleSettings.InitialRect;
            scrollPosition = Vector2.zero;
            isInitialized = true;
        }

        private void OnInputChange()
        {
            var encapsulationCharacter = consoleManager.Settings.MultiargumentEncapsulationCharacter;
            currentAutocompleteMatch = autocompleteHandler.GetBestMatch(consoleManager, CurrentInput, encapsulationCharacter);
        }

        private void InvokeInputString()
        {
            if (string.IsNullOrEmpty(CurrentInput))
            {
                return;
            }

            printedTextBuilder.AppendLine(CurrentInput);
            var resultObject = consoleManager.InvokeCommand(CurrentInput);
            var stringResult = ParseResultToString(resultObject);
            if (!string.IsNullOrEmpty(stringResult))
            {
                printedTextBuilder.AppendLine(stringResult);
            }

            printedText = printedTextBuilder.ToString();
            printedText = printedText.TrimEnd();
            CurrentInput = string.Empty;
            currentAutocompleteMatch = string.Empty;
            MoveScrollToBottom();
        }

        private string ParseResultToString(object resultObject)
        {
            if (resultObject is CommandResult commandResult)
            {
                return ParseResultToString(commandResult);
            }

            return resultObject != null
                ? resultObject.ToString()
                : string.Empty;
        }

        private string ParseResultToString(CommandResult commandResult)
        {
            var messageType = commandResult.MessageType;
            switch (messageType)
            {
                case MessageType.Error:
                    return $"<color=red>{commandResult.Message}</color>";
                case MessageType.Warning:
                    return $"<color=yellow>{commandResult.Message}</color>";
                case MessageType.Log:
                default:
                    return commandResult.Message;
            }
        }

        private void FetchNextCommand()
        {
            if (consoleManager.TryGetNextEntryFromHistory(out var entry))
            {
                CurrentInput = entry;
                updateCursorPosition = true;
            }
        }

        private void FetchPrevCommand()
        {
            if (consoleManager.TryGetPrevEntryFromHistory(out var entry))
            {
                CurrentInput = entry;
                updateCursorPosition = true;
            }
        }

        private void PerformAutocomplete()
        {
            if (string.IsNullOrEmpty(currentAutocompleteMatch))
            {
                return;
            }

            var match = Regex.Match(CurrentInput.Trim(), @"^(.*)\s[^\s]+$");
            CurrentInput = match.Success
                ? $"{match.Groups[1].Value} {currentAutocompleteMatch}"
                : currentAutocompleteMatch;

            updateCursorPosition = true;
        }

        private bool IsKeyPressed(Key key)
        {
            return Keyboard.current[key].wasPressedThisFrame;
        }

        private void RemoveTriggerKeyTextFromInput()
        {
            var triggerKey = inputSettings.TriggerKey;
            if (!triggerKey.IsTextInputKey())
            {
                return;
            }

            var inputString = Keyboard.current[triggerKey].displayName;
            var inputStringLength = inputString.Length;
            var currentInputLength = CurrentInput.Length;
            CurrentInput = CurrentInput.Substring(0, currentInputLength - inputStringLength);
        }

        private Vector2 GetRequiredWindowSize()
        {
            var initialRectSize = styleSettings.InitialRect.size;
            var requiredWindowSize = new Vector2
                (Screen.width < initialRectSize.x ? Screen.width : initialRectSize.x,
                Screen.height < initialRectSize.y ? Screen.height : initialRectSize.y);

            return requiredWindowSize;
        }

        private Vector2 GetValidWindowPosition()
        {
            var windowPosition = windowRect.position;
            var windowSize = windowRect.size;

            if (windowPosition.x > Screen.width - minDistanceFromEdge)
            {
                windowPosition.x = Screen.width - minDistanceFromEdge;
            }
            else if (windowPosition.x < minDistanceFromEdge - windowSize.x)
            {
                windowPosition.x = minDistanceFromEdge - windowSize.x;
            }

            if (windowPosition.y > Screen.height - minDistanceFromEdge)
            {
                windowPosition.y = Screen.height - minDistanceFromEdge;
            }
            else if (windowPosition.y < minDistanceFromEdge - windowSize.y)
            {
                windowPosition.y = minDistanceFromEdge - windowSize.y;
            }

            return windowPosition;
        }

        private void MoveScrollToBottom()
        {
            scrollPosition.y = float.MaxValue;
        }

        public void Toggle()
        {
            if (IsShown)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void Show()
        {
            EnsureInitialized();
            IsShown = true;
        }

        public void Hide()
        {
            EnsureInitialized();
            RemoveTriggerKeyTextFromInput();
            consoleManager.SaveHistory();
            IsShown = false;
        }

        public void Clear()
        {
            printedTextBuilder?.Clear();
            printedText = string.Empty;
        }

        private string CurrentInput
        {
            get => currentInput;
            set
            {
                if (currentInput == value)
                {
                    return;
                }

                currentInput = value;
                OnInputChange();
            }
        }

        public bool IsShown { get; private set; }
    }
}