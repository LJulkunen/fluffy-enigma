using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    // all saved data is turned into longs (64bit int)
    public static long[] SaveData { get; set; }
    // SaveData lenght
    public const int SAVEDATA_LENGHT = 7;

    // indexes of long array
    public const int VERSION = 0;           // you can check if version is different and add/remove/change data etc.
    public const int HUNGER_LVL = 1;
    public const int HUNGER_TIME = 2;
    public const int AFFECTION_LVL = 3;
    public const int AFFECTION_TIME = 4;
    public const int SATISFIED_LVL = 5;
    public const int INTRO_OVER = 6;

    // boolean values
    public const int FALSE = 0;
    public const int TRUE = 1;

    // filepath
    public const string FILE_PATH = "/KitsulopeData.kd";

    public static bool FindSaveFile()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
        {
            return true;
        }

        return false;
    }

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + FILE_PATH);
        bf.Serialize(file, SaveData);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + FILE_PATH, FileMode.Open);
            SaveData = (long[])bf.Deserialize(file);
            file.Close();
        }
    }
    
    public static void Delete()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
        {
            File.Delete(Application.persistentDataPath + FILE_PATH);
        }
    }
}
