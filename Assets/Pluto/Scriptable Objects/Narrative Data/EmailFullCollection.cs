using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Based on bb's CSV parsing files, repurposed into a tool by Perell

[System.Serializable]
public struct EmailContent
{
    public EmailContent(string sender, string subject, string message)
    {
        mSender = sender;
        mSubject = subject;
        mMessage = message;
    }

    public string mSender;
    public string mSubject;
    public string mMessage;
}

[System.Serializable]
public class FakeSortedDictionary
{
    [System.Serializable]
    public struct KeyValuePair<TTKey, TTValue>
    {
        public KeyValuePair(TTKey key, TTValue value)
        {
            Key = key;
            Value = value;
        }

        public TTKey Key;
        public TTValue Value;
    }

    [SerializeField]
    List<KeyValuePair<string, EmailContent>> list;

    public FakeSortedDictionary()
    {
        list = new List<KeyValuePair<string, EmailContent>>();
    }

    public void Add(string key, EmailContent value)
    {
        if (list.Count > 0)
        {
            int index = 0;
            while (index < list.Count && list[index].Key.CompareTo(key) < 0)
            {
                index++;
            }
            list.Insert(index, new KeyValuePair<string, EmailContent>(key, value));
        }
        else
        {
            list.Add(new KeyValuePair<string, EmailContent>(key, value));
        }
    }

    public bool ContainsKey(string key)
    {
        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int index = min + (max - min) / 2;
            if (list[index].Key == key)
            {
                return true;
            }
            else if (list[index].Key.CompareTo(key) < 0)
            {
                min = index + 1;
            }
            else
            {
                max = index - 1;
            }
        }

        return false;
    }

    public EmailContent this[string id]
    {
        get 
        {
            int min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int index = min + (max - min) / 2;
                if (list[index].Key == id)
                {
                    return list[index].Value;
                }
                else if (list[index].Key.CompareTo(id) < 0)
                {
                    min = index + 1;
                }
                else
                {
                    max = index - 1;
                }
            }

            return new EmailContent();
        }

        set 
        {

        }
    }
}

[CreateAssetMenu(fileName = "New Email Full Collection", menuName = "Narrative/Data Generators/Email Full Collection")]
public class EmailFullCollection : ScriptableObject
{
    [SerializeField]
    TextAsset csvFile;
    //[SerializeField]
    //SortedDictionary<string,EmailContent> database;
    [SerializeField]
    FakeSortedDictionary database;

    const string backingPath = "/ScriptableObjects/Narrative/Narrative Data/Emails/";

    [ContextMenu("Parse EmailDataSet")]
    void Parse()
    {
        database = new FakeSortedDictionary();
        fgCSVReader.LoadFromString(csvFile.text, AddToDatabase);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    void AddToDatabase(Line currentLine)
    {
        if(currentLine.cell[0] != null && currentLine.cell[1] != null && currentLine.cell[2] != null && currentLine.cell[3] != null)
        database.Add(currentLine.cell[0].value, new EmailContent(currentLine.cell[1].value, currentLine.cell[2].value, currentLine.cell[3].value));

        string assetName = currentLine.cell[0].value + ".asset";
        string assetContents = "%YAML 1.1" +
            '\n' + "%TAG !u! tag:unity3d.com,2011:" +
            '\n' + "--- !u!114 &11400000" +
            '\n' + "MonoBehaviour:" +
            '\n' + "  m_ObjectHideFlags: 0" +
            '\n' + "  m_CorrespondingSourceObject: {fileID: 0}" +
            '\n' + "  m_PrefabInstance: {fileID: 0}" +
            '\n' + "  m_PrefabAsset: {fileID: 0}" +
            '\n' + "  m_GameObject: {fileID: 0}" +
            '\n' + "  m_Enabled: 1" +
            '\n' + "  m_EditorHideFlags: 0" +
            '\n' + "  m_Script: {fileID: 11500000, guid: 04ab86da48d252c46baf4c766e2090c6, type: 3}" +
            '\n' + "  m_Name: " + currentLine.cell[0].value +
            '\n' + "  m_EditorClassIdentifier:" +
            '\n' + "  id: " + currentLine.cell[0].value +
            '\n' + "  deliveryType: 0" +
            '\n' + "  priorityValue: " + int.Parse(currentLine.cell[4].value) +
            '\n' + "  flagsToMeet:";
        if(currentLine.cell[5].value != "")
        foreach (string e in currentLine.cell[5].value.Split(','))
        {
            assetContents += '\n' + "  - " + e;
        }
        assetContents += '\n' + "  flagsToAvoid:";
        if (currentLine.cell[6] != null && currentLine.cell[6].value != "")
        {
            foreach (string e in currentLine.cell[6].value.Split(','))
            {
                assetContents += '\n' + "  - " + e;
            }
        }
        assetContents += '\n';

        string path = Application.dataPath + backingPath + assetName;

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using (FileStream fs = File.Create(path))
        using (var writer = new StreamWriter(fs))
        {
            writer.Write(assetContents);
        }
    }

    public void DeleteAllFiles()
    {
        
    }

    public bool FindEmailData(string id, out EmailContent email)
    {
        email = new EmailContent();
        bool success = database.ContainsKey(id);
        if (success)
        {
            email = database[id];
        }
        return success;
    }
}
