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

        private List<KeyValuePair<Arrow, Archer>> queue;
        public const string CRITERIA_KEY_WORD = "COND";

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
                        Send(dataPoint.GetArrowID());
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
                Send(dataPoint.GetArrowID());
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
                    Send(pair.Key.GetArrowID());
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

        private const int BASE_COLUMNS = 4;

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
            //List used to track what IDs need to be deleted
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
            if (!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetArrowFolderName()))
            {
                AssetDatabase.CreateFolder(GetContainingFolder(), GetArrowFolderName());
            }

            //Parse CSV
            fgCSVReader.LoadFromString(csvFile.text, BASE_COLUMNS + columnsToReadFrom, AddToDatabase);

            string tmp;
            foreach (string e in notBeingUsed)
            {
                tmp = GetContainingFolder() + "/" + GetArrowFolderName() + "/" + e + ".asset";
                if (AssetDatabase.LoadAssetAtPath<Arrow>(tmp) != null)
                {
                    AssetDatabase.DeleteAsset(tmp);
                }
            }

            foreach (string e in flagsNoLongerBeingUsed)
            {
                Goddess.instance.DisconnectFlag(e, this);
            }

            Goddess.instance.WriteFlagEnumScript();

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

            //Flag checks must be valid
            SortedStrictDictionary<FlagID, Criterion> _rule = null;
            if(!invalid)
            {
                string flagColumnString = "";
                if(currentLine.cell[2] != null)
                {
                    flagColumnString = currentLine.cell[2].value;
                }

                invalid = !TryEvalFlagList(flagColumnString, out _rule);
            }

            //Valid!!!!
            if (!invalid)
            {
                //1) Add to the official database
                database.Add(currentLine.cell[0].value, data);

                //2) Add/update asset
                string _id = currentLine.cell[0].value;
                PreDictionaryFletcher _systemScriptable = this;
                int _priorityValue = 0;
                Arrow.HowPriorityCalculated _howPriorityCalculated = Arrow.HowPriorityCalculated.SET_VALUE;
                if (currentLine.cell[1] != null)
                {
                    string priorityValueInput = currentLine.cell[1].value.Trim();
                    int indexOf = priorityValueInput.IndexOf(CRITERIA_KEY_WORD);
                    if (indexOf != -1)
                    {
                        if (priorityValueInput.Length == CRITERIA_KEY_WORD.Length)
                        {
                            _howPriorityCalculated = Arrow.HowPriorityCalculated.CRITERIA;
                        }
                        else
                        {
                            indexOf = priorityValueInput.Substring(0, indexOf).LastIndexOf("+");
                            if (int.TryParse(priorityValueInput.Substring(0, indexOf), out _priorityValue))
                            {
                                _howPriorityCalculated = Arrow.HowPriorityCalculated.SUM;
                            }
                            else
                            {
                                _howPriorityCalculated = Arrow.HowPriorityCalculated.CRITERIA;
                            }
                        }
                    }
                    else
                    {
                        int.TryParse(priorityValueInput, out _priorityValue);
                    }
                    
                }

                Arrow.HowToHandleBusy _howToHandleBusy;
                if (currentLine.cell[3] != null)
                {
                    if (!Enum.TryParse<Arrow.HowToHandleBusy>(currentLine.cell[3].value, out _howToHandleBusy))
                    {
                        _howToHandleBusy = Arrow.HowToHandleBusy.CANCEL;
                    }
                }
                else
                {
                    _howToHandleBusy = Arrow.HowToHandleBusy.CANCEL;
                }

                Arrow dataPoint = AssetDatabase.LoadAssetAtPath<Arrow>(GetContainingFolder() + "/" + GetArrowFolderName() + "/" + _id + ".asset");

                bool exists = dataPoint != null;

                if (!exists)
                {
                    dataPoint = ScriptableObject.CreateInstance<Arrow>();
                }

                dataPoint.Rewrite(_id, _systemScriptable, _priorityValue, _rule, _howToHandleBusy, _howPriorityCalculated);

                if (exists)
                {
                    EditorUtility.SetDirty(dataPoint);
                }
                else
                {
                    AssetDatabase.CreateAsset(dataPoint, GetContainingFolder() + "/" + GetArrowFolderName() + "/" + _id + ".asset");
                }

                //3) remove from list of uninvolved Assets for clean up later
                notBeingUsed.Remove(_id);
            }
        }

        bool TryEvalFlagList(string str, out SortedStrictDictionary<FlagID, Criterion> flagChecks)
        {
            bool success = true;
            flagChecks = new SortedStrictDictionary<FlagID, Criterion>();

            string[] inputs = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < inputs.Length; i++)
            {
                success = TryEvalSpecificFlag(inputs[i], ref flagChecks);
                if(!success)
                {
                    break;
                }
            }

            return success;
        }

        bool TryEvalSpecificFlag(string input, ref SortedStrictDictionary<FlagID, Criterion> flagChecks)
        {
            //Variable set-up
            CriterionComparisonType compareType = CriterionComparisonType.INVALID;
            Flag.ValueType valueType = Flag.ValueType.INVALID;
            bool valid = false;
            string flag = "";
            string[] tmp;
            float a = 0;
            float b = 0;

            //Trim input
            input = input.Trim();

            //Check input for key symbols
            bool hasLess = input.IndexOf('<') != -1;
            bool hasGreat = input.IndexOf('>') != -1;
            bool hasEq = input.IndexOf('=') != -1;
            bool hasEx = input.IndexOf('!') != -1;

            if (hasGreat)
            {
                valueType = Flag.ValueType.FLOAT;
                valid = !hasEx && !hasLess;
                if (valid)
                {
                    valid = IsValidLessGreat(input, '>', out a, out flag);
                    if (!valid)
                    {
                        tmp = input.Split('>');
                        valid = tmp.Length == 3;
                        if (valid)
                        {
                            bool hasLeftEq = tmp[1].IndexOf('=') != -1;
                            bool hasRightEq = tmp[2].IndexOf('=') != -1;

                            int leftStartIndex = hasLeftEq ? 1 : 0;
                            int rightStartIndex = hasRightEq ? 1 : 0;

                            if (float.TryParse(tmp[0], out a)
                                && float.TryParse(tmp[2].Substring(rightStartIndex), out b))
                            {
                                flag = tmp[1].Substring(leftStartIndex).Trim();
                                valid = IsFlagNameValid(flag);
                                if (valid)
                                {
                                    //String
                                    string output = a + " >";
                                    if (hasLeftEq)
                                    {
                                        output += "=";
                                    }
                                    output += " " + flag + " >";
                                    if (hasRightEq)
                                    {
                                        output += "=";
                                    }
                                    output += " " + b;
                                    Debug.Log(output);

                                    if (hasLeftEq)
                                    {
                                        if (hasRightEq)
                                        {
                                            compareType = CriterionComparisonType.RANGE_CLOSED;
                                        }
                                        else
                                        {
                                            compareType = CriterionComparisonType.RANGE_CLOSED_OPEN;
                                        }
                                    }
                                    else
                                    {
                                        if (hasRightEq)
                                        {
                                            compareType = CriterionComparisonType.RANGE_OPEN_CLOSED;
                                        }
                                        else
                                        {
                                            compareType = CriterionComparisonType.RANGE_OPEN;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                valid = false;
                            }

                        }
                    }
                    else
                    {
                        if (hasEq)
                        {
                            compareType = CriterionComparisonType.GREATER_EQUAL;
                        }
                        else
                        {
                            compareType = CriterionComparisonType.GREATER;
                        }
                    }
                }
            }
            else if (hasLess)
            {
                valueType = Flag.ValueType.FLOAT;
                valid = IsValidLessGreat(input, '<', out a, out flag);
                if (valid)
                {
                    if (hasEq)
                    {
                        compareType = CriterionComparisonType.LESS_EQUAL;
                    }
                    else
                    {
                        compareType = CriterionComparisonType.LESS;
                    }
                }
            }
            else if (hasEq)
            {
                tmp = input.Split('=');

                valid = !hasEx && tmp.Length == 2;

                if (valid)
                {
                    if (float.TryParse(tmp[1], out a)) //number value
                    {
                        valueType = Flag.ValueType.FLOAT;
                        flag = tmp[0].Trim();
                        valid = IsFlagNameValid(flag);
                        if (valid)
                        {
                            compareType = CriterionComparisonType.EQUALS;
                            Debug.Log(flag + " = " + a);
                        }
                    }
                    else
                    {
                        valueType = Flag.ValueType.SYMBOL;
                        flag = tmp[0].Trim();
                        string enumPossibly = tmp[1].Trim();
                        valid = IsFlagNameValid(flag) && IsFlagNameValid(enumPossibly);
                        if (valid)
                        {
                            if (Enum.TryParse(enumPossibly, out ValveInternalSymbols internalSymbol))
                            {
                                a = (float)internalSymbol;
                                compareType = CriterionComparisonType.EQUALS;
                                Debug.Log(flag + " = " + enumPossibly);
                            }
                            else
                            {
                                Debug.LogError("Could not parse \"" + enumPossibly + "\" into a ValveInternalSymbols. Perhaps try to recompile?");
                                valid = false;
                            }
                        }
                    }
                }
            }
            else if (hasEx)
            {
                valueType = Flag.ValueType.BOOL;
                valid = input.IndexOf('!') == 0
                    && input.LastIndexOf('!') == 0;

                if (valid)
                {
                    flag = input.Substring(1);
                    a = 0;
                    compareType = CriterionComparisonType.EQUALS;

                    valid = IsFlagNameValid(flag);
                    if (valid)
                    {
                        Debug.Log(flag + " = FALSE");
                    }
                }
            }
            else
            {
                valueType = Flag.ValueType.BOOL;
                valid = IsFlagNameValid(input);
                if (valid)
                {
                    flag = input;
                    a = 1;
                    compareType = CriterionComparisonType.EQUALS;
                    Debug.Log(flag + " = TRUE");
                }
            }

            if (valid)
            {
                valid = ProcessCriterion(flag, valueType, compareType, a, b, ref flagChecks);
            }
            
            if(!valid)
            {
                Debug.LogError("\"" + input + "\" was found INVALID");
            }

            return valid;
        }

        bool IsValidLessGreat(string input, char compareChar, out float a, out string flag)
        {
            bool valid;
            flag = "";
            a = 0;

            bool hasEq = input.IndexOf('=') != -1;
            bool hasEx = input.IndexOf('!') != -1;

            valid = !hasEx;

            if (valid)
            {
                string[] tmp = input.Split(compareChar);
                valid = tmp.Length == 2;
                if (valid)
                {
                    int startIndex = hasEq ? 1 : 0;
                    if (float.TryParse(tmp[1].Substring(1), out a))
                    {
                        flag = tmp[0].Trim();
                        valid = IsFlagNameValid(flag);
                        if (valid)
                        {
                            string output = flag + " " + compareChar;
                            if (hasEq)
                            {
                                output += "=";
                            }
                            output += " " + a;

                            Debug.Log(output);
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        bool IsFlagNameValid(string flag)
        {
            bool valid = true;

            char[] arr = flag.ToCharArray();

            if (char.IsLetter(arr[0]))
            {
                foreach (char e in arr)
                {
                    if (!char.IsLetterOrDigit(e) && e != '_')
                    {
                        valid = false;
                        break;
                    }
                }
            }
            else
            {
                valid = false;
            }

            return valid;
        }

        bool ProcessCriterion(string flag, Flag.ValueType valueType, CriterionComparisonType comparisonType, float a, float b, ref SortedStrictDictionary<FlagID, Criterion> flagChecks)
        {
            bool success = true;

            FlagID flagID = Goddess.instance.ConnectFlag(flag, valueType, this);

            if (flagID != FlagID.INVALID)
            {
                flagChecks.Add(flagID, new Criterion(flagID, comparisonType, a, b));
                flagsBeingUsed.Add(flag.ToUpper());
                flagsNoLongerBeingUsed.Remove(flag.ToUpper());
            }
            else
            {
                success = false;
            }

            return success;
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

        private string GetArrowFolderName()
        {
            return name + " Arrows";
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
