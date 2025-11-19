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

    private int tutorialLength => currentClass.instructionText.Count - 1;
    private bool tutorialEnd = false;
    public bool isTutorialActive => !tutorialEnd && gameManager.gameLoopManager.GetTutorialState();

    public bool abilityAllowed(string ability)
    {
        if (!isTutorialActive)
        {
            return true;
        }

        if ((tutorialStep <= 2 || tutorialStep >= 12) && ability == "1")
        {
            Debug.Log("1 Pressed and allowed");
            return true;
        }
        if ((tutorialStep >= 3 && tutorialStep <= 5) && ability == "2")
        {
            Debug.Log("2 Pressed and allowed");
            return true;
        }
        if ((tutorialStep >= 6 && tutorialStep <= 8) && ability == "3")
        {
            Debug.Log("3 Pressed and allowed");
            return true;
        }
        if ((tutorialStep >= 9 && tutorialStep <= 11)  && ability == "4")
        {
            Debug.Log("4 Pressed and allowed");
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

        Debug.Log("Target Tag: " + target.tag + "| Expected Target: " + currentClass.targetList[tutorialStep / 3]);
        if (target.tag == currentClass.targetList[tutorialStep / 3])
        {
            Debug.Log("Target Match");
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
        Debug.Log("Tutorial Incremented: " + tutorialStep);

        if(tutorialStep > tutorialLength)
        {
            endTutorial();
        }
    }

    public void endTutorial()
    {
        Debug.Log("Tutorial Ended");
        tutorialEnd = true;
        gameManager.IncrementTutorialFinishedCountRpc();
    }
}