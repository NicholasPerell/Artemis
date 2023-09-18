using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;
using System;
using UnityEditor;

namespace Perell.Artemis.Example.Rituals
{
    [System.Serializable]
    public class DialogueData
    {
        //Column 0: Dialogue Script
        public enum Speaker
        {
            SORCERER,
            MAGE,
            DEMON_SPIRIT,
            DEMON_BOSS,
            SMITH,
            SERVANT,
            NARRATION
        }

        [System.Serializable]
        public struct LineData
        {
            public Speaker speaker;
            public string text;
        }

        public LineData[] lines;

        //Column 1: Flag Value Changes
        [System.Serializable]
        public struct FlagChangeData
        {
            [SerializeField]
            private string stringId;
            [SerializeField]
            private FlagID id;
            [SerializeField]
            private string stringValue;
            [SerializeField]
            private float value;

            public FlagChangeData(string _id, string _value)
            {
                stringId = _id.Trim();
                stringValue = _value.Trim();

                if(!System.Enum.TryParse<FlagID>(stringId,true,out id))
                {
                    id = FlagID.INVALID;
                }

                value = -1;
                if(id != FlagID.INVALID)
                {
                    switch (Goddess.instance.GetFlagValueType(id))
                    {
                        case Flag.ValueType.BOOL:
                            value = (stringValue.ToLower() == "true" ? 1 : 0);
                            break;
                        case Flag.ValueType.FLOAT:
                            value = float.Parse(stringValue);
                            break;
                        case Flag.ValueType.SYMBOL:
                            object symbol;
                            if (System.Enum.TryParse(Goddess.instance.GetFlagSymbolType(id), stringValue, true, out symbol))
                            {
                                value = (int)symbol;
                            }
                            else
                            {
                                value = -1;
                            }
                            break;
                    }
                    stringValue = null;
                }
            }

            public FlagID GetID()
            {
                if(id == FlagID.INVALID)
                {
                    if (!System.Enum.TryParse<FlagID>(stringId, true, out id))
                    {
                        id = FlagID.INVALID;
                    }
                }
                return id;
            }

            public bool TryGetValue(out float result)
            {
                if(stringValue != null && stringValue != "")
                {
                    if (GetID() != FlagID.INVALID)
                    {
                        switch (Goddess.instance.GetFlagValueType(id))
                        {
                            case Flag.ValueType.BOOL:
                                value = (stringValue.ToLower() == "true" ? 1 : 0);
                                break;
                            case Flag.ValueType.FLOAT:
                                value = float.Parse(stringValue);
                                break;
                            case Flag.ValueType.SYMBOL:
                                object symbol;
                                if (System.Enum.TryParse(Goddess.instance.GetFlagSymbolType(id), stringValue, true, out symbol))
                                {
                                    value = (int)symbol;
                                }
                                else
                                {
                                    value = -1;
                                }
                                break;
                        }
                        stringValue = null;
                    }
                }
                result = value;
                return stringValue == null || stringValue == "";
            }

            public bool TestsInvalid()
            {
                return (id == FlagID.INVALID && !IsFlagNameValid(stringId))
                    && (stringValue != null && !IsFlagNameValid(stringValue));
            }

            private bool IsFlagNameValid(string flag)
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
        }

        public FlagChangeData[] flagChanges;

        //Column 2: Archer Arrow Changes
        [System.Serializable]
        public struct ArcherChangeData
        {
            public Archer archer;
            public string[] arrowNames;
            public ArrowBundle arrowBundle;
            public bool dumping;
        }

        public ArcherChangeData[] archerChanges;
    }

    [CreateAssetMenu(fileName = "New Artemis Dialogue Delivery System", menuName = "Artemis Examples/Dialogue Delivery System")]
    public class DialogueBoxFletcher : Fletcher<DialogueData>
    {
        public enum AffectedArchers
        {
            INVALID = -1,
            MAGE,
            SERVANT,
            SMITH,
            RUN_START,
            BOSS_START,
            LOSE_TO_BOSS,
            LOBBY_RETURN
        }

