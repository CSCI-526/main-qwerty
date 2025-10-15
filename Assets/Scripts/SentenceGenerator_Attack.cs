using UnityEngine;

public class SentenceGeneratorAttack : SentenceGeneratorBase
{
    [Header("Text File Settings")]
    public string fileName = "sentences"; // Set in Inspector (no .txt extension)

    [Header("Output")]
    [TextArea(2, 5)]
    public string selectedSentence; // Shows the pulled sentence

    private void Start()
    {
        // Optionally pull one at start
        selectedSentence = GetRandomSentence(fileName);
        Debug.Log($"Random Sentence: {selectedSentence}");
    }

    private void Update()
    {
        // When you press the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PullRandomSentence();
        }
    }

    // Call this whenever you want to pull a new sentence
    public void PullRandomSentence()
    {
        selectedSentence = GetRandomSentence(fileName);
        Debug.Log($"New Random Sentence: {selectedSentence}");
    }
}
