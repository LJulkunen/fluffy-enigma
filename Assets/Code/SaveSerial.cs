using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SaveSerial : MonoBehaviour
{
    [SerializeField]
    int maxHungerLvl = 4;

    int hungerLvlToSave;
    String timeToSave;

    [SerializeField]
    public Button button;

    private Touch theTouch;

    public GameObject hungerText;

    private void Start()
    {
        LoadGame();
        //hungerText.GetComponent<Text>().text = "" + hungerLvlToSave.ToString();
    }

    private void OnGUI()
    {
        if (button)
        {
            if (Input.touchCount > 0)
            {
                theTouch = Input.GetTouch(0);
                if (theTouch.phase == TouchPhase.Ended)
                {
                } else
                {
                    Feed();
                }
            }
        }
    }

    public void Feed()
    {
        if (hungerLvlToSave >= maxHungerLvl)
        {
            hungerLvlToSave = maxHungerLvl;
        }
        else
        {
            hungerLvlToSave++;
            Debug.Log("Hunger level is: " + hungerLvlToSave);
            timeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
    }

        // v Not used separately.
        /*if (GUI.Button(new Rect(750, 0, 125, 50), "Save Your Game"))
            SaveGame();*/
        /*if (GUI.Button(new Rect(750, 100, 125, 50),
                    "Load Your Game"))
            LoadGame();*/
        /*if (GUI.Button(new Rect(750, 200, 125, 50),
                    "Reset Save Data"))
            ResetData();*/

    void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.savedHungerLvl = hungerLvlToSave;
        data.savedTime = timeToSave;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data (hunger level) saved!");
    }

    void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/MySaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            hungerLvlToSave = data.savedHungerLvl;
            timeToSave = data.savedTime;
            Debug.Log("Game data loaded!");
            UpdateHungerLvl();
            data.savedHungerLvl = hungerLvlToSave;
        }
        else
            Debug.LogError("There is no save data!");
    }

    void UpdateHungerLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(timeToSave);
        Debug.Log("This much time has passed since last offering..." + timeSpan);
        if (hungerLvlToSave > 0)
        {
            hungerLvlToSave -= timeSpan.Seconds;
            if (hungerLvlToSave == maxHungerLvl)
            {
                Debug.Log("Your fluffy overlord is pleased, good job!");
            }
            else
            {
                Debug.Log("Hunger level has decreased to: " + hungerLvlToSave);
            }
            if (hungerLvlToSave < 0)
            {
                hungerLvlToSave = 0;
            }
        }
    }
    void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
                      + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
                              + "/MySaveData.dat");
            hungerLvlToSave = 0;
            timeToSave = "";
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }
}

[Serializable]
class SaveData
{
    public int savedHungerLvl;
    public String savedTime;
}



