using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative System", menuName = "Artemis/Overall Narrative System")]
    public class Goddess : ScriptableSingleton<Goddess>
    {
        [SerializeField]
        private List<Flag> flagsToKeep;

        //For managing internal symbols
        [SerializeField]
        private SortedStrictDictionary<FlagID, Flag> idToFlag;
        [SerializeField]
        private SortedStrictList<FlagID> idsUsed;

        [SerializeField]
        private SortedStrictDictionary<string, int> toAdd;
        [SerializeField]
        private SortedStrictList<int> intsReadyToConvert;
        [SerializeField]
        private List<FlagID> toRemove;

        [SerializeField]
        private SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>> flagIDConnections;

        public void Awake()
        {
        }

        public Flag GetFlag(string name)
        {
            Flag rtn = null;
            FlagID id;

            //TODO: create a TryGetValue function that only requires the one binary search instead of the two.
            if (Enum.TryParse<FlagID>(name, out id))
            {
                rtn = idToFlag[id];
            }
            return rtn;
        }

#if UNITY_EDITOR
        public FlagID ConnectFlag(string name, Flag.ValueType valueType, PreDictionaryFletcher connector)
        {
            //New Code
            bool successful = true;

            name = name.ToUpper();

            FlagID id;
            Flag flag;
            idToFlag ??= new SortedStrictDictionary<FlagID, Flag>();
            //Checks if flag enum already exists
            bool found = Enum.TryParse<FlagID>(name, out id);
            int idInt;
            if(!found && toAdd.LinearSearch(name,out idInt))
            {
                id = (FlagID)idInt;
                found = true;
            }

            if (found)
            {
                flag = idToFlag[id];
                Flag.ValueType originalValueType = flag.GetValueType();
                flagIDConnections ??= new SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>>();
                flagIDConnections[id] ??= new List<PreDictionaryFletcher>();

                if (originalValueType != valueType)
                {
                    if(flagIDConnections[id].Contains(connector))
                    {
                        flag.SetValueType(valueType);
                    }
                    else
                    {
                        successful = false;
                        Debug.LogError("The flag \"" + name + "\" already exists as a " + originalValueType + ". Feltcher is trying to use this flag as a " + valueType + ".");
                    }
                }

                if(successful)
                {
                    if (!flagIDConnections[id].Contains(connector))
                    {
                        flagIDConnections[id].Add(connector);
                    }
                }
            }
            else
            {

                int newIdValue = FindValidUnusedFlagIDNumber();
                if (newIdValue != (int)FlagID.INVALID)
                {
                    if (!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetFlagRepoFolderName()))
                    {
                        AssetDatabase.CreateFolder(GetContainingFolder(), GetFlagRepoFolderName());
                    }

                    flag = AssetDatabase.LoadAssetAtPath<Flag>(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");

                    bool exists = flag != null;

                    if (!exists)
                    {
                        flag = ScriptableObject.CreateInstance<Flag>();
                        flag.SetValue(0);
                        flag.SetValueType(valueType);
                        AssetDatabase.CreateAsset(flag, GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");
                    }

                    toAdd.Add(name, newIdValue);
                    id = (FlagID)newIdValue;

                    idToFlag.Add(id, flag);
                    flagIDConnections.Add(id, new List<PreDictionaryFletcher>());
                    flagIDConnections[id].Add(connector);
                }
                else
                {
                    successful = false;
                }
            }

            EditorUtility.SetDirty(this);

            if(!successful)
            {
                id = FlagID.INVALID;
            }

            return id;
        }

        public void DisconnectFlag(string name, PreDictionaryFletcher connector)
        {
            name = name.ToUpper();
            FlagID id;

            if (Enum.TryParse<FlagID>(name, out id))
            {
                flagIDConnections ??= new SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>>();
                flagIDConnections[id] ??= new List<PreDictionaryFletcher>();

                if (flagIDConnections[id].Contains(connector))
                {
                    flagIDConnections[id].Remove(connector);
                }

                if (flagIDConnections[id].Count == 0 && !flagsToKeep.Contains(idToFlag[id]))
                {
                    idToFlag.Remove(id);
                    flagIDConnections.Remove(id);
                    AssetDatabase.DeleteAsset(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");
                    toRemove.Add(id);
                }
            }

            EditorUtility.SetDirty(this);
        }

        private string GetContainingFolder()
        {
            string rtn = AssetDatabase.GetAssetPath(this);
            rtn = rtn.Substring(0, rtn.LastIndexOf('/'));
            return rtn;
        }

        private string GetFlagRepoFolderName()
        {
            return this.name + " Flag Repo";
        }

        public void WriteFlagEnumScript()
        {
            string elementName;
            int elementInt;
            FlagID elementID;

            //Remove unused enums
            for(int i = 0; i < toRemove.Count; i++)
            {
                elementID = toRemove[i];
                idsUsed.Remove(elementID);
            }

            //Build new enum script
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");
            stringBuilder.Append("namespace Artemis\n{\n\tpublic enum FlagID\n\t{\n\t\tINVALID = -1");

            for (int i = 0; i < idsUsed.Count; i++)
            {
                elementID = idsUsed[i];
                elementInt = (int)elementID;

                stringBuilder.Append(",\n\t\t" + elementID.ToString() + " = " + elementInt);
            }

            for (int i = 0; i < toAdd.Count; i++)
            {
                elementName = toAdd[i].Key;
                elementInt = toAdd[i].Value;
                elementID = (FlagID)elementInt;

                idsUsed.Add(elementID);

                stringBuilder.Append(",\n\t\t" + elementName + " = " + elementInt);
            }

            stringBuilder.Append("\n\t}\n}");


            //Determine File Path
            string relativePath = GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + nameof(FlagID) + ".cs";
            string path;
            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"
            path +=  relativePath;

            //Write new script
            if(!File.Exists(path))
            {
                File.Create(path);
            }

            File.WriteAllText(path,stringBuilder.ToString());

            //Reset toAdd/Remove
            toAdd.Clear();
            toRemove.Clear();
            intsReadyToConvert.Clear();

            AssetDatabase.ImportAsset(relativePath);
        }

        private int FindValidUnusedFlagIDNumber()
        {
            int rtn;
            int start;
            int invalid = (int)FlagID.INVALID;

            if (intsReadyToConvert.Count == 0)
            {
                if (idsUsed.Count != 0)
                {
                    rtn = (int)idsUsed[idsUsed.Count - 1] + 1;
                }
                else
                {
                    rtn = 0;
                }
            }
            else
            {
                rtn = intsReadyToConvert[intsReadyToConvert.Count - 1] + 1;
            }

            if(rtn == int.MaxValue)
            {
                rtn = int.MinValue;
            }

            start = rtn;

            while(rtn == invalid || idsUsed.Has((FlagID)rtn) || intsReadyToConvert.Has(rtn))
            {
                rtn++;
                if (rtn == int.MaxValue)
                {
                    rtn = int.MinValue;
                }

                if(rtn == start)
                {
                    //Looped the whole way around and had no luck!
                    Debug.LogError("You've run out of space for flags to be tracked. That's over (2^32)-1 flags!");
                    rtn = invalid;
                    break;
                }
            }

            if(rtn != invalid)
            {
                intsReadyToConvert.Add(rtn);
            }

            return rtn;
        }

        [ContextMenu("Reset Entirely")]
        private void Reset()
        {
            for (int i = 0; i < idToFlag.Count; i++)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(idToFlag[i].Value));
            }

            toAdd.Clear();
            toRemove.Clear();
            intsReadyToConvert.Clear();
            flagsToKeep.Clear();
            idToFlag.Clear();
            flagIDConnections.Clear();
            idsUsed.Clear();

            WriteFlagEnumScript();
        }
#endif
    }
}
