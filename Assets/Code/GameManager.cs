using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public GameObject kitsulope;

    public TextMeshProUGUI hungerText;

    public TextMeshProUGUI affectionText;

    public string textPrefix = "";

    public GameObject fridgeBubble;
    public GameObject fridgeBubbleText;

    void Start()
    {
        hungerText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
        affectionText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().affectionLvlToSave.ToString();
    }

    public void ButtonBehaviour(int i)
    {
        switch (i)
        {
            case (0):
            default:
                kitsulope.GetComponent<SaveSerial>().Feed();
                hungerText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
                break;

            case (1):
                kitsulope.GetComponent<SaveSerial>().Pet();
                affectionText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().affectionLvlToSave.ToString();
                break;

            case (2):
                fridgeBubble.SetActive(!fridgeBubble.activeInHierarchy);
                fridgeBubbleText.SetActive(!fridgeBubbleText.activeInHierarchy);
                break;
        }
    }
}

