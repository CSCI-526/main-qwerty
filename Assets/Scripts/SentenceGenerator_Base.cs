using UnityEngine;

public class SentenceGeneratorBase: MonoBehaviour
{
    // Loads a text file from Resources and returns a random line
    public string GetRandomSentence(string fileNameWithoutExtension)
    {
        TextAsset textFile = Resources.Load<TextAsset>(fileNameWithoutExtension);
        if (textFile == null)
        {
            Debug.LogWarning($"Text file not found in Resources: {fileNameWithoutExtension}");
            return "";
        }

        string[] lines = textFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            Debug.LogWarning($"Text file is empty: {fileNameWithoutExtension}");
            return "";
        }

        int randomIndex = Random.Range(0, lines.Length);
        Debug.Log($"New Random Sentence: {lines[randomIndex].Trim()}");
        return lines[randomIndex].Trim();
    }
}
