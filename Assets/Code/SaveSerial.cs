﻿using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    #region forCounterReduction
    // Reduction = remainder + modulo
    float affectionCounterReduction;
    float hungerCounterReduction;
    float affectionRemainder;
    float hungerRemainder;
    float affectionModulo;
    float hungerModulo;
    #endregion

    public float timer = 0.0f;

    [SerializeField]
    public bool isThisForJustMyTesting;

    public bool isPetting;
    public bool isFeeding;

    int daysReduction = 0;

    private static SaveSerial save;

    void Awake()
    {
        if (save != null && save != this)
        {
            Debug.LogError("Destroyed newer save");
            Destroy(this);
        } else
        {
            save = this;
            DontDestroyOnLoad(save);
        }
    }

    void Start()
    {
        maxCounter = howManyHours * 3600;
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //ResetSave();
        }

        LoadGame();

        //Debug.Log(13 % 4);
        /*UpdateHungerLvl();
        UpdateAffectionLvl();
        UpdateSatisfiedLvl();*/
        //Debug.Log(DateTime.UtcNow);
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
    // Might be useful with testing. Hide when you give a build tho.
    void OnGUI()
    {
        if (isThisForJustMyTesting == true)
        {
            if (GUI.Button(new Rect(100, 50, 125, 50),
                    "Reset Save Data"))
                ResetSave();
        }
    }

    public void CreateSave()
    {
        hungerLvlToSave = 3;
        affectionLvlToSave = 3;
        satisfiedLvlToSave = 3;
        hungerTimeToSave = DateTime.UtcNow;
        affectionTimeToSave = DateTime.UtcNow;
        SaveGame();
    }
    public void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
                       + "/MySaveData.dat");
        }
    }
    public void ResetSave()
    {
        DeleteSave();
        CreateSave();
        /*if (File.Exists(Application.persistentDataPath
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
            Debug.LogError("No save data to delete.");*/
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
        data.savedHungerTime = hungerTimeToSave.Ticks;
        data.savedAffectionTime = affectionTimeToSave.Ticks;
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
            hungerTimeToSave = DateTime.FromBinary(data.savedHungerTime);
            affectionTimeToSave = DateTime.FromBinary(data.savedAffectionTime);
            Debug.Log("Game data loaded!");
            UpdateHungerLvl();
            UpdateAffectionLvl();
            UpdateSatisfiedLvl();
            data.savedHungerLvl = hungerLvlToSave;
            data.savedAffectionTime = affectionTimeToSave.Ticks;
            data.savedHungerTime = hungerTimeToSave.Ticks;
            data.savedSatisfiedLvl = satisfiedLvlToSave;
        }
        else
        {
            //Debug.LogError("There is no save data!");
        }
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
            hungerTimeToSave = DateTime.UtcNow;
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
            affectionTimeToSave = DateTime.UtcNow;
            SaveGame();
        }

        UpdateSatisfiedLvl();
    }

    void UpdateAffectionLvl()
    {
        TimeSpan timeSpan = DateTime.UtcNow - Convert.ToDateTime(affectionTimeToSave);
        
        Debug.Log("Time was: " + Convert.ToDateTime(affectionTimeToSave) + " and time is: " + DateTime.UtcNow);
        Debug.Log("It's been this long since last petting: " + timeSpan);
        Debug.Log("Affection level was: " + affectionLvlToSave);

        if (affectionLvlToSave > 0)
        {
            if (timeSpan.Days > 0)
            {
                // Reducing affection stat.
                daysReduction = (timeSpan.Days * 24) / howManyHours;
                Debug.Log("What?! You've ignored the pet for over a day... :( Here's how much will be added to stacks lowering: " + daysReduction);
                Debug.Log("Amount decreased should be: " + daysReduction);
                affectionLvlToSave -= daysReduction;

                // Reducing affection counter.
                affectionRemainder = (float) timeSpan.TotalDays - timeSpan.Days;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = (affectionRemainder + affectionModulo) * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(true, affectionCounterReduction);
            }
             else if (timeSpan.Days == 0 & timeSpan.Hours < 0)
            {
                // Reducing affection stat.
                affectionLvlToSave += timeSpan.Hours / howManyHours;
                float affectionCounterReduction = (((float)timeSpan.TotalHours * 24 / howManyHours) * 3600) - timeSpan.Hours / howManyHours;
                affectionCounter -= affectionCounterReduction;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = (float)timeSpan.TotalHours - timeSpan.Hours;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = (affectionRemainder + affectionModulo) * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(false, affectionCounterReduction);
            }
            else
            {
                // Reducing affection stat.
                affectionLvlToSave -= timeSpan.Hours / howManyHours;
                float affectionCounterReduction = ((float)timeSpan.TotalHours - timeSpan.Hours) * 3600;
                affectionCounter -= affectionCounterReduction;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = (float)timeSpan.TotalHours - timeSpan.Hours;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = (affectionRemainder + affectionModulo) * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(false, affectionCounterReduction);
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
        TimeSpan timeSpan = DateTime.UtcNow - Convert.ToDateTime(hungerTimeToSave);
        Debug.Log("Time was: " + Convert.ToDateTime(hungerTimeToSave) + " and time is: " + DateTime.UtcNow);
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

                // Reducing hunger counter.
                hungerRemainder = (float)timeSpan.TotalDays - timeSpan.Days;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = (hungerRemainder + affectionModulo) * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(true, hungerCounterReduction);
            }
            else if (timeSpan.Days == 0 && timeSpan.Hours < 0)
            {
                hungerLvlToSave += timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing hunger counter.
                hungerRemainder = (float)timeSpan.TotalHours - timeSpan.Hours;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = (hungerRemainder + affectionModulo) * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(false, hungerCounterReduction);
            }
            else
            {
                hungerLvlToSave -= timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing hunger counter.
                hungerRemainder = (float)timeSpan.TotalHours - timeSpan.Hours;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = (hungerRemainder + affectionModulo) * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(false, hungerCounterReduction);
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
    void UpdateSatisfiedLvl()
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
    void UpdateAffectionCounter(bool isItOverADay, float reduced)
    {
        affectionCounter -= reduced;
        Debug.Log("Affection counter reduced by: " + reduced);
    }
    void UpdateHungerCounter(bool isItOverADay, float reduced)
    {
        hungerCounter -= reduced;
        Debug.Log("Hunger counter reduced by: " + reduced);
    }

    public void Exit()
    {
        SaveGame();
        Application.Quit();
    }
}

[Serializable]
class SaveData
{

    public int savedHungerLvl;
    public long savedHungerTime;

    public int savedAffectionLvl;
    public long savedAffectionTime;

    public int savedSatisfiedLvl;
}



