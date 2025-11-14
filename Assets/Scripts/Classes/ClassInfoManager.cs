using UnityEngine;
using TMPro;

public class ClassInfoManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectedClass, passive, tabability1, tabability2, tabability3, tabability4;
    [SerializeField] TMP_Text ability1, ability2, ability3, ability4;
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
        tabability1.text = playerClass.classDescription[2];
        tabability2.text = playerClass.classDescription[3];
        tabability3.text = playerClass.classDescription[4];
        tabability4.text = playerClass.classDescription[5];

        ability1.text += playerClass.abilityDescription[0];
        ability2.text += playerClass.abilityDescription[1];
        ability3.text += playerClass.abilityDescription[2];
        ability4.text += playerClass.abilityDescription[3];
    }
}
