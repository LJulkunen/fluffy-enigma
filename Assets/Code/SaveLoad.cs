using UnityEngine;
using System.IO;
using System;

public static class SaveLoad
{
    // all saved data is turned into longs (64bit int)
    public static long[] SaveData { get; set; }
    // SaveData lenght
    public static int SaveDataSize
    {
        get { return (int)Line.LineCount; }
    }

    public enum Line
    {
        Version,            // 0
        K_HungerLevel,      // 1
        K_HungerTime,       // 2
        K_AffectionLevel,   // ...
        K_AffectionTime,
        K_Satisfaction,
        K_IntroOver,
        A_AloeWateredLevel,
        A_AloeLevel,
        A_AloeWateredTime,
        SapientLevel,
        A_IntroOver,
        LineCount           // Always last !!!!
    }
    
    // boolean values
    public const int FALSE = 0;
    public const int TRUE = 1;

    // filepath
    public const string FILE_PATH = "/KitsulopeData.kd";

    public static bool Find()
    {
        return File.Exists(Application.persistentDataPath + FILE_PATH);
    }

    public static void Save(TMPro.TextMeshProUGUI debugText)
    {
        // making string for saving
        string data = "";
        for (int i = 0; i < SaveData.Length; i++)
        {
            // adding only number value and newline
            data += SaveData[i].ToString() + Environment.NewLine;
        }

        // write data to file
        File.WriteAllText(Application.persistentDataPath + FILE_PATH, data, System.Text.Encoding.UTF8);

        #region debug
        data = "Saved:" + Environment.NewLine;
        for (int i = 0; i < SaveData.Length; i++)
        {
            data += ((Line)i).ToString() + " : " + SaveData[i].ToString() + Environment.NewLine;
        }
        if (debugText) debugText.text = data;
        #endregion
    }

    public static int Load(TMPro.TextMeshProUGUI debugText)
    {
        // converting textlines to longs
        string[] lines = File.ReadAllLines(Application.persistentDataPath + FILE_PATH, System.Text.Encoding.UTF8);
        for (int i = 0; i < lines.Length; i++)
        {
            SaveData[i] = TextToLong(lines[i]);
        }

        #region debug
        string data = "Loaded:" + Environment.NewLine;
        for (int i = 0; i < SaveData.Length; i++)
        {
            data += ((Line)i).ToString() + " : " + SaveData[i].ToString() + Environment.NewLine;
        }
        if (debugText) debugText.text = data;
        #endregion

        return lines.Length;
    }

    public static void Delete()
    {
        if (Find())
        {
            File.Delete(Application.persistentDataPath + FILE_PATH);
        }
    }

    static long TextToLong(string text)
    {
        long result = 0;
        char[] numbers = text.ToCharArray();

        for (int i = 0; i < numbers.Length; i++)
        {
            result *= 10;
            // number zero is value 30(hex) 48(dec) in unicode and ascii
            // meaning char '0' = int 48
            result += numbers[i] - 48;
        }

        return result;
    }
}
