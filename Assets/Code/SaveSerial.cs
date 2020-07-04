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

    // for satisfaction level
    int _levelValueCounter;
    long[] _levelValues = new long[SaveLoad.LevelLineCount];

    // for counters
    int _remainingHours, _remainingMinutes, _remainingSeconds;
    public float maxCounter, hungerCounter, affectionCounter;
    float timer = 0.0f;

    #region TODO and ???
    /* TODO: [] Save aloe stats.
     *       [] Change sprite after a day if aloe watered enough.
     *       [] 4 aloe levels. 0 being just got nibbled by kitsulope, 4 fully grown.
     *       [] WateredLevel just probably needs to be 0 or 1?
     *       [] A high max counter for waterLevel dropping (a day probably).
     *       [] Counter should work in the exact same way as it does with the actual pet.
    */
    public int aloeWatered = 0;
    public int aloeLevel = 0;
    DateTime aloeWateredTime;
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
    }

    public void Start()
    {
        maxCounter = howManyHours * 60 * 60;
        hungerCounter = maxCounter;
        affectionCounter = maxCounter;
    }

    void Update()
    {
        // dont run update on editor
        // it messed up default data XD
        if (Application.isEditor)
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
    }
    /*
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
    */
    void CreateOrEditSave(int start)
    {
        _levelValueCounter = 0;

        // sets all new data to default values
        // if start is zero sets all data to default
        for (int i = start; i < SaveLoad.SaveDataSize; i++)
        {
            // add hanling as needed :)
            if (i == (int)SaveLoad.Line.Version ||
                i == (int)SaveLoad.Line.IntroOver ||
                i == (int)SaveLoad.Line.Satisfaction)
                continue;

            if (((SaveLoad.Line)i).ToString().Contains("Level"))
            {
                SaveLoad.SaveData[i] = defaultLevelValue;
                // add it to levelValues array
                _levelValues[_levelValueCounter] = SaveLoad.SaveData[i];
                _levelValueCounter++;
                continue;
            }
            if (((SaveLoad.Line)i).ToString().Contains("Time"))
            {
                SaveLoad.SaveData[i] = DateTime.UtcNow.Ticks;
                continue;
            }

            Debug.LogError("Missing handling for " + ((SaveLoad.Line)i).ToString() + " and is set to zero!");
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

        if (Application.isEditor)
            return;

        //Debug.Log("Game data loaded!");
        CorrectHungerLevel();
        UpdateAffectionLvl();
        UpdateSatisfaction();
    }

    long UpdateLevelValue(long value, int valueToAdd)
    {
        if (value < levelMin) return levelMin;
        if (value >= levelMax) return levelMax;
        return value + valueToAdd;
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
        /*
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

        UpdateSatisfaction();
        */
    }

    void CorrectLevelValue(SaveLoad.Line time, SaveLoad.Line level)
    {
        // Link to documentation of TimeSpan
        // https://docs.microsoft.com/en-us/dotnet/api/system.timespan?view=netcore-3.1

        TimeSpan difference = 
            DateTime.UtcNow - Convert.ToDateTime(SaveLoad.SaveData[(int)time]);

        // every N hour is full number down
        SaveLoad.SaveData[(int)level] -= difference.Hours / howManyHours;
        // Updates levelValue between levelMin and levelMax
        UpdateLevelValue(SaveLoad.SaveData[(int)level], valueToAdd: 0);
        // remaining time to seconds with some steps :D
        _remainingHours = difference.Hours % howManyHours;
        _remainingMinutes = _remainingHours * 60 + difference.Minutes;
        _remainingSeconds = _remainingMinutes * 60 + difference.Seconds;
    }

    void UpdateAffectionLvl()
    {
        /*
        TimeSpan timeSpan = DateTime.UtcNow - Convert.ToDateTime(affectionTimeToSave);
        
        //Debug.Log("Time was: " + Convert.ToDateTime(affectionTimeToSave) + " and time is: " + DateTime.UtcNow);
        //Debug.Log("It's been this long since last petting: " + timeSpan);
        //Debug.Log("Affection level was: " + affectionLvlToSave);

        if (affectionLvlToSave > 0)
        {
            if (timeSpan.Days > 0)
            {
                // Reducing affection stat.
                daysReduction = (timeSpan.Days * 24) / howManyHours;
                //Debug.Log("What?! You've ignored the pet for over a day... :( Here's how much will be added to stacks lowering: " + daysReduction);
                //Debug.Log("Amount decreased should be: " + daysReduction);
                affectionLvlToSave -= daysReduction;

                // Reducing affection counter.
                affectionRemainder = ((float) timeSpan.TotalDays - timeSpan.Days) * 24;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                //Debug.Log("Affection remainder:" + affectionRemainder);
                //Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(affectionCounterReduction);
            }
             else if (timeSpan.Days == 0 & timeSpan.Hours < 0)
            {
                // Reducing affection stat.
                affectionLvlToSave += timeSpan.Hours / howManyHours;
                float affectionCounterReduction = (((float)timeSpan.TotalHours * 24 / howManyHours) * 3600) - timeSpan.Hours / howManyHours;
                affectionCounter -= affectionCounterReduction;
                //Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                //Debug.Log("Affection remainder:" + affectionRemainder);
                //Debug.Log("Modulo: " + affectionModulo);
                UpdateAffectionCounter(affectionCounterReduction);
            }
            else
            {
                // Reducing affection stat.
                affectionLvlToSave -= timeSpan.Hours / howManyHours;
                float affectionCounterReduction = ((float)timeSpan.TotalHours - timeSpan.Hours) * 3600;
                affectionCounter -= affectionCounterReduction;
                //Debug.Log("Amount decreased should be: " + timeSpan.Hours / howManyHours);

                // Reducing affection counter.
                affectionRemainder = ((float)timeSpan.TotalHours - timeSpan.Hours) * 60;
                affectionModulo = timeSpan.Hours % howManyHours;
                affectionCounterReduction = affectionRemainder + affectionModulo * 3600;
                //Debug.Log("Affection remainder:" + affectionRemainder);
                //Debug.Log("Modulo: " + affectionModulo);
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
        //Debug.Log("Affection level is: " + affectionLvlToSave);
        */
    }
    void CorrectHungerLevel()
    {
        // corrects hunger level when loading
        CorrectLevelValue(SaveLoad.Line.HungerTime, SaveLoad.Line.HungerLevel);
        // sets counter
        hungerCounter = maxCounter - _remainingSeconds;
    }

    void UpdateSatisfaction()
    {
        // sets satisfaction values to lowest level value
        foreach (int value in _levelValues)
        {
            if (value < SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction])
                SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] = value;
        }

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

        SaveGame();
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
        if (Application.isEditor) SaveGame();
        Debug.LogError("Exiting");
    }
}
