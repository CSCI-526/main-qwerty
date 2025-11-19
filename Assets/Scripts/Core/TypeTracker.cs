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
using System;
using System.Threading;

public class TypeTracker : MonoBehaviour
{
    [SerializeField] public SentenceGeneratorBase promptGenerator;

    [SerializeField] private TMP_InputField inputField; // Player input
    [SerializeField] private TMP_Text promptText;       // Displayed prompt
    [SerializeField] private TMP_Text instructionText;       // Displayed prompt
    [SerializeField] private Image ability1, ability2, ability3, ability4;
    [SerializeField] private GameObject damageScreen;
    [SerializeField] private float modMultiplier = 1.2f;

    [SerializeField] private TypingEffectManager typingEffectManager; // manager of curses & buffs
    [SerializeField] private DamageManager damageManager;
    [SerializeField] private ClassInfoManager classInfoManager;
    [SerializeField] private TutorialManager tutorialManager;

    private string prompt;
    private bool timerStarted = false;
    private float startTime = 0f;
    private int errors;

    private int mode = 0; 
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

    private int phase = 0;
    private bool tutorialIncremented = false;

    private TargetableController currentTarget;

    public ClassBase currentClass = new BalancedClass();

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public void OnEnable()
    {
        if (!gameManager.gameLoopManager.GetTutorialState())
        {
            resetState();
            EnterTargetPhase();
        }
    }

