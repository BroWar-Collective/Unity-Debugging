using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private AutocompleteHandler autocompleteHandler;

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
                string previousInput = currentInput;
                currentInput = GUILayout.TextField(currentInput, Style.consoleTextStyle);
                if (previousInput != currentInput)
                {
                    InputChanged();
                }
                if (forceInputFocus)
                {
                    GUI.FocusControl(inputFieldControlName);
                }

                if (updateCursorPosition)
                {
                    GUI.FocusControl(inputFieldControlName);
                    var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.OnFocus();
                    editor.cursorIndex = currentInput.Length;
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
                    GUILayout.Label($"{currentAutocompleteMatch} (Press {inputSettings.AutocompleteKey} to apply)");
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

            currentInput = string.Empty;
            printedTextBuilder = new StringBuilder();
            inputKeysToActions = new Dictionary<Key, Action>()
            {
                { inputSettings.ConfirmationKey, InvokeInputString },
                { inputSettings.NextCommandKey, FetchNextCommand },
                { inputSettings.PrevCommandKey, FetchPrevCommand },
                { inputSettings.AutocompleteKey, PerformAutocomplete }
            };

            autocompleteHandler = new AutocompleteHandler();
            windowRect = styleSettings.InitialRect;
            scrollPosition = Vector2.zero;
            isInitialized = true;
        }

        private void InputChanged()
        {
            UpdateAutocomplete();
        }

        private void UpdateAutocomplete()
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                currentAutocompleteMatch = string.Empty;
                return;
            }

            var splitInput = currentInput.Split(' ');
            autocompleteHandler.RefreshOptions(consoleManager, splitInput, splitInput.Length - 1);
            currentAutocompleteMatch = autocompleteHandler.GetBestMatch(splitInput.Last());
        }

        private void InvokeInputString()
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                return;
            }

            printedTextBuilder.AppendLine(currentInput);
            var resultObject = consoleManager.InvokeCommand(currentInput);
            var stringResult = ParseResultToString(resultObject);
            if (!string.IsNullOrEmpty(stringResult))
            {
                printedTextBuilder.AppendLine(stringResult);
            }

            printedText = printedTextBuilder.ToString();
            printedText = printedText.TrimEnd();
            currentInput = string.Empty;
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
                currentInput = entry;
                updateCursorPosition = true;
            }
        }

        private void FetchPrevCommand()
        {
            if (consoleManager.TryGetPrevEntryFromHistory(out var entry))
            {
                currentInput = entry;
                updateCursorPosition = true;
            }
        }

        private void PerformAutocomplete()
        {
            if (string.IsNullOrEmpty(currentAutocompleteMatch))
            {
                return;
            }

            if (currentInput.Contains(' '))
            {
                currentInput = currentInput.Trim().Substring(0, currentInput.LastIndexOf(' '));
                currentInput = $"{currentInput} {currentAutocompleteMatch}";
            }
            else
            {
                currentInput = currentAutocompleteMatch;
            }

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
            var currentInputLength = currentInput.Length;
            currentInput = currentInput.Substring(0, currentInputLength - inputStringLength);
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

        public bool IsShown { get; private set; }
    }
}