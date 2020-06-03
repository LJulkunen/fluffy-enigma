using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SaveSerial : MonoBehaviour
{
    [SerializeField]
    public int maxHungerLvl = 4;
    public int hungerLvlToSave = 0;
    String timeToSave;

    [SerializeField]
    public int maxAffectionLvl = 4;
    public int affectionLvlToSave = 0;
    String affectionTimeToSave;

    [SerializeField]
    public int maxSatisfiedLvl = 4;
    public int satisfiedLvlToSave = 0;

    [SerializeField]
    public float maxCounter = 28800;
    public float hungerCounter;
    public float affectionCounter;

    public float timer = 0.0f;

    private void Start()
    {
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;
        LoadGame();
    }

    public void Feed()
    {
        if (hungerLvlToSave < 0)
        {
            hungerLvlToSave = 0;
            SaveGame();
        }
        else if (hungerLvlToSave >= maxHungerLvl)
        {
            hungerLvlToSave = maxHungerLvl;
            SaveGame();
        }
        else if (hungerLvlToSave < maxHungerLvl)
        {
            hungerLvlToSave++;
            hungerCounter = maxCounter;
            Debug.Log("Hunger level is: " + hungerLvlToSave);
            timeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
    }
    public void Pet()
    {
        if (affectionLvlToSave < 0)
        {
            affectionLvlToSave = 0;
            SaveGame();
        }
        else if (affectionLvlToSave >= maxAffectionLvl)
        {
            affectionLvlToSave = maxAffectionLvl;
            SaveGame();
        }
        else
        {
            affectionLvlToSave++;
            affectionCounter = maxCounter;
            Debug.Log("Affection level is: " + affectionLvlToSave);
            affectionTimeToSave = DateTime.Now.ToLongTimeString();
            SaveGame();
        }
    }

    // Might be useful with testing. Hide when you give a build tho.
    /*private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 50, 125, 50),
                "Reset Save Data"))
            ResetData();
    }*/

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.savedHungerLvl = hungerLvlToSave;
        data.savedAffectionLvl = affectionLvlToSave;
        data.savedSatisfiedLvl = satisfiedLvlToSave;
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
            satisfiedLvlToSave = data.savedSatisfiedLvl;
            timeToSave = data.savedTime;
            affectionTimeToSave = data.savedAffectionTime;
            Debug.Log("Game data loaded!");
            UpdateHungerLvl();
            UpdateAffectionLvl();
            UpdateSatisfiedLvl();
            data.savedHungerLvl = hungerLvlToSave;
            data.savedAffectionTime = affectionTimeToSave;
            data.savedSatisfiedLvl = satisfiedLvlToSave;
        }
        else
            Debug.LogError("There is no save data!");
    }

    void UpdateAffectionLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(affectionTimeToSave);
        //Debug.Log("Time now: " + DateTime.Now + "Saved affectionTime: " + Convert.ToDateTime(affectionTimeToSave));
        Debug.Log("It's been this long since last petting: " + timeSpan);
        if (affectionLvlToSave > 0)
        {
            //affectionLvlToSave -= timeSpan.Minutes;
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
            //hungerLvlToSave -= timeSpan.Minutes;
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
    private void UpdateSatisfiedLvl()
    {
        if (hungerLvlToSave == maxHungerLvl && affectionLvlToSave == maxAffectionLvl)
        {
            satisfiedLvlToSave = maxSatisfiedLvl;
            Debug.Log("Your fluffy overlord is satisfied, good job!");
        }
        else if (hungerLvlToSave > affectionLvlToSave)
        {
            satisfiedLvlToSave = affectionLvlToSave;
        }
        else if (hungerLvlToSave < affectionLvlToSave)
        {
            satisfiedLvlToSave = hungerLvlToSave;
        } else
        {
            satisfiedLvlToSave = hungerLvlToSave;
        }

        if (satisfiedLvlToSave == 3)
        {
            Debug.Log("Your fluffy overlord is mostly satisfied, keep it up!");
        } else if (satisfiedLvlToSave == 2)
        {
            Debug.Log("Your fluffy overlord is okay...");
        } else if (satisfiedLvlToSave == 1)
        {
            Debug.Log("Your fluffy overlord is getting impatient...");
        } else if (satisfiedLvlToSave == 0)
        {
            Debug.Log("Your fluffy overlord says fuck off!");
        } else if (satisfiedLvlToSave < 0)
        {
            satisfiedLvlToSave = 0;
        }
    }

    // TODO: This would also be an appropriate plase for the timer later.
    void Update()
    {
        timer += Time.deltaTime;
        float seconds = timer % 60;

        if (seconds >= 1)
        {
            hungerCounter--;
            affectionCounter--;
            timer = 0;
        }

        if (hungerCounter <= 0)
        {
            hungerCounter = maxCounter;
            hungerLvlToSave--;
            satisfiedLvlToSave--;
            UpdateHungerLvl();
            SaveGame();
        }
        if (affectionCounter <= 0)
        {
            affectionCounter = maxCounter;
            affectionLvlToSave--;
            satisfiedLvlToSave--;
            UpdateAffectionLvl();
            SaveGame();
        }
        
        UpdateSatisfiedLvl();
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                // Quit the application
                Application.Quit();
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

    public int savedSatisfiedLvl;
}



