using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TypeTracker typeTracker;
    [SerializeField] private GameManager gameManager;

    private int currentStep = 0;
    private bool tutorialActive = false;

    private List<TutorialStep> steps;

    private void Start()
    {
        // Only start tutorial if not already completed
        if (!gameManager.gameLoopManager.GetTutorialState())
            return;

        tutorialActive = true;
        steps = new List<TutorialStep>()
        {
            new TutorialStep("Select an ability (press 1). Let's try attacking first!", TutorialTrigger.AbilitySelected),
            new TutorialStep("Enter the enemy's <color=yellow>Target word</color> (word in yellow).", TutorialTrigger.TargetEntered),
            new TutorialStep("Type the prompt carefully! Speed & accuracy affect your damage.", TutorialTrigger.PromptCompleted),
            new TutorialStep("Great job! Tutorial complete.", TutorialTrigger.TutorialEnd)
        };

        instructionText.text = steps[0].message;

        // Subscribe to events from TypeTracker
        typeTracker.OnAbilitySelected += HandleAbilitySelected;
        typeTracker.OnTargetConfirmed += HandleTargetConfirmed;
        typeTracker.OnPromptFinished += HandlePromptFinished;
    }

    private void HandleAbilitySelected()
    {
        AdvanceStep(TutorialTrigger.AbilitySelected);
    }

    private void HandleTargetConfirmed()
    {
        AdvanceStep(TutorialTrigger.TargetEntered);
    }

    private void HandlePromptFinished()
    {
        AdvanceStep(TutorialTrigger.PromptCompleted);
    }

    private void AdvanceStep(TutorialTrigger trigger)
    {
        if (!tutorialActive || currentStep >= steps.Count)
            return;

        if (steps[currentStep].trigger == trigger)
        {
            currentStep++;

            if (currentStep < steps.Count)
            {
                instructionText.text = steps[currentStep].message;
            }
            else
            {
                EndTutorial();
            }
        }
    }

    private void EndTutorial()
    {
        tutorialActive = false;
        instructionText.text = "Tutorial complete!";

        // Update the server-side variable so it won't run again
        gameManager.gameLoopManager.SetTutorialState(false);

        // Unsubscribe from events
        typeTracker.OnAbilitySelected -= HandleAbilitySelected;
        typeTracker.OnTargetConfirmed -= HandleTargetConfirmed;
        typeTracker.OnPromptFinished -= HandlePromptFinished;
    }
}

[System.Serializable]
public class TutorialStep
{
    public string message;
    public TutorialTrigger trigger;

    public TutorialStep(string message, TutorialTrigger trigger)
    {
        this.message = message;
        this.trigger = trigger;
    }
}

public enum TutorialTrigger
{
    AbilitySelected,
    TargetEntered,
    PromptCompleted,
    TutorialEnd
}
