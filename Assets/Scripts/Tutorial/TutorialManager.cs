using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class TutorialManager : MonoBehaviour
{
    public ClassBase currentClass;

    private int tutorialStep = 0;
    private int promptStep = 0;

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    public bool isTutorialActive => gameManager.gameLoopManager.GetTutorialState();
    private int tutorialLength => currentClass.instructionText.Count - 1;

    public void Start()
    {

    }

    public bool abilityAllowed(int ability)
    {
        if (!isTutorialActive)
        {
            return true;
        }

        if (tutorialStep >= 2 && tutorialStep < 12 && ability == 1)
        {
            Debug.Log("1 Pressed and allowed");
            return true;
        }
        if ((tutorialStep < 3 || tutorialStep > 5) && tutorialStep < tutorialLength && ability == 2)
        {
            Debug.Log("2 Pressed and allowed");
            return true;
        }
        if ((tutorialStep < 6 || tutorialStep > 8) && tutorialStep < tutorialLength && ability == 3)
        {
            Debug.Log("3 Pressed and allowed");
            return true;
        }
        if ((tutorialStep < 9 || tutorialStep > 11) && tutorialStep < tutorialLength && ability == 3)
        {
            Debug.Log("3 Pressed and allowed");
            return true;
        }

        return false;

    }

    public bool checkTarget(TargetableController target)
    {
        if (target == null)
        {
            return false;
        }

        if (target.tag == currentClass.targetList[tutorialStep / 3])
        {
            return true;
        }


        return false;
    }

    public bool ShouldBlockAbilityInput()
    {
        return isTutorialActive && tutorialStep < tutorialLength;
    }

    public bool ShouldBlockTargetInput(string input)
    {
        if (!isTutorialActive) return false;

        // replicate your old restrictions here
        return false;
    }

    public string getInstruction()
    {
        if (isTutorialActive)
        {
            return currentClass.instructionText[tutorialStep];
        }
        return "";
    }

    public string getPrompt()
    {
        if (!isTutorialActive)
            return null;

        if (promptStep < currentClass.promptText.Count)
            return currentClass.promptText[promptStep++];

        return null;
    }

    public void incrementTutorial()
    {
        tutorialStep++;
    }
}