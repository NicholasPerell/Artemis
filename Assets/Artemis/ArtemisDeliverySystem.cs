using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ArtemisPreDictionarySystem : ScriptableObject
{
    public abstract bool Send();

#if UNITY_EDITOR
    public abstract void DeliverySystemDatabase();
#endif

}

public abstract class ArtemisDeliverySystem<T> : ArtemisPreDictionarySystem
{
    [Header("Database Loading")]
    [SerializeField]
    private TextAsset csvFile;
    [SerializeField]
    [Tooltip("Number of columns in the CSV used to generate the data structures in each database. Number does not include the base 5 columns.")]
    protected int columnsToReadFrom;
    [SerializeField]
    private ArtemisSortingDictionaries<T> database;

    private const int BASE_COLUMNS = 5;

    private List<string> notBeingUsed;

#if UNITY_EDITOR
    public override void DeliverySystemDatabase()
    {
        //List used to track what IDs need to deleted
        notBeingUsed = new List<string>();
        if (database != null)
        {
            notBeingUsed = database.GetKeyList();
        }

        //Reset databases
        database = new ArtemisSortingDictionaries<T>();

        //Check for folder
        if(!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetDataPointFolderName()))
        {
            AssetDatabase.CreateFolder(GetContainingFolder(), GetDataPointFolderName());
        }

        //Parse CSV
        fgCSVReader.LoadFromString(csvFile.text, BASE_COLUMNS + columnsToReadFrom, AddToDatabase);

        string tmp;
        foreach(string e in notBeingUsed)
        {
            tmp = GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + e + ".asset";
            if (AssetDatabase.LoadAssetAtPath<ArtemisNarrativeDataPoint>(tmp) != null)
            {
                AssetDatabase.DeleteAsset(tmp);
            }
        }

        EditorUtility.SetDirty(this);
    }

    private void AddToDatabase(Line currentLine)
    {
        bool invalid = false;

        //Datapoints must have an ID
        if (currentLine.cell[0] == null || currentLine.cell[0].value == "" || currentLine.cell[0].value == "END")
        {
            if (currentLine.cell[0].value != "END")
            {
                Debug.LogError("ID was not found");
            }
            invalid = true;
        }

        //Data intake must be validated
        T data = default(T);
        if (!invalid)
        {
            string[] stringsToInterpret = new string[columnsToReadFrom];
            
            for(int i = 0; i < columnsToReadFrom; i++)
            {
                if(currentLine.cell[BASE_COLUMNS + i] != null)
                {
                    stringsToInterpret[i] = currentLine.cell[BASE_COLUMNS + i].value;
                }
                else
                {
                    stringsToInterpret[i] = "";
                }
            }

            if (!SetUpDataFromCells(stringsToInterpret, out data))
            {
                Debug.LogError(data.GetType() + " for " + currentLine.cell[0].value + " was not loaded correctly!");
                invalid = true;
            }
        }

        //Valid!!!!
        if (!invalid)
        {
            //1) Add to the official database
            database.Add(currentLine.cell[0].value, data);

            //2) Add/update asset
            string _id = currentLine.cell[0].value;
            ArtemisPreDictionarySystem _systemScriptable = this; 
            int _priorityValue;
            if (currentLine.cell[1] == null || !int.TryParse(currentLine.cell[1].value, out _priorityValue))
            {
                _priorityValue = 0;
            }

            string[] _flagsToMeet = new string[0];
            if (currentLine.cell[2] != null && currentLine.cell[2].value != "")
            {
                _flagsToMeet = currentLine.cell[2].value.Split(',');
            }

            string[] _flagsToAvoid = new string[0];
            if (currentLine.cell[3] != null && currentLine.cell[3].value != "")
            {
                _flagsToAvoid = currentLine.cell[3].value.Split(',');
            }

            ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying _whenAlreadyVoicePlaying;
            if (currentLine.cell[4] != null)
            {
                switch (currentLine.cell[4].value)
                {
                    case "DELETE":
                        _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.DELETE;
                        break;
                    case "FRONT_OF_QUEUE":
                        _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.FRONT_OF_QUEUE;
                        break;
                    case "INTERRUPT":
                        _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.INTERRUPT;
                        break;
                    case "QUEUE":
                        _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.QUEUE;
                        break;
                    case "CANCEL":
                    default:
                        _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.CANCEL;
                        break;
                }
            }
            else
            {
                _whenAlreadyVoicePlaying = ArtemisNarrativeDataPoint.WhenAlreadyVoicePlaying.CANCEL;
            }

            ArtemisNarrativeDataPoint dataPoint = AssetDatabase.LoadAssetAtPath<ArtemisNarrativeDataPoint>(GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + _id + ".asset");

            bool exists = dataPoint != null;

            if(!exists)
            {
                dataPoint = ScriptableObject.CreateInstance<ArtemisNarrativeDataPoint>();
            }

            dataPoint.Rewrite(_id, _systemScriptable, _priorityValue, _flagsToMeet, _flagsToAvoid, _whenAlreadyVoicePlaying);

            if (exists)
            {
                EditorUtility.SetDirty(dataPoint);
            }
            else
            {
                AssetDatabase.CreateAsset(dataPoint,GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + _id + ".asset");
            }

            //3) remove from list of uninvolved Assets for clean up later
            notBeingUsed.Remove(_id);
        }
    }
#endif

    public bool FindData(string id, out T value)
    {
        value = default(T);
        bool success = database.ContainsKey(id);
        if (success)
        {
            value = database[id];
        }
        return success;
    }

    private string GetContainingFolder()
    {
        string rtn = AssetDatabase.GetAssetPath(this);
        rtn = rtn.Substring(0, rtn.LastIndexOf('/'));
        return rtn;
    }

    private string GetDataPointFolderName()
    {
        return name + " Data Points";
    }
    
    protected abstract bool SetUpDataFromCells(string[] dataToInterpret, out T valueDetermined);
}
