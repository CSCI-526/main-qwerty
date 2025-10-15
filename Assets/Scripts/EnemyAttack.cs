using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int attackSpeed = 5;
    [SerializeField] private int minLength = 5;
    [SerializeField] private int maxLength = 8;
    private int attackCd = 0;

    [Header("Prefabs and GameObjects")]
    [SerializeField] private GameObject wordPrefab;

    private List<string> wordList = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackCd = attackSpeed * 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCd > 0)
        {
            attackCd--;
            if (attackCd == 0)
            {
                ShootWord();
                attackCd = attackSpeed * 60;
            }
        }
    }

    private void ShootWord()
    {
        string word = GenerateWord();
        GameObject projectile = Instantiate(wordPrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<TMP_Text>().text = word;
        projectile.transform.SetParent(gameObject.transform);
        projectile.transform.rotation = transform.rotation;
        projectile.transform.localScale = Vector3.one;
        wordList.Add(word);
        //string result = "";
        //foreach (var item in wordList)
        //{
        //    result += item.ToString() + ", ";
        //}
        //Debug.Log(result);
    }

    public void RemoveWord(string word)
    {
        wordList.Remove(word);
        //string result = "";
        //foreach (var item in wordList)
        //{
        //    result += item.ToString() + ", ";
        //}
        //Debug.Log(result);
    }

    private string GenerateWord()
    {
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        string[] vowels = { "a", "e", "i", "o", "u" };

        string word = "";

        int requestedLength = UnityEngine.Random.Range(minLength, maxLength + 1);

        // Generate the word in consonant / vowel pairs
        while (word.Length < requestedLength)
        {
            if (requestedLength != 1)
            {
                // Add the consonant
                string consonant = consonants[UnityEngine.Random.Range(0, consonants.Length)];

                if (consonant == "q" && word.Length + 3 <= requestedLength) // check +3 because we'd add 3 characters in this case, the "qu" and the vowel.  Change 3 to 2 to allow words that end in "qu"
                {
                    word += "qu";
                }
                else
                {
                    while (consonant == "q")
                    {
                        // Replace an orphaned "q"
                        consonant = consonants[UnityEngine.Random.Range(0, consonants.Length)];
                    }

                    if (word.Length + 1 <= requestedLength)
                    {
                        // Only add a consonant if there's enough room remaining
                        word += consonant;
                    }
                }
            }

            if (word.Length + 1 <= requestedLength)
            {
                // Only add a vowel if there's enough room remaining
                word += vowels[UnityEngine.Random.Range(0, vowels.Length)];
            }
        }

        //Debug.Log(word);
        return word;
    }
}
