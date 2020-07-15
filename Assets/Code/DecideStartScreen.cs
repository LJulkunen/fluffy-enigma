using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecideStartScreen : MonoBehaviour
{
    public void LoadNextScene()
    {
        //save.ResetSave();
        if (SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel] == 0)
        {
            SceneManager.LoadScene("AloeCareStartScreen");
        }
        else
        {
            SceneManager.LoadScene("StartScreen");
        }
    }
}