    private void Start()
    {
        classInfoManager.updateUI(currentClass);

        getInstructions();
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

        if (Input.GetKeyDown(KeyCode.Alpha1) && shiftHeld == false  && phase == 0)
        {
            if(tutorialManager.isTutorialActive && tutorialManager.abilityAllowed(1))
            {
                tutorialManager.incrementTutorial();
                changeMode(1);
            }
            else
            {
                Debug.Log("Ability 1 not allowed");
                return;
            }
            changeMode(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && shiftHeld == false && phase == 0)
        {
            if (tutorialManager.isTutorialActive && tutorialManager.abilityAllowed(2))
            {
                tutorialManager.incrementTutorial();
                changeMode(2);
            }
            else
            {
                Debug.Log("Ability 2 not allowed");
                return;
            }
            changeMode(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && shiftHeld == false && phase == 0)
        {
            if (tutorialManager.isTutorialActive && tutorialManager.abilityAllowed(3))
            {
                tutorialManager.incrementTutorial();
                changeMode(3);
            }
            else
            {
                Debug.Log("Ability 3 not allowed");
                return;
            }
            changeMode(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && shiftHeld == false && phase == 0)
        {
            if (tutorialManager.isTutorialActive && tutorialManager.abilityAllowed(4))
            {
                tutorialManager.incrementTutorial();
                changeMode(4);
            }
            else
            {
                Debug.Log("Ability 4 not allowed");
                return;
            }
            changeMode(4);
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
        {
            if (tutorialManager.isTutorialActive)
            {
                EnterTargetPhase();
            }
            else
            {
                return;
            }
        }

        mode = newMode;

        ability1.color = new Color(0f, 0f, 0f, 0.3f);
        ability2.color = new Color(0f, 0f, 0f, 0.3f);
        ability3.color = new Color(0f, 0f, 0f, 0.3f);
        ability4.color = new Color(0f, 0f, 0f, 0.3f);


        if (mode == 1)
        {
            ability1.color = new Color(0f, 1f, 0f, 1f); // Green at 100% opacity
        }
        if (mode == 2)
        {
            ability2.color = new Color(0f, 1f, 0f, 1f); // Green at 100% opacity
        }
        if (mode == 3)
        {
            ability3.color = new Color(0f, 1f, 0f, 1f); // Green at 100% opacity
        }
        if (mode == 4)
        {
            ability4.color = new Color(0f, 1f, 0f, 1f); // Green at 100% opacity
        }

        resetState();
        EnterTargetPhase();
    }

    private void EnterTargetPhase()
    {
        awaitingTarget = true;
        getInstructions();
        phase = 1;
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
                        currentClass.Ability1(gameManager.networkManager.LocalClientId, currentTarget, 0);
                    }
                    else if (mode == 2)
                    {
                        currentClass.Ability2(gameManager.networkManager.LocalClientId, currentTarget, 0);
                    }
                    else if (mode == 3)
                    {
                        currentClass.Ability3(gameManager.networkManager.LocalClientId, currentTarget, 0);
                    }
                    else if (mode == 4)
                    {
                        currentClass.Ability4(gameManager.networkManager.LocalClientId, currentTarget, 0);
                    }
                    currentTarget = null;
                    promptText.text = "";
                    inputField.text = "";

                    if (tutorialManager.isTutorialActive)
                    {
                        Debug.Log("In current target projectile");
                        phase = 0;
                        mode = 0;
                        awaitingTarget = true;
                        promptText.color = Color.white;
                        getInstructions();
                    }
                    else
                    {
                        EnterTargetPhase();
                    }
                    return;
                }

                if(!tutorialManager.isTutorialActive)
                {
                    getPrompt();
                    getInstructions();
                    FocusInputField();
                }
                else
                {
                    if(tutorialManager.checkTarget(currentTarget))
                    {
                        tutorialManager.incrementTutorial();
                        getPrompt();
                        getInstructions();
                        FocusInputField();
                    }
                    else
                    {
                        instructionText.text = "Invalid Target. Try Again.";

                        inputField.text = "";
                        promptText.text = "";

                        FocusInputField();
                    }
                }
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
        if (phase == 0)
        {

            // Restrict only during tutorial
            if (tutorialManager.isTutorialActive)
            {
                bool valid = false;

                if (tutorialManager.abilityAllowed(int.Parse(currentText)))
                {
                    valid = true;
                }

                // If invalid input during tutorial
                if (!valid)
                {

                    inputField.text = "";
                    getInstructions();
                    return;
                }
            }
            else
            {
                // General rule outside tutorial: Only allow 1–4
                if (currentText != "1" && currentText != "2" && currentText != "3" && currentText != "4")
                {
                    inputField.text = "";
                    getInstructions();
                    return;
                }
            }

            // Clear text after pressing a valid number
            inputField.text = "";
            return;
        }

        if (mode == 0)
        {
            inputField.text = "";
            return;
        }

        // Prevent 1 or 2 from appearing if pressed (we handle those separately)
        if ((currentText == "1" || currentText == "2" || currentText == "3" || currentText == "4") && phase != 2)
        {
            Debug.Log("Phase: " + phase);
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
                    //Debug.Log("TypeTracker: "+ gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).name);
                    //Debug.Log("TypeTracker: " + gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).transform.position);

                    damageManager.applyHealthChange(gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId), mod == 0 ? -2 : mod > 0 ? (-2 * (int)Math.Pow(modMultiplier, mod)) : -1);
                    gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).ModifyCurrentHealth(mod == 0 ? -2 : mod > 0 ? (-2 * (int)Math.Pow(modMultiplier, mod)) : -1);
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
                damageManager.applyHealthChange(gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId), mod == 0 ? -2 : mod > 0 ? (-2 * (int)Math.Pow(modMultiplier, mod)) : -1);
                gameManager.GetPlayerByClientId(gameManager.networkManager.LocalClientId).ModifyCurrentHealth(mod == 0 ? -2 : mod > 0 ? (-2 * (int)Math.Pow(modMultiplier, mod)) : -1);
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

        if(prompt.Length != input.Length)
        {
            errors += Math.Abs(prompt.Length - input.Length);
        }

        if (input.Length > 0)
        {
            int correctCharacters = Mathf.Max(0, input.Length - errors);
            float ratio = correctCharacters / (float)input.Length;
            accuracy = ratio * 100f;
        }
        else
        {
            accuracy = 0f;
        }

        // Damage calculation to make speed more forgiving and errors more penalizing. Also has a floor of 1 damage.
        float wpmFactor = Math.Min(1, Mathf.Log10((grossWPM + 14) / 14));
        float accuracyFactor = Mathf.Pow(accuracy / 100f, 2f);
        float modifier = wpmFactor * accuracyFactor;

        gameManager.analyticsManager.addNumSubmissions(1);
        gameManager.analyticsManager.addAverageWPM(grossWPM);
        gameManager.analyticsManager.addAverageAccuracy(accuracy);

        if (mode == 1)
        {
            currentClass.Ability1(gameManager.networkManager.LocalClientId, currentTarget, modifier);
            gameManager.analyticsManager.addAbility1Uses(1);
        }
        else if (mode == 2)
        {
            currentClass.Ability2(gameManager.networkManager.LocalClientId, currentTarget, modifier);
            gameManager.analyticsManager.addAbility2Uses(1);
        }
        else if (mode == 3)
        {
            currentClass.Ability3(gameManager.networkManager.LocalClientId, currentTarget, modifier);
            gameManager.analyticsManager.addAbility3Uses(1);
        }
        else if (mode == 4)
        {
            currentClass.Ability4(gameManager.networkManager.LocalClientId, currentTarget, modifier);
            gameManager.analyticsManager.addAbility4Uses(1);
        }
        currentTarget.RandomizeTargetWord();
        currentTarget = null;

        resetState();

        if(tutorialManager.isTutorialActive)
        {
            tutorialManager.incrementTutorial();
        }

        if (!gameManager.gameLoopManager.GetTutorialState())
        {
            EnterTargetPhase();
        }
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

        if(tutorialManager.isTutorialActive && phase == 2)
        {
            phase = 0;
            mode = 0;
            getInstructions();
        }
        else
        {
            phase = 1;
            getInstructions();
        }
            FocusInputField();
    }

