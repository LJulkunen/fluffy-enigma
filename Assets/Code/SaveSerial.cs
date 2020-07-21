using UnityEngine;
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

    // generic default values
    public int defaultLevelValue = 3, levelMin = 0, levelMax = 4;

    // for counters
    int _remainingHours, _remainingMinutes, _remainingSeconds;
    public float maxCounter, hungerCounter, affectionCounter;
    float timer = 0.0f;

    #region TODO and ???
    /* TODO: [x] Save aloe stats.
     *       [] Change sprite after a day if aloe watered enough.
     *       [] 4 aloe levels. 0 being just got nibbled by kitsulope, 4 fully grown.
     *       [x] WateredLevel just probably needs to be 0 or 1?
     *       [x] A high max counter for waterLevel dropping (a day probably).
     *       [] Counter should work in the exact same way as it does with the actual pet.
     */
    public int aloeWatered = 0;
    public int aloeLevel = 0;

    public int maxSapientLevel = 7;
    public int sapientLevel = 0;

    public int isAloeIntroOver = 0;
    #endregion

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
            SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerLevel]--;
            UpdateSatisfaction();
        }

        if (affectionCounter <= 0)
        {
            affectionCounter = maxCounter;
            SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionLevel]--;
            UpdateSatisfaction();
        }

        // Check platform and if Back (or esc) was pressed this frame
        if (Application.isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
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
        // sets all new data to default values
        // if start is zero sets all data to default
        for (int i = start; i < SaveLoad.SaveDataSize; i++)
        {
            // set some values to 3
            if ((int)SaveLoad.Line.K_HungerLevel == i ||
                (int)SaveLoad.Line.K_AffectionLevel == i)
            {
                SaveLoad.SaveData[i] = defaultLevelValue;
                continue;
            }

            // set some values to utc now
            if ((int)SaveLoad.Line.K_HungerTime == i ||
                (int)SaveLoad.Line.K_AffectionTime == i)
            {
                SaveLoad.SaveData[i] = DateTime.UtcNow.Ticks;
                continue;
            }

            Debug.LogWarning("Missing handling for " + ((SaveLoad.Line)i).ToString() + " and is set to zero!");
            SaveLoad.SaveData[i] = 0;
        }
        
        SaveGame();
    }

    public void ResetSave()
    {
        SaveLoad.Delete();
        CreateOrEditSave(0);
    }

    public void SaveGame()
    {
        // saving correct values
        SaveLoad.Save(debugText);
    }

    public void LoadGame()
    {
        int oldDataCount = 0;
        // if savegame exist load it
        if (SaveLoad.Find())
            oldDataCount = SaveLoad.Load(debugText);

        CreateOrEditSave(oldDataCount);

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

        SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerLevel] =
            UpdateLevelValue(SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerLevel], valueToAdd: 1);

        hungerCounter = maxCounter;
        SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerTime] = DateTime.UtcNow.Ticks;

        UpdateSatisfaction();
    }

    public void Pet()
    {
        isPetting = true;

        SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionLevel] =
            UpdateLevelValue(SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionLevel], valueToAdd: 1);

        affectionCounter = maxCounter;
        SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionTime] = DateTime.UtcNow.Ticks;

        UpdateSatisfaction();
    }

    void CorrectLevelValue(SaveLoad.Line time, SaveLoad.Line level)
    {
        // Link to documentation of TimeSpan
        // https://docs.microsoft.com/en-us/dotnet/api/system.timespan?view=netcore-3.1

        TimeSpan difference = 
            DateTime.UtcNow - new DateTime(SaveLoad.SaveData[(int)time]);

        // every N hour is full number down
        SaveLoad.SaveData[(int)level] -= difference.Hours / howManyHours;
        // Updates levelValue between levelMin and levelMax
        UpdateLevelValue(SaveLoad.SaveData[(int)level], valueToAdd: 0);
        // remaining time to seconds with some steps :D
        _remainingHours = difference.Hours % howManyHours;
        _remainingMinutes = _remainingHours * 60 + difference.Minutes;
        _remainingSeconds = _remainingMinutes * 60 + difference.Seconds;
    }

    public void WaterAloe()
    {
        // V TODO V
        //isWatering = true;

        // V TODO V
        //SaveLoad.SaveData[(int)SaveLoad.Line.A_AloeWateredLevel] =
        //    UpdateAloeLevelValue(SaveLoad.SaveData[(int)SaveLoad.Line.A_AloeWateredLevel], valueToAdd: 1);

        // V TODO V
        //aloeWateredCounter = maxCounter;
        SaveLoad.SaveData[(int)SaveLoad.Line.A_AloeWateredTime] = DateTime.UtcNow.Ticks;

        // V TODO V
        //UpdateAloeSatisfaction();

        // old stuff
        {
            /*
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
            */
        }
    }

    public void Read()
    {
        if (SaveLoad.SaveData[(int)SaveLoad.Line.SapientLevel] < 1)
        {
            SaveLoad.SaveData[(int)SaveLoad.Line.SapientLevel]++;
        }

        SaveGame();
    }

    void UpdateAffectionLvl()
    {
        // corrects level when loading
        CorrectLevelValue(SaveLoad.Line.K_AffectionTime, SaveLoad.Line.K_AffectionLevel);
        // sets counter
        affectionCounter = maxCounter - _remainingSeconds;
    }

    void CorrectHungerLevel()
    {
        // corrects level when loading
        CorrectLevelValue(SaveLoad.Line.K_HungerTime, SaveLoad.Line.K_HungerLevel);
        // sets counter
        hungerCounter = maxCounter - _remainingSeconds;
    }

    void UpdateSatisfaction()
    {
        if (SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerLevel] < SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionLevel])
        {
            SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] = SaveLoad.SaveData[(int)SaveLoad.Line.K_HungerLevel];
        }
        else
        {
            SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] = SaveLoad.SaveData[(int)SaveLoad.Line.K_AffectionLevel];
        }

        {
            if (SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] == 3)
            {
                //Debug.Log("Your fluffy overlord is mostly satisfied, keep it up!");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] == 2)
            {
                //Debug.Log("Your fluffy overlord is okay...");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] == 1)
            {
                //Debug.Log("Your fluffy overlord is getting impatient...");
            }
            else if (SaveLoad.SaveData[(int)SaveLoad.Line.K_Satisfaction] == 0)
            {
                //Debug.Log("Your fluffy overlord says fuck off!");
            }
        }

        SaveGame();
    }

    long UpdateLevelValue(long value, int valueToAdd)
    {
        if (value < levelMin) return levelMin;
        if (value >= levelMax) return levelMax;
        return value + valueToAdd;
    }

    public void Exit()
    {
        SaveGame();
        Application.Quit();
    }
}
