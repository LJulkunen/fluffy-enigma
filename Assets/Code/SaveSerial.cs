﻿using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSerial : MonoBehaviour
{
    #region KitsulopeVariables
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
    #endregion

    public static SaveSerial SAVE;
    public int isIntroOver = 0;
    public int version = 0;
    public TextMeshProUGUI debugText;

    /* TODO: [x] Save aloe stats.
     *       [] Change sprite after a day if aloe watered enough.
     *       [] 4 aloe levels. 0 being just got nibbled by kitsulope, 4 fully grown.
     *       [x] WateredLevel just probably needs to be 0 or 1?
     *       [x] A high max counter for waterLevel dropping (a day probably).
     *       [] Counter should work in the exact same way as it does with the actual pet.
    */
    public int aloeWatered = 0;
    public int aloeLevel = 0;
    DateTime aloeWateredTime;

    public int maxSapientLevel = 7;
    public int sapientLevel = 0;

    public int isAloeIntroOver = 0;

    void Awake()
    {
        #region singleton
        if (SAVE != null && SAVE != this)
        {
            //Debug.LogError("Destroyed newer save");
            Destroy(this);
            return;
        } else
        {
            SAVE = this;
            DontDestroyOnLoad(SAVE);
        }
        #endregion

        // initializes long array
        SaveLoad.SaveData = new long[SaveLoad.SAVEDATA_LENGHT];

        LoadGame();
        Debug.Log("Sapient level is: " + sapientLevel);
    }

    void Start()
    {
        maxCounter = howManyHours * 3600;
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;
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
                //isIntroOver = 1;
                // Quit the application
                SaveGame();
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

    void CreateSave()
    {
        // set default values
        hungerLvlToSave = 3;
        affectionLvlToSave = 3;
        satisfiedLvlToSave = 3;
        hungerTimeToSave = DateTime.UtcNow;
        affectionTimeToSave = DateTime.UtcNow;
        isIntroOver = 0;
        aloeWatered = 0;
        aloeLevel = 0;
        aloeWateredTime = DateTime.UtcNow;
        sapientLevel = 0;
        
        SaveGame();
    }

    public void ResetSave()
    {
        SaveLoad.Delete();
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
        // populating array with correct values
        SaveLoad.SaveData[(int)SaveLoad.Line.Version] = version;
        SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel] = hungerLvlToSave;
        SaveLoad.SaveData[(int)SaveLoad.Line.HungerTime] = hungerTimeToSave.Ticks;
        SaveLoad.SaveData[(int)SaveLoad.Line.AffectionLevel] = affectionLvlToSave;
        SaveLoad.SaveData[(int)SaveLoad.Line.AffectionTime] = affectionTimeToSave.Ticks;
        SaveLoad.SaveData[(int)SaveLoad.Line.SatisfiedLevel] = satisfiedLvlToSave;
        SaveLoad.SaveData[(int)SaveLoad.Line.IntroOver] = isIntroOver;
        SaveLoad.SaveData[(int)SaveLoad.Line.AloeWatered] = aloeWatered;
        SaveLoad.SaveData[(int)SaveLoad.Line.AloeLevel] = aloeLevel;
        SaveLoad.SaveData[(int)SaveLoad.Line.AloeWateredTime] = aloeWateredTime.Ticks;
        SaveLoad.SaveData[(int)SaveLoad.Line.SapientLevel] = sapientLevel;
        SaveLoad.SaveData[(int)SaveLoad.Line.AloeIntroOver] = isAloeIntroOver;
        // saving correct values
        SaveLoad.Save(debugText);

        /*BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.savedHungerLvl = hungerLvlToSave;
        data.savedAffectionLvl = affectionLvlToSave;
        data.savedSatisfiedLvl = satisfiedLvlToSave;
        data.savedHungerTime = hungerTimeToSave.Ticks;
        data.savedAffectionTime = affectionTimeToSave.Ticks;
        data.savedIsIntroOver = isIntroOver;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data (hunger & affection & satisfaction levels & related times) saved!");*/
    }

    void LoadGame()
    {
        // if savegame load it else make savegame
        if (SaveLoad.Find())
            SaveLoad.Load(debugText);
        else
            CreateSave();

        // assign local values from saved ones
        version = (int)SaveLoad.SaveData[(int)SaveLoad.Line.Version];
        hungerLvlToSave = (int)SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel];
        hungerTimeToSave = DateTime.FromBinary(SaveLoad.SaveData[(int)SaveLoad.Line.HungerTime]);
        affectionLvlToSave = (int)SaveLoad.SaveData[(int)SaveLoad.Line.AffectionLevel];
        affectionTimeToSave = DateTime.FromBinary(SaveLoad.SaveData[(int)SaveLoad.Line.AffectionTime]);
        satisfiedLvlToSave = (int)SaveLoad.SaveData[(int)SaveLoad.Line.SatisfiedLevel];
        isIntroOver = (int) SaveLoad.SaveData[(int)SaveLoad.Line.IntroOver];
        aloeWatered = (int)SaveLoad.SaveData[(int)SaveLoad.Line.AloeWatered];
        aloeLevel = (int) SaveLoad.SaveData[(int)SaveLoad.Line.AloeLevel];
        aloeWateredTime = DateTime.FromBinary(SaveLoad.SaveData[(int)SaveLoad.Line.AloeWateredTime]);
        sapientLevel = (int)SaveLoad.SaveData[(int)SaveLoad.Line.SapientLevel];
        isAloeIntroOver = (int)SaveLoad.SaveData[(int)SaveLoad.Line.AloeIntroOver];

        Debug.Log("Game data loaded!");
        UpdateHungerLvl();
        UpdateAffectionLvl();
        UpdateSatisfiedLvl();

        SaveGame();

        /*
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/MySaveData.dat", FileMode.Open);
            // makes long array with zeros
            SaveLoad.SaveData = new long[SaveLoad.SAVEDATA_LENGHT];
            // populating array with correct values
            version = (int) SaveLoad.SaveData[SaveLoad.VERSION];
            hungerLvlToSave = (int) SaveLoad.SaveData[SaveLoad.HUNGER_LVL];
            hungerTimeToSave = DateTime.FromBinary(SaveLoad.SaveData[SaveLoad.HUNGER_TIME]);
            SaveLoad.SaveData[SaveLoad.AFFECTION_LVL] = affectionLvlToSave;
            affectionTimeToSave = DateTime.FromBinary(SaveLoad.SaveData[SaveLoad.AFFECTION_TIME]);
            SaveLoad.SaveData[SaveLoad.SATISFIED_LVL] = satisfiedLvlToSave;
            SaveLoad.SaveData[SaveLoad.INTRO_OVER] = isIntroOver;
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            hungerLvlToSave = data.savedHungerLvl;
            affectionLvlToSave = data.savedAffectionLvl;
            satisfiedLvlToSave = data.savedSatisfiedLvl;
            hungerTimeToSave = DateTime.FromBinary(data.savedHungerTime);
            affectionTimeToSave = DateTime.FromBinary(data.savedAffectionTime);
            isIntroOver = data.savedIsIntroOver;
            Debug.Log("Game data loaded!");
            UpdateHungerLvl();
            UpdateAffectionLvl();
            UpdateSatisfiedLvl();
            data.savedHungerLvl = hungerLvlToSave;
            data.savedAffectionTime = affectionTimeToSave.Ticks;
            data.savedHungerTime = hungerTimeToSave.Ticks;
            data.savedSatisfiedLvl = satisfiedLvlToSave;*/
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

    public void WaterAloe()
    {
        TimeSpan timeSpan = DateTime.UtcNow - Convert.ToDateTime(aloeWateredTime);

        if (timeSpan.Days > 11)
        {
            aloeWatered = 0;
            aloeLevel = 0;
        }
        else if (timeSpan.Days > 0)
        {
            aloeWatered = 0;
        }

        if (aloeWatered < 1 && aloeLevel < 3)
        {
            aloeWatered++;
            aloeLevel++;
            aloeWateredTime = DateTime.UtcNow;
        }

        Debug.LogWarning("Aloe watered: " + aloeWatered + " at: " + aloeWateredTime);
        SaveGame();
    }

    public void Read()
    {
        if (sapientLevel < 1)
        {
            sapientLevel++;
        }

        SaveGame();
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
                affectionRemainder = ((float) timeSpan.TotalDays - timeSpan.Days) * 24;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(affectionCounterReduction);
            }
             else if (timeSpan.Days == 0 & timeSpan.Hours < 0)
            {
                // Reducing affection stat.
                affectionLvlToSave += timeSpan.Hours / howManyHours;
                float affectionCounterReduction = (((float)timeSpan.TotalHours * 24 / howManyHours) * 3600) - timeSpan.Hours / howManyHours;
                affectionCounter -= affectionCounterReduction;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(affectionCounterReduction);
            }
            else
            {
                // Reducing affection stat.
                affectionLvlToSave -= timeSpan.Hours / howManyHours;
                float affectionCounterReduction = ((float)timeSpan.TotalHours - timeSpan.Hours) * 3600;
                affectionCounter -= affectionCounterReduction;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                Debug.Log("Affection remainder:" + affectionRemainder);
                Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(affectionCounterReduction);
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
                hungerRemainder = ((float)timeSpan.TotalDays - timeSpan.Days) * 24;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = hungerRemainder + hungerModulo * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(hungerCounterReduction);
            }
            else if (timeSpan.Days == 0 && timeSpan.Hours < 0)
            {
                hungerLvlToSave += timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing hunger counter.
                hungerRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = hungerRemainder + hungerModulo * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(hungerCounterReduction);
            }
            else
            {
                hungerLvlToSave -= timeSpan.Hours / howManyHours;
                Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing hunger counter.
                hungerRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                hungerModulo = timeSpan.Hours % howManyHours;
                hungerCounterReduction = hungerRemainder + hungerModulo * 3600;
                Debug.Log("Hunger remainder:" + hungerRemainder);
                Debug.Log("Modulo: " + hungerModulo);
                UpdateHungerCounter(hungerCounterReduction);
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

        SaveGame();
    }
    void UpdateAffectionCounter(float reduced)
    {
        affectionCounter -= reduced;
        Debug.Log("Affection counter reduced by: " + reduced);
    }
    void UpdateHungerCounter(float reduced)
    {
        hungerCounter -= reduced;
        Debug.Log("Hunger counter reduced by: " + reduced);
    }

    public void Exit()
    {
        isIntroOver = 1;
        SaveGame();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (Application.isEditor)
        {
            //isIntroOver = 1;
            SaveGame();
        }
    }
}
