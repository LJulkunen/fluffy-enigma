using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject affectionText;
    public GameObject hungerText;
    //public GameObject kitsulope;
    public GameObject saveSerial;
    

    void Start()
    {
        //affectionText.GetComponent<Text>().text = "" + kitsulope.GetComponent<SaveSerial>().affection.ToString();
        //hungerText.GetComponent<Text>().text = "" + kitsulope.GetComponent<SaveSerial>().hungerLvlToSave.ToString();
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
                //saveSerial.Feed();
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

