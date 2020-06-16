﻿using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SaveSerial : MonoBehaviour
{
    [SerializeField]
    public int maxHungerLvl = 4;
    public int hungerLvlToSave = 0;
    DateTime hungerTimeToSave;

    [SerializeField]
    public int maxAffectionLvl = 4;
    public int affectionLvlToSave = 0;
    DateTime affectionTimeToSave;

    [SerializeField]
    public int maxSatisfiedLvl = 4;
    public int satisfiedLvlToSave = 0;


    [SerializeField]
    public int howManyHours = 4;

    [SerializeField]
    public float maxCounter = 28800;
    public float hungerCounter;
    public float affectionCounter;

    public float timer = 0.0f;

    [SerializeField]
    public bool isThisForJustMyTesting;

    public bool isPetting;
    public bool isFeeding;

    private void Start()
    {
        maxCounter = howManyHours * 3600;
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;
        LoadGame();
        /*UpdateHungerLvl();
        UpdateAffectionLvl();
        UpdateSatisfiedLvl();*/
        //Debug.Log(DateTime.Now);
    }

    public void Feed()
    {
        isFeeding = true;
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
            hungerTimeToSave = DateTime.Now;
            SaveGame();
        }

        UpdateSatisfiedLvl();
    }
    public void Pet()
    {
        isPetting = true;
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
            affectionTimeToSave = DateTime.Now;
            SaveGame();
        }

        UpdateSatisfiedLvl();
    }

    // Might be useful with testing. Hide when you give a build tho.
    private void OnGUI()
    {
        if (isThisForJustMyTesting == true)
        {
            if (GUI.Button(new Rect(100, 50, 125, 50),
                    "Reset Save Data"))
                ResetData();
        }
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.savedHungerLvl = hungerLvlToSave;
        data.savedAffectionLvl = affectionLvlToSave;
        data.savedSatisfiedLvl = satisfiedLvlToSave;
        data.savedHungerTime = hungerTimeToSave;
        data.savedAffectionTime = affectionTimeToSave;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data (hunger & affection & satisfaction levels & related times) saved!");
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
            hungerTimeToSave = data.savedHungerTime;
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

    int daysReduction = 0;
    void UpdateAffectionLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(affectionTimeToSave);
        
        Debug.Log("Time was: " + Convert.ToDateTime(affectionTimeToSave) + " and time is: " + DateTime.Now);
        Debug.Log("It's been this long since last petting: " + timeSpan);
        Debug.Log("Affection level was: " + affectionLvlToSave);
        if (affectionLvlToSave > 0)
        {
            if (timeSpan.Days > 0)
            {
                daysReduction = (timeSpan.Days * 24) / howManyHours;
                affectionLvlToSave -= daysReduction;
                Debug.Log("What?! You've ignored the pet for over a day... :( Here's how much will be added to stacks lowering: " + daysReduction);
                Debug.Log("Amount decreased should be: " + daysReduction);
            }
             else if (timeSpan.Days == 0 & timeSpan.Hours < 0)
            {
                if (isThisForJustMyTesting)
                {

                    affectionLvlToSave += timeSpan.Minutes / howManyHours;
                    //Debug.Log("Amount decreased should be: " + timeSpan.Minutes / howManyHours);
                }
                else
                {
                    affectionLvlToSave += timeSpan.Hours / howManyHours;
                    Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);
                }
            }
            else
            {
                if (isThisForJustMyTesting)
                {

                    affectionLvlToSave -= timeSpan.Minutes / howManyHours;
                    //Debug.Log("Amount decreased should be: " + timeSpan.Minutes / howManyHours);
                }
                else
                {
                    affectionLvlToSave -= timeSpan.Hours / howManyHours;
                    Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);
                }
            }

            if (affectionLvlToSave >= maxAffectionLvl)
            {
                affectionLvlToSave = maxAffectionLvl;
            }
            else if (affectionLvlToSave > 0)
            {

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
        Debug.Log("Affection level is: " + affectionLvlToSave);
    }
    void UpdateHungerLvl()
    {
        TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(hungerTimeToSave);
        Debug.Log("Time was: " + Convert.ToDateTime(hungerTimeToSave) + " and time is: " + DateTime.Now);
        Debug.Log("This much time has passed since last offering..." + timeSpan);
        Debug.Log("Hunger level was: " + hungerLvlToSave); 

        if (hungerLvlToSave > 0)
        {
            if (timeSpan.Days > 0)
            {
                daysReduction = (timeSpan.Days * 24) / howManyHours;
                hungerLvlToSave -= daysReduction;
                Debug.Log("What?! You've ignored the pet for over a day... :( Here's how much will be added to stacks lowering: " + daysReduction);
                Debug.Log("Amount decreased should be: " + daysReduction);
            }
            else if (timeSpan.Days == 0 && timeSpan.Hours < 0)
            {
                hungerLvlToSave += timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);
            }
            else
            {
                hungerLvlToSave -= timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);
            }

            if (hungerLvlToSave >= maxHungerLvl)
            {
                hungerLvlToSave = maxHungerLvl;
                
            }
            else if (hungerLvlToSave > 0)
            {

            } else
            {
                hungerLvlToSave = 0;
            }
        } else
        {
            hungerLvlToSave = 0;
        }
        Debug.Log("Hunger level is: " + hungerLvlToSave);
    }

    private void UpdateSatisfiedLvl()
    {
        if (hungerLvlToSave == maxHungerLvl && affectionLvlToSave == maxAffectionLvl)
        {
            satisfiedLvlToSave = maxSatisfiedLvl;
            //Debug.Log("Your fluffy overlord is satisfied, good job!");
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
            //Debug.Log("Your fluffy overlord is mostly satisfied, keep it up!");
        } else if (satisfiedLvlToSave == 2)
        {
            //Debug.Log("Your fluffy overlord is okay...");
        } else if (satisfiedLvlToSave == 1)
        {
            //Debug.Log("Your fluffy overlord is getting impatient...");
        } else if (satisfiedLvlToSave == 0)
        {
            //Debug.Log("Your fluffy overlord says fuck off!");
        } else if (satisfiedLvlToSave < 0)
        {
            satisfiedLvlToSave = 0;
        }
    }

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
            UpdateSatisfiedLvl();
            SaveGame();
        }
        if (affectionCounter <= 0)
        {
            affectionCounter = maxCounter;
            affectionLvlToSave--;
            satisfiedLvlToSave--;
            UpdateSatisfiedLvl();
            SaveGame();
        }

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

    public void Exit()
    {
        Application.Quit();
    }

    void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
                      + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
                              + "/MySaveData.dat");
            hungerLvlToSave = 3;
            affectionLvlToSave = 3;
            satisfiedLvlToSave = 3;
            hungerTimeToSave = DateTime.Now;
            affectionTimeToSave = DateTime.Now;
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
    public DateTime savedHungerTime;

    public int savedAffectionLvl;
    public DateTime savedAffectionTime;

    public int savedSatisfiedLvl;
}



