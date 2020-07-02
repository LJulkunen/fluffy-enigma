using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecideStartScreen : MonoBehaviour
{

    public SaveSerial save;

    void Start()
    {
        if (save.hungerLvlToSave == 0)
        {
            SceneManager.LoadScene("AloeCareStartScreen");
        }
        else
        {
            SceneManager.LoadScene("StartScreen");
        }
    }
}
