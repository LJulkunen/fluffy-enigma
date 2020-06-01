using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject affectionText;
    //public GameObject hungerText;
    public GameObject kitsulope;
    //public GameObject saveSerial; 

    public TextMeshProUGUI text;
    public string textPrefix = "Hunger: ";
    //IEnumerator hungerloop;

    void Start()
    {
        //StartCoroutine(hungerloop);
        text.text = textPrefix + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
        //text.text = textPrefix + textValue.ToString();
        //affectionText.GetComponent<Text>().text = "" + kitsulope.GetComponent<SaveSerial>().affection.ToString();
        //hungerText.GetComponent<TextMesh>().text = "" + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
        //saveSerial = saveSerial.GetComponent<SaveSerial>();
    }

    // TODO: This.
    void Update()
    {
        
    }

    public void ButtonBehaviour(int i)
    {
        switch (i)
        {
            case (0):
            default:
                kitsulope.GetComponent<SaveSerial>().Feed();
                text.text = textPrefix + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
                break;

            case (1):
                break;

            /*case (2):
                kitsulope.GetComponent<Kitsulope>().SaveKitsulope();
                Application.Quit();
                break;*/
        }
    }
}

