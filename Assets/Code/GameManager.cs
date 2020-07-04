using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI affectionText;

    public string textPrefix = "";

    public GameObject fridgeBubble;
    public GameObject fridgeBubbleText;

    void Start()
    {
        //hungerText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
        //affectionText.text = textPrefix + kitsulope.GetComponent<SaveSerial>().affectionLvlToSave.ToString();
    }

    public void ButtonBehaviour(int i)
    {
        switch (i)
        {
            case (0):
            default:
                SaveSerial.SAVE.Feed();
                hungerText.text = textPrefix + SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel].ToString();
                break;

            case (1):
                SaveSerial.SAVE.Pet();
                affectionText.text = textPrefix + SaveLoad.SaveData[(int)SaveLoad.Line.AffectionLevel].ToString();
                break;

            case (2):
                //downBubble.SetActive(!downBubble.activeInHierarchy);
                //bubbleTextObject.SetActive(!bubbleTextObject.activeInHierarchy);
                break;
        }
    }
}