    // Probably get rid of this eventually and just fix the prompts
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


    // Focuses the text field
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

    // Caret positioning settings
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

    private void getInstructions()
    {
        if(tutorialManager.isTutorialActive)
        {
            tutorialManager.getInstruction();
        }
        else
        {
            switch (phase)
            {
                case 0:
                    instructionText.text = "Please select an ability 1-4.";
                    break;
                case 1:
                    instructionText.text = "Enter a <color=yellow>Target word</color>:";
                    break;
                case 2:
                    instructionText.text = "Type the prompt below.";
                    break;
            }
        }
    }

    private void getPrompt()
    {
        phase = 2;
        if (tutorialManager.isTutorialActive)
        {
            promptText.text = tutorialManager.getPrompt();
            prompt = promptText.text; // For comparisons
            promptText.color = Color.gray;

            inputField.text = "";
            timerStarted = true; // will start when they begin typing
            startTime = Time.time;

            FocusInputField();
        }
        else
        {
            if (mode == 1)
            {
                //Debug.Log(currentClass.promptFileNames);
                string temp = promptGenerator.GetRandomSentence(currentClass.promptFileNames[0]);
                promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
            }
            else if (mode == 2)
            {
                string temp = promptGenerator.GetRandomSentence(currentClass.promptFileNames[1]);
                promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
            }
            else if (mode == 3)
            {
                string temp = promptGenerator.GetRandomSentence(currentClass.promptFileNames[2]);
                promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
            }
            else if (mode == 4)
            {
                string temp = promptGenerator.GetRandomSentence(currentClass.promptFileNames[3]);
                promptText.text = gameManager.typingEffectManager.ApplyEffectOnPrompt(ref temp);
            }

            prompt = promptText.text; // For comparisons
            promptText.color = Color.gray;

            inputField.text = "";
            timerStarted = true; // will start when they begin typing
            startTime = Time.time;

            FocusInputField();
        }
    }
}
