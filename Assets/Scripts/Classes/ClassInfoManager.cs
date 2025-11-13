using UnityEngine;
using TMPro;

public class ClassInfoManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectedClass, passive, ability1, ability2, ability3, ability4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateUI(ClassBase playerClass)
    {
        selectedClass.text += playerClass.classDescription[0];
        passive.text = playerClass.classDescription[1];
        ability1.text = playerClass.classDescription[2];
        ability2.text = playerClass.classDescription[3];
        ability3.text = playerClass.classDescription[4];
        ability4.text = playerClass.classDescription[5];
    }
}
