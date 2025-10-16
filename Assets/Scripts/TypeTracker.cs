using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Net;
using Unity.VisualScripting;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] public SentenceGeneratorBase promptGenerator;

    [SerializeField] private TMP_InputField inputField; // Player input
    [SerializeField] private TMP_Text promptText;       // Displayed prompt
    [SerializeField] private Image ability1, ability2;

    [SerializeField] private TypingEffectManager typingEffectManager; // manager of curses & buffs

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;
    private int errors;

    private int mode = 0; // 0 = none, 1 = attack, 2 = heal
    private bool awaitingTarget = true; // Whether we're asking for a target name
    private HashSet<int> activeErrors = new HashSet<int>();

    private TargetableController currentTarget;
    GameManager gameManager => FindFirstObjectByType<GameManager>();


    private void Start()
    {
        promptText.text = "Select ability: 1 for attack and 2 for healing.\n";

        inputField.text = "";
        inputField.onValueChanged.AddListener(OnInputChanged);

        FocusInputField();
    }

    private void Update()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.Alpha1) && shiftHeld == false)
        {
            changeMode(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && shiftHeld == false)
        {
            changeMode(2);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onEnter(inputField.text);
        }
    }

    // For changing abilities
    private void changeMode(int newMode)
    {
        // If they�re already in that mode, do nothing
        if (mode == newMode)
            return;

        mode = newMode;

        if (mode == 1)
        { 
            ability1.color = new Color(0f, 1f, 0f, 1f); // Green at 100% opacity
            ability2.color = new Color(0f, 0f, 0f, 0.3f); // Black at 30% opacity
        }
        if (mode == 2)
        {
            ability1.color = new Color(0f, 0f, 0f, 0.3f); // Green at 100% opacity
            ability2.color = new Color(0f, 1f, 0f, 1f); // Black at 30% opacity
        }

        resetState();
        EnterTargetPhase();
    }

    private void EnterTargetPhase()
    {
        awaitingTarget = true;
        string modeName = GetModeName();
        promptText.text = $"{modeName}. Enter Target:";
        Debug.Log($"Switched to {modeName} mode. Awaiting target...");
        FocusInputField();
    }

    // Called when player presses Enter
    private void onEnter(string input)
    {
        // If we're waiting for a target
        if (awaitingTarget)
        {
            currentTarget = gameManager.GetTargetFromWord(input);

            if (currentTarget != null)
            {
                awaitingTarget = false;

                if(currentTarget is ProjectileController)
                {
                    inputField.text = "";
                    if (mode == 1)
                    {
                        currentTarget.ModifyCurrentHealth(-10);
                    }
                    else if (mode == 2)
                    {
                        currentTarget.ModifyCurrentHealth(10);
                    }
                    currentTarget = null;
                    EnterTargetPhase();
                    return;
                }
                else if (mode == 1)
                {
                    string temp = promptGenerator.GetRandomSentence("Attack");
                    promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
                }
                else if (mode == 2)
                {
                    string temp = promptGenerator.GetRandomSentence("Heal");
                    promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
                }

                prompt = promptText.text; // For comparisons

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

        // If they�re typing the prompt and press Enter, end typing
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

        // Check all characters that overlap with the prompt
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

        // Count any extra characters typed beyond the prompt as errors
        for (int i = prompt.Length; i < input.Length; i++)
        {
            newErrors.Add(i);
            if (!activeErrors.Contains(i))
            {
                Debug.Log($"Extra character error at index {i}: '{input[i]}' beyond prompt length");
                errors++;
                // Add player damage here
            }
        }

        activeErrors = newErrors;
    }


    // Ends typing phase to calculate damage to enemy
    private void endTyping(string input)
    {
        float accuracy, totalTime;

        if (timerStarted)
        {
            totalTime = Time.time - startTime;
        }
        else
        {
            totalTime = 0f;
        }

        float totalMinutes = Mathf.Max(0.0001f, totalTime / 60f);

        float grossWPM = (float)input.Length / 5f / totalMinutes;
        float netWPM = grossWPM - (errors / totalMinutes);
        netWPM = Mathf.Max(0, netWPM);

        if (input.Length > 0)
        {
            int correctCharacters = Mathf.Max(0, input.Length - errors);
            float ratio = (float)correctCharacters / input.Length;
            accuracy = ratio * 100f;
        }
        else
        {
            accuracy = 0f;
        }

        if(mode == 1)
        {
            currentTarget.ModifyCurrentHealth(-10);
        }
        else if (mode == 2)
        {
            currentTarget.ModifyCurrentHealth(10);
        }
        currentTarget.RandomizeTargetWord();
        currentTarget = null;
        Debug.Log($"Typing Test Ended (Enter pressed)");
        Debug.Log($"Time: {totalTime:F2}s | Gross WPM: {grossWPM:F1} | Net WPM: {netWPM:F1} | Accuracy: {accuracy:F1}% | Errors: {errors}");

        resetState();
        EnterTargetPhase();
    }

    // Dummy function to test targeting
    // Replace with Josh's list generation stuff
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

    // For getting mode names for UI
    private string GetModeName()
    {
        if (mode == 1)
        {
            return "Attack";
        }
        else if (mode == 2)
        {
            return "Heal";
        }
        else
        {
            return "None";
        }
    }

    // Resets all values
    private void resetState()
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

    // Ensures text field is always active
    private void FocusInputField()
    {
        if (inputField == null) return;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }

        inputField.Select();
        inputField.ActivateInputField();
    }
}
