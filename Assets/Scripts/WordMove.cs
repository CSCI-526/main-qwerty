using TMPro;
using UnityEngine;

public class WordMove : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int wordSpeed = 5;
    [SerializeField] private int collideX = -500;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(-0.001f * wordSpeed, 0, 0));
        if (gameObject.GetComponent<RectTransform>().anchoredPosition.x <= collideX - 500)
        {
            string word = gameObject.GetComponent<TMP_Text>().text;
            Debug.Log("Hit by word: " + word);
            gameObject.GetComponentInParent<EnemyAttack>().RemoveWord(word);
            Destroy(gameObject);
        }
    }
}
