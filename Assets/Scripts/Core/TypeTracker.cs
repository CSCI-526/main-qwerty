using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] public SentenceGeneratorBase promptGenerator;

    [SerializeField] private TMP_InputField inputField; // Player input
    [SerializeField] private TMP_Text promptText;       // Displayed prompt
    [SerializeField] private TMP_Text instructionText;       // Displayed prompt
    [SerializeField] private Image ability1, ability2;
    [SerializeField] private GameObject damageScreen;

    [SerializeField] private TypingEffectManager typingEffectManager; // manager of curses & buffs
    [SerializeField] private DamageManager damageManager;

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;
    private int errors;

    private int mode = 0; // 0 = none, 1 = attack, 2 = heal
    private bool awaitingTarget = true; // Whether we're asking for a target name
    private HashSet<int> activeErrors = new HashSet<int>();

    [SerializeField] private RectTransform caretRect;
    [SerializeField] private float caretBlinkRate = 0.5f;
    private float caretTimer = 0f;
    private bool caretVisible = true;

    private int numSubmissions = 0;
    private float averageWPM = 0f;
    private float averageAccuracy = 0f;
    private int damageDealt = 0;
    private int healingDone = 0;

    private TargetableController currentTarget;
    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public void OnEnable()
    {
        resetState();
        EnterTargetPhase();
        ResetMetrics();
    }

    public void OnDisable()
    {
        ReportStats();
    }

    private void ReportStats()
    {
        if (numSubmissions == 0)
            return;

        StatsEvent statsEvent = new StatsEvent
        {
            NumSubmissions = numSubmissions,
            AverageWPM = averageWPM,
            AverageAccuracy = averageAccuracy,
            DamageDealt = damageDealt,
            HealingDone = healingDone
        };
        gameManager.analyticsManager.PushAnalyticsEvent(statsEvent);
    }

    private void ResetMetrics()
    {
        numSubmissions = 0;
        averageWPM = 0f;
        averageAccuracy = 0f;
        damageDealt = 0;
        healingDone = 0;
    }

    private void Start()
    {
        instructionText.text = "Select ability: 1 for attack and 2 for healing.\n";
        promptText.text = "";

        inputField.text = "";
        inputField.onValueChanged.AddListener(OnInputChanged);

        if (caretRect != null)
            caretRect.gameObject.SetActive(false);

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

        caretTimer += Time.deltaTime;
        if (caretTimer >= caretBlinkRate)
        {
            caretTimer = 0f;
            caretVisible = !caretVisible;
            caretRect.gameObject.SetActive(caretVisible);
        }

        // Updates caret location
        positionCaret(inputField.text.Length);
    }

    // For changing abilities
    private void changeMode(int newMode)
    {
        // If they're already in that mode, do nothing
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
        instructionText.text = $"{modeName}. Enter <color=yellow>Target</color>:";
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

                if (currentTarget is ProjectileController)
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
                    promptText.text = "";
                    inputField.text = "";
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
                promptText.color = Color.gray;
                instructionText.text = "Type the prompt below!";

                inputField.text = "";
                timerStarted = true; // will start when they begin typing
                startTime = Time.time;

                FocusInputField();
            }
            else
            {
                instructionText.text = "Invalid Target. Try Again.";
                inputField.text = "";
                promptText.text = "";

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

        if (awaitingTarget || string.IsNullOrEmpty(prompt))
        {
            promptText.text = currentText;
        }
    }

    // Counts the errors as the player is typing
    private void countErrors(string input, string prompt)
    {
        int len = Mathf.Min(input.Length, prompt.Length);

        string outputText = "";
        promptText.color = Color.white;

        string newPrompt = NormalizeText(prompt);
        string newInput = NormalizeText(input);

        HashSet<int> newErrors = new HashSet<int>();

        // Check all characters that overlap with the prompt
        for (int i = 0; i < len; i++)
        {
            if (newInput[i] != newPrompt[i])
            {
                newErrors.Add(i);
                outputText += $"<mark=#FF0000>{input[i]}</mark>";

                if (!activeErrors.Contains(i))
                {
                    errors++;
                    int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[0];
                    //gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).ModifyCurrentHealth(mod == 0 ? -2 : mod == 1 ? -4 : -1);
                    damageManager.applyHealthChange(gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId), mod == 0 ? -2 : mod == 1 ? -4 : -1);
                }
            }
            else
            {
                outputText += prompt[i];
            }
        }

        if (len < prompt.Length)
        {
            outputText += $"<color=#888888>{prompt.Substring(len)}</color>"; // remaining
        }

        // Count any extra characters typed beyond the prompt as errors
        for (int i = prompt.Length; i < input.Length; i++)
        {
            newErrors.Add(i);
            outputText += $"<mark=#FF0000>{input[i]}</mark>";
            if (!activeErrors.Contains(i))
            {
                errors++;
                int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[0];
                //gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).ModifyCurrentHealth(-5);
                damageManager.applyHealthChange(gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId), mod == 0 ? -2 : mod == 1 ? -4 : -1);
            }
        }

        activeErrors = newErrors;
        promptText.text = outputText;
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

        numSubmissions++;
        averageWPM = ((averageWPM * (numSubmissions - 1)) + grossWPM) / numSubmissions;
        averageAccuracy = ((averageAccuracy * (numSubmissions - 1)) + accuracy) / numSubmissions;

        int healthModifier = (int)((grossWPM / 5f) * Mathf.Pow(accuracy / 100f, 1.25f));

        if (mode == 1)
        {
            int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[2];
            //currentTarget.ModifyCurrentHealth(mod == 0 ? -healthModifier : mod == 1 ? -healthModifier / 2 : -healthModifier * 2);
            damageManager.applyHealthChange(currentTarget, mod == 0 ? -healthModifier : mod == 1 ? -healthModifier / 2 : -healthModifier * 2);
            damageDealt -= mod == 0 ? -healthModifier : mod == 1 ? -healthModifier / 2 : -healthModifier * 2;
        }
        else if (mode == 2)
        {
            int mod = gameManager.typingEffectManager.ApplyEffectOnMod()[1];
            //currentTarget.ModifyCurrentHealth(mod == 0 ? healthModifier : mod == 1 ? healthModifier / 2 : healthModifier * 2);
            damageManager.applyHealthChange(currentTarget, mod == 0 ? healthModifier : mod == 1 ? healthModifier / 2 : healthModifier * 2);
            healingDone += mod == 0 ? healthModifier : mod == 1 ? healthModifier / 2 : healthModifier * 2;
        }
        currentTarget.RandomizeTargetWord();
        currentTarget = null;

        resetState();
        EnterTargetPhase();
    }

    // Dummy function to test targeting
    private bool IsValidTarget(string target)
    {
        if (target == "Hello")
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
        awaitingTarget = true;
        promptText.color = Color.white;

        FocusInputField();
    }

    private string NormalizeText(string text)
    {
        if (text == null)
        {
            return "";
        }

        string normalized = text.Normalize(System.Text.NormalizationForm.FormKC);
        normalized = normalized.Replace('‘', '\'')
                               .Replace("‘", "'")
                               .Replace("’", "'")
                               .Replace("“", "\"")
                               .Replace("”", "\"")
                               .Replace("–", "-")
                               .Replace("—", "-")
                               .Replace("…", "...");

        return normalized;
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

    private void positionCaret(int caretIndex)
    {
        if (caretRect == null)
        {
            return;
        }

        TMP_Text activeText = promptText;

        if (activeText == null)
        {
            return;
        }

        activeText.ForceMeshUpdate();

        if (activeText is TMP_Text && activeText.textInfo.characterCount == 0)
        {
            Canvas.ForceUpdateCanvases();
            activeText.ForceMeshUpdate();
        }

        var textInfo = activeText.textInfo;
        int charCount = textInfo.characterCount;

        Vector3 localPos;

        if (charCount == 0)
        {
            var rt = activeText.rectTransform;
            localPos = new Vector3(rt.rect.xMin + 4f, rt.rect.yMin * -.65f, 0f);
        }
        else
        {
            int idx = Mathf.Clamp(caretIndex, 0, charCount);
            TMP_CharacterInfo ci;

            if (idx == 0)
            {
                ci = textInfo.characterInfo[0];
                localPos = new Vector3(ci.origin, ci.baseLine, 0);
            }
            else if (idx >= charCount)
            {
                ci = textInfo.characterInfo[charCount - 1];
                localPos = new Vector3(ci.xAdvance, ci.baseLine, 0);
            }
            else
            {
                ci = textInfo.characterInfo[idx - 1];
                localPos = new Vector3(ci.xAdvance, ci.baseLine, 0);
            }
        }

        Vector3 worldPos = activeText.transform.TransformPoint(localPos);
        RectTransform parentRect = caretRect.parent as RectTransform;
        Canvas canvas = activeText.canvas;
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        Vector2 anchored;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, cam, out anchored);

        caretRect.anchoredPosition = anchored;

        caretRect.anchoredPosition += new Vector2(0, caretRect.sizeDelta.y * 0.3f);
    }
}
