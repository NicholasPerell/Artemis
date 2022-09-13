using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Artemis
{
    public abstract class PreDictionaryFletcher : ScriptableObject
    {
        List<KeyValuePair<Arrow, Archer>> queue;

        private void Awake()
        {
            queue = new List<KeyValuePair<Arrow, Archer>>();
        }

        public bool ProcessDataPoint(Arrow dataPoint, Archer sender)
        {
            bool successfullyProcessed = false;

            Arrow.HowToHandleBusy decision = dataPoint.GetWhenBusyDescision();

            if (IsBusy())
            {
                if (queue == null)
                {
                    queue = new List<KeyValuePair<Arrow, Archer>>();
                }

                KeyValuePair<Arrow, Archer> storedPairing = new KeyValuePair<Arrow, Archer>(dataPoint, sender);

                switch (decision)
                {
                    case Arrow.HowToHandleBusy.CANCEL:
                        successfullyProcessed = false;
                        break;
                    case Arrow.HowToHandleBusy.QUEUE:
                        queue.Add(storedPairing);
                        successfullyProcessed = true;
                        break;
                    case Arrow.HowToHandleBusy.INTERRUPT:
                        AbruptEnd();
                        Send(dataPoint.name);
                        successfullyProcessed = true;
                        break;
                    case Arrow.HowToHandleBusy.DELETE:
                        successfullyProcessed = true;
                        break;
                    case Arrow.HowToHandleBusy.FRONT_OF_QUEUE:
                        queue.Insert(0, storedPairing);
                        successfullyProcessed = true;
                        break;
                    default:
                        Debug.LogError(dataPoint.name + " has invalid whenAlreadyVoicePlaying value.");
                        successfullyProcessed = false;
                        break;
                }
            }
            else
            {
                Send(dataPoint.name);
                successfullyProcessed = true;
            }

            return successfullyProcessed;
        }

        public void ProcessEnd()
        {
            if (queue.Count > 0)
            {
                KeyValuePair<Arrow, Archer> pair = queue[0];
                queue.RemoveAt(0);
                if (pair.Key.CondtionsMet())
                {
                    Send(pair.Key.name);
                }
                else
                {
                    pair.Value.RecieveDataPoint(pair.Key);
                    ProcessEnd();
                }
            }
        }

        protected abstract void Send(string id);
        protected abstract bool IsBusy();
        protected abstract void AbruptEnd();

#if UNITY_EDITOR
        public abstract void DeliverySystemDatabase();
#endif
    }

    public abstract class Fletcher<T> : PreDictionaryFletcher
    {
        [Header("Database Loading")]
        [SerializeField]
        private TextAsset csvFile;
        [SerializeField]
        [Tooltip("Number of columns in the CSV used to generate the data structures in each database. Number does not include the base 5 columns.")]
        protected int columnsToReadFrom;
        [SerializeField]
        private StringSortingDictionary<T> database;

        private const int BASE_COLUMNS = 5;

        [HideInInspector]
        private List<string> flagsBeingUsed;
        [HideInInspector]
        private List<string> notBeingUsed;
        [HideInInspector]
        private List<string> flagsNoLongerBeingUsed;

        Bow<T> inSceneObject;

#if UNITY_EDITOR
        public override void DeliverySystemDatabase()
        {
            //List used to track what IDs need to deleted
            notBeingUsed = new List<string>();
            if (database != null)
            {
                notBeingUsed = database.GetKeyList();
            }
            flagsBeingUsed ??= new List<string>();
            flagsNoLongerBeingUsed = new List<string>();
            foreach (string e in flagsBeingUsed)
            {
                flagsNoLongerBeingUsed.Add(e);
            }
            flagsBeingUsed.Clear();

            //Reset databases
            database = new StringSortingDictionary<T>();

            //Check for folder
            if (!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetDataPointFolderName()))
            {
                AssetDatabase.CreateFolder(GetContainingFolder(), GetDataPointFolderName());
            }

            //Parse CSV
            fgCSVReader.LoadFromString(csvFile.text, BASE_COLUMNS + columnsToReadFrom, AddToDatabase);

            string tmp;
            foreach (string e in notBeingUsed)
            {
                tmp = GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + e + ".asset";
                if (AssetDatabase.LoadAssetAtPath<Arrow>(tmp) != null)
                {
                    AssetDatabase.DeleteAsset(tmp);
                }
            }

            foreach (string e in flagsNoLongerBeingUsed)
            {
                Goddess.instance.DisconnectFlag(e, this);
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

                for (int i = 0; i < columnsToReadFrom; i++)
                {
                    if (currentLine.cell[BASE_COLUMNS + i] != null)
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
                PreDictionaryFletcher _systemScriptable = this;
                int _priorityValue;
                if (currentLine.cell[1] == null || !int.TryParse(currentLine.cell[1].value, out _priorityValue))
                {
                    _priorityValue = 0;
                }

                Flag[] _flagsToMeet = new Flag[0];
                if (currentLine.cell[2] != null && currentLine.cell[2].value != "")
                {
                    string[] names = currentLine.cell[2].value.Split(',');
                    _flagsToMeet = new Flag[names.Length];
                    for (int i = 0; i < _flagsToMeet.Length; i++)
                    {
                        _flagsToMeet[i] = Goddess.instance.ConnectFlag(names[i], this);
                        flagsNoLongerBeingUsed.Remove(names[i]);
                        if (!flagsBeingUsed.Contains(names[i]))
                        {
                            flagsBeingUsed.Add(names[i]);
                        }
                    }
                }

                Flag[] _flagsToAvoid = new Flag[0];
                if (currentLine.cell[3] != null && currentLine.cell[3].value != "")
                {
                    string[] names = currentLine.cell[3].value.Split(',');
                    _flagsToAvoid = new Flag[names.Length];
                    for (int i = 0; i < _flagsToMeet.Length; i++)
                    {
                        _flagsToAvoid[i] = Goddess.instance.ConnectFlag(names[i], this);
                        flagsNoLongerBeingUsed.Remove(names[i]);
                        if (!flagsBeingUsed.Contains(names[i]))
                        {
                            flagsBeingUsed.Add(names[i]);
                        }
                    }
                }

                Arrow.HowToHandleBusy _howToHandleBusy;
                if (currentLine.cell[4] != null)
                {
                    switch (currentLine.cell[4].value)
                    {
                        case "DELETE":
                            _howToHandleBusy = Arrow.HowToHandleBusy.DELETE;
                            break;
                        case "FRONT_OF_QUEUE":
                            _howToHandleBusy = Arrow.HowToHandleBusy.FRONT_OF_QUEUE;
                            break;
                        case "INTERRUPT":
                            _howToHandleBusy = Arrow.HowToHandleBusy.INTERRUPT;
                            break;
                        case "QUEUE":
                            _howToHandleBusy = Arrow.HowToHandleBusy.QUEUE;
                            break;
                        case "CANCEL":
                        default:
                            _howToHandleBusy = Arrow.HowToHandleBusy.CANCEL;
                            break;
                    }
                }
                else
                {
                    _howToHandleBusy = Arrow.HowToHandleBusy.CANCEL;
                }

                Arrow dataPoint = AssetDatabase.LoadAssetAtPath<Arrow>(GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + _id + ".asset");

                bool exists = dataPoint != null;

                if (!exists)
                {
                    dataPoint = ScriptableObject.CreateInstance<Arrow>();
                }

                dataPoint.Rewrite(_id, _systemScriptable, _priorityValue, _flagsToMeet, _flagsToAvoid, _howToHandleBusy);

                if (exists)
                {
                    EditorUtility.SetDirty(dataPoint);
                }
                else
                {
                    AssetDatabase.CreateAsset(dataPoint, GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + _id + ".asset");
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

#if UNITY_EDITOR
        private string GetContainingFolder()
        {
            string rtn = AssetDatabase.GetAssetPath(this);
            rtn = rtn.Substring(0, rtn.LastIndexOf('/'));
            return rtn;
        }
#endif

        private string GetDataPointFolderName()
        {
            return name + " Data Points";
        }

        public void SetInSceneObject(Bow<T> _value)
        {
            inSceneObject = _value;
        }

        public Bow<T> GetInSceneObject()
        {
            return inSceneObject;
        }

        protected override void Send(string id)
        {
            T value;
            if (FindData(id, out value))
            {
                inSceneObject.Send(value);
            }
        }

        protected override bool IsBusy()
        {
            return inSceneObject.IsBusy();
        }

        protected override void AbruptEnd()
        {
            inSceneObject.AbruptEnd();
        }

        protected abstract bool SetUpDataFromCells(string[] dataToInterpret, out T valueDetermined);
    }
}
