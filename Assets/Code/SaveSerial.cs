using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SaveSerial : MonoBehaviour
{
    [SerializeField]
    int maxHungerLvl = 4;
    public int hungerLvlToSave;
    String timeToSave;

    [SerializeField]
    int maxAffectionLvl = 4;
    public int affectionLvlToSave;
    String affectionTimeToSave;

    [SerializeField]
    int maxPleasedLvl = 4;
    public int pleasedLvlToSave;

    private void Start()
    {
        LoadGame();
    }

    public void Feed()
    {
        if (hungerLvlToSave >= maxHungerLvl)
        {
            hungerLvlToSave = maxHungerLvl;
            timeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
        else
        {
            hungerLvlToSave++;
            Debug.Log("Hunger level is: " + hungerLvlToSave);
            timeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
    }
    public void Pet()
    {
        if (affectionLvlToSave >= maxAffectionLvl)
        {
            affectionLvlToSave = maxAffectionLvl;
            affectionTimeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
        else
        {
            affectionLvlToSave++;
            Debug.Log("Affection level is: " + affectionLvlToSave);
            affectionTimeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
    }

    // v Not used separately.
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
        data.savedAffectionLvl = affectionLvlToSave;
        data.savedPleasedLvl = pleasedLvlToSave;
        data.savedTime = timeToSave;
        data.savedAffectionTime = affectionTimeToSave;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data (hunger & affection level) saved!");
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
            affectionLvlToSave = data.savedAffectionLvl;
            pleasedLvlToSave = data.savedPleasedLvl;
            timeToSave = data.savedTime;
            affectionTimeToSave = data.savedAffectionTime;
            Debug.Log("Game data loaded!");
            UpdateHungerLvl();
            UpdateAffectionLvl();
            UpdatePleasedLvl();
            data.savedHungerLvl = hungerLvlToSave;
            data.savedAffectionTime = affectionTimeToSave;
            data.savedPleasedLvl = pleasedLvlToSave;
        }
        else
            Debug.LogError("There is no save data!");
    }

    void UpdateAffectionLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(affectionTimeToSave);
        Debug.Log("Time now: " + DateTime.Now + "Saved affectionTime: " + Convert.ToDateTime(affectionTimeToSave));
        Debug.Log("It's been this long since last petting: " + timeSpan);
        if (affectionLvlToSave > 0)
        {
            affectionLvlToSave -= timeSpan.Minutes;
            if (affectionLvlToSave >= maxAffectionLvl)
            {
                affectionLvlToSave = maxAffectionLvl;
            }
            else if (affectionLvlToSave > 0)
            {
                Debug.Log("Affection level has decreased to: " + affectionLvlToSave);
            }
            else
            {
                affectionLvlToSave = 0;
            }
        }
        else
        {
            affectionLvlToSave = 0;
        }
    }
    void UpdateHungerLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(timeToSave);
        Debug.Log("This much time has passed since last offering..." + timeSpan);
        if (hungerLvlToSave > 0)
        {
            hungerLvlToSave -= timeSpan.Minutes;
            if (hungerLvlToSave >= maxHungerLvl)
            {
                hungerLvlToSave = maxHungerLvl;
                
            }
            else if (hungerLvlToSave > 0)
            {
                Debug.Log("Hunger level has decreased to: " + hungerLvlToSave);
            } else
            {
                hungerLvlToSave = 0;
            }
        } else
        {
            hungerLvlToSave = 0;
        }
    }
    // TODO: Replace witch switch case later.
    private void UpdatePleasedLvl()
    {
        if (hungerLvlToSave == maxHungerLvl && affectionLvlToSave == maxAffectionLvl)
        {
            pleasedLvlToSave = maxPleasedLvl;
            Debug.Log("Your fluffy overlord is pleased, good job!");
        }
        else if (hungerLvlToSave > affectionLvlToSave)
        {
            pleasedLvlToSave = affectionLvlToSave;
        }
        else if (hungerLvlToSave < affectionLvlToSave)
        {
            pleasedLvlToSave = hungerLvlToSave;
        } else
        {
            pleasedLvlToSave = hungerLvlToSave;
        }

        if (pleasedLvlToSave == 3)
        {
            Debug.Log("Your fluffy overlord is mostly pleased, keep it up!");
        } else if (pleasedLvlToSave == 2)
        {
            Debug.Log("Your fluffy overlord is okay...");
        } else if (pleasedLvlToSave == 1)
        {
            Debug.Log("Your fluffy overlord is getting impatient...");
        } else
        {
            Debug.Log("Your fluffy overlord says fuck off!");
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
            affectionLvlToSave = 0;
            timeToSave = "";
            affectionTimeToSave = "";
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

    public int savedAffectionLvl;
    public String savedAffectionTime;

    public int savedPleasedLvl;
}



