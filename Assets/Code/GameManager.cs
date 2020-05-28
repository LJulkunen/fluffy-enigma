using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject affectionText;
    public GameObject hungerText;
    public GameObject kitsulope;

    void Start()
    {

    }

    // TODO: This.
    /*void Update()
    {
        affectionText.GetComponent<Text>().text = "" + kitsulope.GetComponent<Kitsulope>().affection.ToString();
        hungerText.GetComponent<Text>().text = "" + kitsulope.GetComponent<Kitsulope>().hunger.ToString();
    }

    public void ButtonBehaviour(int i)
    {
        switch (i)
        {
            case (0):
            default:
                kitsulope.GetComponent<Kitsulope>().FeedKitsulope();
                break;

            case (1):
                break;

            case (2):
                kitsulope.GetComponent<Kitsulope>().SaveKitsulope();
                Application.Quit();
                break;
        }
    }*/
}

