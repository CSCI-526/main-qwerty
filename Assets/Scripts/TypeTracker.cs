using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;   // player input
    [SerializeField] private TMP_Text promptText;   // prompt text

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;

    private HashSet<int> activeErrors = new HashSet<int>(); // tracks which indices are wrong

    private void Start()
    {
        prompt = promptText != null ? promptText.text : "";
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
        if (!timerStarted && currentText.Length > 0 && prompt.Length > 0 && currentText[0] == prompt[0])
        {
            timerStarted = true;
            startTime = Time.time;
        }

        // Track current errors while typing
        countErrors(currentText, prompt);
    }

    private void countErrors(string input, string prompt)
    {
        int len = Mathf.Min(input.Length, prompt.Length);

        HashSet<int> newErrors = new HashSet<int>();

        for (int i = 0; i < len; i++)
        {
            if (input[i] != prompt[i])
            {
                newErrors.Add(i);

                if (!activeErrors.Contains(i))
                {
                    Debug.Log($"Error at index {i}: expected '{prompt[i]}', got '{input[i]}'");
                    // TODO: Apply damage here later if needed
                }
            }
        }

        /*
        foreach (int oldIndex in activeErrors)
        {
            if (oldIndex < len && input[oldIndex] == prompt[oldIndex])
            {
                Debug.Log($"Fixed error at index {oldIndex}");
            }
        }
        */

        // Replace old error set with new one
        activeErrors = newErrors;
    }


    private void endTyping(string input)
    {

        float totalTime = 0f;
        if (timerStarted)
        {
            totalTime = Time.time - startTime;
        }

        float totalMinutes = Mathf.Max(0.0001f, totalTime / 60f);

        int correctChars = countCorrect(input, prompt);
        int totalChars = prompt.Length;
        int totalErrors = countTotalErrors(input, prompt);

        float grossWPM = (float)input.Length / 5f / totalMinutes;
        float netWPM = grossWPM - (totalErrors / totalMinutes);
        netWPM = Mathf.Max(0, netWPM);

        // Debug.Log($"Prompt length: {prompt.Length}, Input length: {input.Length}");

        float accuracy = 0f;
        if(totalChars > 0)
        {
            accuracy = ((float) correctChars / totalChars) * 100f;
        }

        Debug.Log($"Typing Test Ended (Enter pressed)");
        Debug.Log($"Time: {totalTime:F2}s | Gross WPM: {grossWPM:F1} | Net WPM: {netWPM:F1} | Accuracy: {accuracy:F1}% | Errors: {totalErrors}");

        inputField.text = "";
        promptText.text = "";
        timerStarted = false;
        totalErrors = 0;
    }

    private int countCorrect(string input, string prompt)
    {
        int correct = 0;
        int len = Mathf.Min(input.Length, prompt.Length);
        for (int i = 0; i < len; i++)
        {
            if (input[i] == prompt[i])
                correct++;
        }
        return correct;
    }

    private int countTotalErrors(string input, string prompt)
    {
        int errors = 0;
        int len = Mathf.Min(input.Length, prompt.Length);

        for (int i = 0; i < len; i++)
        {
            if (input[i] != prompt[i])
            {
                errors++;
            }
        }

        // Count extra characters beyond prompt as errors
        if (input.Length > prompt.Length)
            errors += input.Length - prompt.Length;

        return errors;
    }
}
