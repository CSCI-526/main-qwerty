using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;   // player input
    [SerializeField] private TMP_Text promptText;   // prompt text

    [SerializeField] private GameObject typingEffectManager; // manager of curses & buffs    

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;
    private int errors;

    private HashSet<int> activeErrors = new HashSet<int>();

    private void Start()
    {
        if (promptText != null)
        {
            prompt = promptText.text;
        }
        else
        {
            prompt = "";
        }

        inputField.text = "";
        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            endTyping(inputField.text);
        }
    }

    private void OnInputChanged(string currentText)
    {
        // Start timer when first correct character is typed
        // Going to change this to after the prompt appears and they begin typing.
        if (!timerStarted && currentText.Length > 0 && prompt.Length > 0 && currentText[0] == prompt[0])
        {
            timerStarted = true;
            startTime = Time.time;
        }

        // Track current errors while typing
        countErrors(currentText, prompt);
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

    // Calculates the WPM and accuracy for damage calculations
    private void endTyping(string input)
    {

        float totalTime = 0f;
        if (timerStarted)
        {
            totalTime = Time.time - startTime;
        }

        float totalMinutes = Mathf.Max(0.0001f, totalTime / 60f);

        float grossWPM = (float)input.Length / 5f / totalMinutes;
        float netWPM = grossWPM - (errors / totalMinutes);
        netWPM = Mathf.Max(0, netWPM);

        float accuracy = 0f;
        if (input.Length > 0)
        {
            accuracy = ((float)(Mathf.Max(0, input.Length - errors)) / input.Length) * 100f;
        }

        // Add enemy damage logic here
        Debug.Log($"Typing Test Ended (Enter pressed)");
        Debug.Log($"Time: {totalTime:F2}s | Gross WPM: {grossWPM:F1} | Net WPM: {netWPM:F1} | Accuracy: {accuracy:F1}% | Errors: {errors}");

        inputField.text = "";
        promptText.text = "";
        timerStarted = false;
        startTime = 0;
        errors = 0;
        activeErrors = new HashSet<int>();
    }
}
