using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField; // Player input
    [SerializeField] private TMP_Text promptText;       // Displayed prompt

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;
    private int errors;

    private int mode = 0; // 0 = none, 1 = attack, 2 = heal
    private bool awaitingTarget = false; // Whether we're asking for a target name
    private HashSet<int> activeErrors = new HashSet<int>();

    private void Start()
    {
        promptText.text = "Select ability: 1 for attack and 2 for healing.\n";

        inputField.text = "";
        inputField.onValueChanged.AddListener(OnInputChanged);

        FocusInputField();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            changeMode(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            changeMode(2);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onEnter(inputField.text);
        }
    }

    // Called when player presses 1 or 2
    private void changeMode(int newMode)
    {
        // If they’re already in that mode, do nothing
        if (mode == newMode)
            return;

        // Reset current typing state
        ResetTypingState();

        mode = newMode;
        awaitingTarget = true;

        string modeName = mode == 1 ? "Attacking" : "Healing";
        promptText.text = $"Mode: {modeName}\nEnter Target:";
        Debug.Log($"Switched to {modeName} mode. Awaiting target...");

        FocusInputField();
    }

    // Called when player presses Enter
    private void onEnter(string input)
    {
        // If we're waiting for a target
        if (awaitingTarget)
        {
            if (IsValidTarget(input))
            {
                awaitingTarget = false;

                // TODO: retrieve the prompt from your Prompt Generator here
                // Example:
                // prompt = PromptGenerator.GetPrompt(mode, input);
                // promptText.text = prompt;
                promptText.text = "The quick brown fox jumped over the lazy dog."; // placeholder
                prompt = promptText.text;

                Debug.Log($"Target '{input}' selected. Starting {GetModeName()} prompt...");
                inputField.text = "";
                timerStarted = true; // will start when they begin typing
                startTime = Time.time;

                FocusInputField();
            }
            else
            {
                promptText.text = "Invalid Target. Try Again.";
                Debug.Log($"Invalid target: {input}");
                inputField.text = "";

                FocusInputField();
            }

            return;
        }

        // If they’re typing the prompt and press Enter, end typing
        if (!awaitingTarget && !string.IsNullOrEmpty(prompt))
        {
            endTyping(input);
            FocusInputField();
        }
    }

    // Called when text changes (while typing)
    private void OnInputChanged(string currentText)
    {
        // Prevent 1 or 2 from appearing if pressed (we handle those separately)
        if (currentText == "1" || currentText == "2")
        {
            inputField.text = "";
            return;
        }

        // Only count errors or start timer if in active typing mode
        if (!awaitingTarget && timerStarted)
        {
            countErrors(currentText, prompt);
        }
    }

    // Counts the errors as the player is typing
    private void countErrors(string input, string prompt)
    {
        int len = Mathf.Min(input.Length, prompt.Length);

        HashSet<int> newErrors = new HashSet<int>();

        // Checks for errors and make sure it isn't accounting for previously accounted for errors.
        for (int i = 0; i < len; i++)
        {
            if (input[i] != prompt[i])
            {
                newErrors.Add(i);

                if (!activeErrors.Contains(i))
                {
                    Debug.Log($"Error at index {i}: expected '{prompt[i]}', got '{input[i]}'");
                    errors++;
                    // Add player damage here
                }
            }
        }

        activeErrors = newErrors;
    }

    // Ends typing phase (calculates stats)
    private void endTyping(string input)
    {
        float totalTime = timerStarted ? Time.time - startTime : 0f;
        float totalMinutes = Mathf.Max(0.0001f, totalTime / 60f);

        float grossWPM = (float)input.Length / 5f / totalMinutes;
        float netWPM = grossWPM - (errors / totalMinutes);
        netWPM = Mathf.Max(0, netWPM);

        float accuracy = input.Length > 0 ? ((float)Mathf.Max(0, input.Length - errors) / input.Length) * 100f : 0f;

        // TODO: Add enemy damage logic here
        Debug.Log($"Typing Test Ended (Enter pressed)");
        Debug.Log($"Time: {totalTime:F2}s | Gross WPM: {grossWPM:F1} | Net WPM: {netWPM:F1} | Accuracy: {accuracy:F1}% | Errors: {errors}");

        ResetTypingState();

        FocusInputField();
    }

    private bool IsValidTarget(string target)
    {
        if(target == "Hello")
        {
            Debug.Log("Valid Target Entered\n");
            FocusInputField();

            return true;
        }
        else
        {
            Debug.Log("Invalid Target Entered.\n");
            FocusInputField();

            return false;
        }
    }

    private string GetModeName()
    {
        return mode == 1 ? "Attack" : mode == 2 ? "Heal" : "None";
    }

    // Helper: resets everything when switching modes or finishing
    private void ResetTypingState()
    {
        inputField.text = "";
        promptText.text = "";
        timerStarted = false;
        startTime = 0;
        errors = 0;
        activeErrors.Clear();
        awaitingTarget = false;

        FocusInputField();
    }

    // Ensures the input field is selected and activated so the user can type immediately
    private void FocusInputField()
    {
        if (inputField == null) return;

        // Fallback to EventSystem selection first (helps in some Unity builds)
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }

        inputField.Select();
        inputField.ActivateInputField(); // ensures caret is visible and keyboard is ready
    }
}
