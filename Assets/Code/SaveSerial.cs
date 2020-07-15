﻿using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SaveSerial : MonoBehaviour
{
    // static variables
    public static SaveSerial SAVE;

    // uncategorized variables
    public bool isThisForJustMyTesting;
    public bool isPetting, isFeeding;
    public int howManyHours = 4;
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
            // cant do this in editor
            if (Application.isPlaying)
                DontDestroyOnLoad(SAVE);
        }
        #endregion

        // initializes long array
        if (SaveLoad.SaveData == null)
            SaveLoad.SaveData = new long[SaveLoad.SaveDataSize];

        LoadGame();
        Debug.Log("Sapient level is: " + sapientLevel);
    }

    public void Start()
    {
        maxCounter = howManyHours * 60 * 60;
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;

        if (Application.isPlaying)
        {
            if (FindObjectOfType<DecideStartScreen>())
                FindObjectOfType<DecideStartScreen>().LoadNextScene();
        }
    }

    void Update()
    {
        // only do update in playmode and builds
        if (Application.isPlaying == false)
            return;

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
            SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel]--;
            UpdateSatisfaction();
        }
        if (affectionCounter <= 0)
        {
            affectionCounter = maxCounter;
            //affectionLvlToSave--;
            UpdateSatisfaction();
        }

        // Check platform and if Back was pressed this frame
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
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
    
    void CreateOrEditSave(int start)
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
        CreateOrEditSave(0);
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
    }

    public void LoadGame()
    {
        int oldDataCount = 0;
        // if savegame exist load it
        if (SaveLoad.Find())
            oldDataCount = SaveLoad.Load(debugText);

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

        if (Application.isPlaying)
        {
            CorrectHungerLevel();
            UpdateAffectionLvl();
            UpdateSatisfaction();
        }
    }

    public void Feed()
    {
        isFeeding = true;

        SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel] =
            UpdateLevelValue(SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel], valueToAdd: 1);

        hungerCounter = maxCounter;
        SaveLoad.SaveData[(int)SaveLoad.Line.HungerTime] = DateTime.UtcNow.Ticks;
        //Debug.Log("Hunger level is: " + SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel]);

        UpdateSatisfaction();
    }

    public void Pet()
    {
        isPetting = true;

        SaveLoad.SaveData[(int)SaveLoad.Line.AffectionLevel] =
            UpdateLevelValue(SaveLoad.SaveData[(int)SaveLoad.Line.AffectionLevel], valueToAdd: 1);

        affectionCounter = maxCounter;
        SaveLoad.SaveData[(int)SaveLoad.Line.AffectionTime] = DateTime.UtcNow.Ticks;
        //Debug.Log("Hunger level is: " + SaveLoad.SaveData[(int)SaveLoad.Line.HungerLevel]);

        UpdateSatisfaction();
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
        // corrects level when loading
        CorrectLevelValue(SaveLoad.Line.AffectionTime, SaveLoad.Line.AffectionLevel);
        // sets counter
        affectionCounter = maxCounter - _remainingSeconds;
    }

    void CorrectHungerLevel()
    {
        // corrects level when loading
        CorrectLevelValue(SaveLoad.Line.HungerTime, SaveLoad.Line.HungerLevel);
        // sets counter
        hungerCounter = maxCounter - _remainingSeconds;
    }

    void UpdateSatisfaction()
    {
        // set satisfaction to max instead of zero
        // before it checked is 3 less than 0 XD
        SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] = levelMax;

        LoadLevelValues();

        // sets satisfaction values to lowest level value
        foreach (int value in _levelValues)
        {
            if (value < SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction])
                SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] = value;
        }

        {
            if (SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] == 3)
            {
                //Debug.Log("Your fluffy overlord is mostly satisfied, keep it up!");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] == 2)
            {
                //Debug.Log("Your fluffy overlord is okay...");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] == 1)
            {
                //Debug.Log("Your fluffy overlord is getting impatient...");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] == 0)
            {
                //Debug.Log("Your fluffy overlord says fuck off!");
            }
        }

        SaveGame();
    }

    void LoadLevelValues()
    {
        _levelValueCounter = 0;

        for (int i = 0; i < SaveLoad.SaveDataSize; i++)
        {
            if (((SaveLoad.Line)i).ToString().Contains("Level"))
            {
                _levelValues[_levelValueCounter] = SaveLoad.SaveData[i];
                _levelValueCounter++;
                continue;
            }
        }
    }

    long UpdateLevelValue(long value, int valueToAdd)
    {
        if (value < levelMin) return levelMin;
        if (value >= levelMax) return levelMax;
        return value + valueToAdd;
    }

    public void Exit()
    {
        if (SceneManager.GetActiveScene().name.Contains("Game"))
            SaveLoad.SaveData[(int)SaveLoad.Line.IntroOver] = 1;

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
