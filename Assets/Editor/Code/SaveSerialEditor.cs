using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveSerial))]
public class SaveSerialEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (SaveSerial.SAVE == null)
        {
            Debug.LogError("No SaveSerial instanses! Please reload scene!");
            return;
        }

        if (SaveLoad.SaveData == null)
            SaveSerial.SAVE.LoadGame();

        SaveSerial.SAVE.Start();

        EditorGUILayout.SelectableLabel(
            "Read only savedata!!! To add new savedata line edit:" + Environment.NewLine +
            "Savedata.Line but keep LineCount as last enum element!");

        for (int i = 0; i < SaveLoad.SaveDataSize; i++)
        {
            // Normal editor box with name on left and variable on right
            EditorGUILayout.LongField(((SaveLoad.Line)i).ToString(), SaveLoad.SaveData[i]);
        }
    }
}
