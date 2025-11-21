using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class TutorialManager : MonoBehaviour
{
    public ClassBase currentClass;

    private int tutorialStep = 0;
    private int promptStep = 0;

    GameManager gameManager => FindFirstObjectByType<GameManager>();

    private int tutorialLength => currentClass.instructionText.Count - 1;
    private bool tutorialEnd = false;
    public bool isTutorialActive => !tutorialEnd && gameManager.gameLoopManager.GetTutorialState();

    private static List<ParticleSystem> allParticles = new List<ParticleSystem>();

    private void Start()
    {
        ParticleSystem[] found = Resources.FindObjectsOfTypeAll<ParticleSystem>();

        foreach (var ps in found)
        {
            // Make sure it's actually part of the scene, not a prefab in the project folder
            if (ps.gameObject.scene.IsValid())
            {
                registerParticle(ps);
            }
        }
    }
    public bool abilityAllowed(string ability)
    {
        if (!isTutorialActive || tutorialStep == tutorialLength)
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
        if (target.tag == currentClass.targetList[tutorialStep / 3][0])
        {
            Debug.Log("Target Match");
            return true;
        }


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

        if(tutorialStep == 1)
        {
            turnOnParticles(currentClass.targetList[0][0]);
        }
        else if(tutorialStep == 4)
        {
            turnOnParticles(currentClass.targetList[1][0]);
        }
        else if(tutorialStep == 7)
        {
            turnOnParticles(currentClass.targetList[2][0]);
        }
        else if(tutorialStep == 10)
        {
            turnOnParticles(currentClass.targetList[3][0]);
        }
        else if(tutorialStep == 13)
        {
            if (currentClass.targetList.Count > 4 && currentClass.targetList[4] != null)
            {
                turnOnParticles(currentClass.targetList[4][0]);
            }
        }

        if (tutorialStep > tutorialLength)
        {
            endTutorial();
        }
    }

    public void endTutorial()
    {
        Debug.Log("Tutorial Ended");
        tutorialEnd = true;
        turnOnParticles("LOL");
        gameManager.IncrementTutorialFinishedCountRpc();
    }

    private void turnOnParticles(string tag)
    {
        Debug.Log("Turning on " + tag + " particles");
        foreach (var ps in allParticles)
        {
            if (ps == null) continue;

            if (ps.CompareTag(tag))
            {
                Debug.Log("Particle Turned On.");
                ps.Play();
            }
            else
            {
                Debug.Log("Particle Turned Off.");
                ps.Stop();
                ps.Clear();
            }
        }
    }

    public static void registerParticle(ParticleSystem particle)
    {
        if(!allParticles.Contains(particle))
        {
            Debug.Log("Particle Registered.");
            allParticles.Add(particle);
        }
    }
}