        [System.Serializable]
        public struct ArcherAttributes
        {
            public AffectedArchers archerId;
            public Archer archer;
        }

        [SerializeField]
        ArcherAttributes[] archerAttributes;

        [HideInInspector]
        [SerializeField]
        bool generating;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(!generating)
            {
                return;
            }

            generating = false;

            Arrow[] arrows = RetrieveAllGeneratedArrows();

            if(arrows == null || arrows.Length <= 1)
            {
                return;
            }

            Array.Sort<Arrow>(arrows, (Arrow a, Arrow b) => { return a.GetArrowID().CompareTo(b.GetArrowID()); });

            DialogueData data;
            //ref DialogueData.ArcherChangeData change;
            List<Arrow> arrowsBundled;
            object searchKeyObj;
            int searchKeyInt;
            Arrow searchKeyArrow = Arrow.CreateInstance<Arrow>();
            int searchIndex;
            string arrowFilePath;
            for (int i = 0; i < arrows.Length; i++)
            {
                if(FindData(arrows[i].GetArrowID(), out data))
                {
                    for( int j = 0; j < data.archerChanges.Length; j++)
                    {
                        ref DialogueData.ArcherChangeData change = ref data.archerChanges[j];
                        if (change.arrowBundle == null)
                        {
                            arrowsBundled = new List<Arrow>();
                            foreach(string arrowName in change.arrowNames)
                            {
                                if (System.Enum.TryParse(GetSymbolType(), arrowName, true, out searchKeyObj))
                                {
                                    searchKeyInt = (int)searchKeyObj;
                                    searchKeyArrow.Rewrite(searchKeyInt, null, 0, null, Arrow.HowToHandleBusy.DELETE, Arrow.HowPriorityCalculated.SET_VALUE);
                                    searchIndex = Array.BinarySearch<Arrow>(arrows, searchKeyArrow, Comparer<Arrow>.Create((Arrow a, Arrow b) => { return a.GetArrowID().CompareTo(b.GetArrowID()); }));
                                    if(searchIndex >= 0)
                                    {
                                        arrowsBundled.Add(arrows[searchIndex]);
                                    }
                                }
                            }
                            change.arrowBundle = ArrowBundle.CreateInstance(arrowsBundled.ToArray());
                            arrowFilePath = AssetDatabase.GetAssetPath(arrows[i]);
                            AssetDatabase.DeleteAsset(arrowFilePath.Substring(0,arrowFilePath.Length - 6) + "_Bundle" + j + ".asset");
                            AssetDatabase.CreateAsset(change.arrowBundle, arrowFilePath.Substring(0,arrowFilePath.Length - 6) + "_Bundle" + j + ".asset");
                            AssetDatabase.ImportAsset(arrowFilePath.Substring(0,arrowFilePath.Length - 6) + "_Bundle" + j + ".asset");
                        }
                    }
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        protected override bool SetUpDataFromCells(string[] dataToInterpret, out DialogueData valueDetermined)
        {
            generating = true;
            bool success = true;

            //Column 0: Dialogue Script
            string[] stringLines = dataToInterpret[0].Split('\n',System.StringSplitOptions.RemoveEmptyEntries);

            valueDetermined = new DialogueData();
            valueDetermined.lines = new DialogueData.LineData[stringLines.Length];

            if (stringLines.Length == 0)
            {
                Debug.LogError("Not Enough Lines");
                success = false;
            }
            else
            {
                DialogueData.Speaker speakerEnum;
                string speakerString;
                int indexOfDivider;
                for (int i = 0; i < stringLines.Length; i++)
                {
                    indexOfDivider = stringLines[i].IndexOf(':');
                    if(indexOfDivider < 0)
                    {
                Debug.LogError("No : found");
                        success = false;
                        break;
                    }

                    speakerString = stringLines[i].Substring(0, indexOfDivider);
                    if(System.Enum.TryParse<DialogueData.Speaker>(speakerString, true, out speakerEnum))
                    {
                        valueDetermined.lines[i].speaker = speakerEnum;
                        valueDetermined.lines[i].text = stringLines[i].Substring(indexOfDivider + 1).Trim();
                    }
                    else
                    {
                        Debug.LogError("No enum found for "+ speakerString);
                        success = false;
                        break;
                    }
                }
            }

            //Column 1: Flag Value Changes
            string[] flagLines = dataToInterpret[1].Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

            if(flagLines.Length == 0)
            {
                valueDetermined.flagChanges = new DialogueData.FlagChangeData[0];
            }
            else
            {
                valueDetermined.flagChanges = new DialogueData.FlagChangeData[flagLines.Length];
                int indexOfDivider;
                for (int i = 0; i < flagLines.Length; i++)
                {
                    indexOfDivider = flagLines[i].IndexOf('=');
                    if (indexOfDivider <= 0)
                    {
                        Debug.LogError("No = found");
                        success = false;
                        break;
                    }
                    valueDetermined.flagChanges[i] = new DialogueData.FlagChangeData(flagLines[i].Substring(0, indexOfDivider), flagLines[i].Substring(indexOfDivider + 1));
                    if(valueDetermined.flagChanges[i].TestsInvalid())
                    {
                        Debug.LogError($"\"{flagLines[i]}\" not found valid");
                        success = false;
                        break;
                    }
                }
            }

            //Column 2: Archer Arrow Changes
            string[] archerLines = dataToInterpret[2].Split('=', System.StringSplitOptions.RemoveEmptyEntries);
            if (archerLines.Length == 0)
            {
                valueDetermined.archerChanges = new DialogueData.ArcherChangeData[0];
            }
            else
            {
                valueDetermined.archerChanges = new DialogueData.ArcherChangeData[archerLines.Length];
                string[] arrowLines;
                string archerString;
                for (int i = 0; i < archerLines.Length; i++)
                {
                    valueDetermined.archerChanges[i] = new DialogueData.ArcherChangeData();
                    arrowLines = archerLines[i].Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
                    if (arrowLines.Length <= 1)
                    {
                        Debug.LogError($"\"{archerLines[i]}\" not found valid # of lines");
                        success = false;
                        break;
                    }
                    else
                    {
                        archerString = arrowLines[0].Trim();
                        valueDetermined.archerChanges[i].dumping = (archerString.IndexOf('-') != 0);
                        if(!valueDetermined.archerChanges[i].dumping)
                        {
                            archerString = archerString.Substring(1);
                        }

                        valueDetermined.archerChanges[i].archer = GetArcher(archerString);
                        if(!valueDetermined.archerChanges[i].archer)
                        {
                            Debug.LogError($"\"{archerLines[i]}\" not found valid archer name of {archerString}");
                            success = false;
                            break;
                        }

                        valueDetermined.archerChanges[i].arrowNames = new string[arrowLines.Length - 1];
                        for(int j = 0; j < valueDetermined.archerChanges[i].arrowNames.Length; j++)
                        {
                            valueDetermined.archerChanges[i].arrowNames[j] = arrowLines[j + 1];
                        }

                        valueDetermined.archerChanges[i].arrowBundle = null;
                    }
                }
            }

            return success;
        }

        private Archer GetArcher(string affectedArcherName)
        {
            AffectedArchers affectedArcher;
            if (System.Enum.TryParse<AffectedArchers>(affectedArcherName, true, out affectedArcher))
            {
                for (int i = 0; i < archerAttributes.Length; i++)
                {
                    if (affectedArcher == archerAttributes[i].archerId)
                    {
                        return archerAttributes[i].archer;
                    }
                }
            }
            return null;
        }
    }
}
