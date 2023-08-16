using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Perell.Artemis.Generated;


#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor;
#endif
namespace Perell.Artemis
{
    [FilePath("Assets/Scripts/Generated/Artemis/Goddess.art", FilePathAttribute.Location.ProjectFolder)]
    public class Goddess : ScriptableSingleton<Goddess>
    {
        const string CURRENT_VERSION = "0.2.4";

        [SerializeField]
        private List<FlagID> flagsIdsToKeep = new List<FlagID>();

        [SerializeField]
        private SortedStrictDictionary<FlagID, Flag.ValueType> flagValueTypes = new SortedStrictDictionary<FlagID, Flag.ValueType>();

        [SerializeField]
        private SortedStrictDictionary<FlagID, InternalSymbolCompiler> flagSymbolTypes = new SortedStrictDictionary<FlagID, InternalSymbolCompiler>();

        //For managing internal symbols
        [SerializeField]
        private SortedStrictList<FlagID> idsUsed = new SortedStrictList<FlagID>();
        [SerializeField]
        private SortedStrictDictionary<string, int> toAdd = new SortedStrictDictionary<string, int>();
        [SerializeField]
        private SortedStrictList<int> intsReadyToConvert = new SortedStrictList<int>();
        [SerializeField]
        private List<FlagID> toRemove = new List<FlagID>();

        [SerializeField]
        public FlagBundle[] globallyLoadedFlagBundles = new FlagBundle[0];

        [SerializeField]
        private SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>> flagIDConnections = new SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>>();

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
            InitializeScripts();
        }

        public Flag.ValueType GetFlagValueType(FlagID id)
        {
            return flagValueTypes[id];
        }

        private void InitializeScripts()
        {
#if UNITY_EDITOR
            string relativePath, path, totalPath;
            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"

            //1) delete sample flagid
            AssetDatabase.DeleteAsset("Assets/Samples/Artemis/"+CURRENT_VERSION+ "/Initialize Package");


            //2) create assembly for generated
            relativePath = GetContainingFolder() + "Perell.Artemis.Generated.asmdef";
            totalPath = path + relativePath;

            if (!File.Exists(totalPath))
            {
                if (!System.IO.Directory.Exists(totalPath.Substring(0, totalPath.LastIndexOf('/'))))
                {
                    System.IO.Directory.CreateDirectory(totalPath.Substring(0, totalPath.LastIndexOf('/')));
                }

                StreamWriter stream = File.CreateText(totalPath); 
                stream.Write("{\"name\": \"Perell.Artemis.Generated\",\"rootNamespace\": \"\",\"references\": [],\"includePlatforms\": [],\"excludePlatforms\": [],\"allowUnsafeCode\": false,\"overrideReferences\": false,\"precompiledReferences\": [],\"autoReferenced\": true,\"defineConstraints\": [],\"versionDefines\": [],\"noEngineReferences\": false}");
                stream.Close();
                AssetDatabase.ImportAsset(relativePath);
            }

            //3) create assets flagid
            relativePath = GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + nameof(FlagID) + ".cs";
            totalPath = path + relativePath;

            if (!File.Exists(path + relativePath))
            {
                WriteFlagEnumScript();
            }
#endif
        }

#if UNITY_EDITOR
        public FlagID[] GetFlagIDs()
        {
            idsUsed ??= new SortedStrictList<FlagID>();
            return idsUsed.ToArray();
        }

        public FlagID ConnectFlag(string name, Flag.ValueType valueType, PreDictionaryFletcher connector)
        {
            //New Code
            bool successful = true;

            name = name.ToUpper();

            FlagID id;
            flagValueTypes ??= new SortedStrictDictionary<FlagID, Flag.ValueType>();
            flagSymbolTypes ??= new SortedStrictDictionary<FlagID, InternalSymbolCompiler>();
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
                Flag.ValueType originalValueType = flagValueTypes[id];
                flagIDConnections ??= new SortedStrictDictionary<FlagID, List<PreDictionaryFletcher>>();
                flagIDConnections[id] ??= new List<PreDictionaryFletcher>();

                if (originalValueType != valueType)
                {
                    if(flagIDConnections[id].Contains(connector))
                    {
                        flagValueTypes[id] = valueType;
                        if(valueType == Flag.ValueType.SYMBOL)
                        {
                            flagSymbolTypes.Add(id, new InternalSymbolCompiler(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/", name));

                        }
                        else if (originalValueType == Flag.ValueType.SYMBOL)
                        {
                            flagSymbolTypes[id].DeleteFlagEnumScript();
                            flagSymbolTypes.Remove(id);
                        }
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

                int newIDValue = FindValidUnusedFlagIDNumber();
                if (newIDValue != (int)FlagID.INVALID)
                {
                    toAdd.Add(name, newIDValue);
                    id = (FlagID)newIDValue;

                    flagValueTypes.Add(id, valueType);
                    flagIDConnections.Add(id, new List<PreDictionaryFletcher>());
                    flagIDConnections[id].Add(connector);
                    if (valueType == Flag.ValueType.SYMBOL)
                    {
                        flagSymbolTypes.Add(id, new InternalSymbolCompiler(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/", name));
                    }
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

            Modify();

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
                flagSymbolTypes ??= new SortedStrictDictionary<FlagID, InternalSymbolCompiler>();

                if (flagIDConnections[id].Contains(connector))
                {
                    flagIDConnections[id].Remove(connector);
                }

                if (flagIDConnections[id].Count == 0 && !flagsIdsToKeep.Contains(id))
                {
                    flagIDConnections.Remove(id);
                    flagValueTypes.Remove(id);
                    toRemove.Add(id);
                    flagSymbolTypes[id]?.DeleteFlagEnumScript();
                    flagSymbolTypes.Remove(id);
                }
            }

            EditorUtility.SetDirty(this);

            Modify();
        }

        private string GetContainingFolder()
        {
            string rtn = AssetDatabase.GetAssetPath(this);
            rtn = "Assets/Scripts/Generated/Artemis/";
            return rtn;
        }

        private string GetFlagRepoFolderName()
        {
            return this.name + "FlagEnums";
        }

        public void WriteFlagEnumScript()
        {
            string elementName;
            int elementInt;
            FlagID elementID;

            //Remove unused enums
            toRemove ??= new List<FlagID>();
            for (int i = 0; i < toRemove.Count; i++)
            {
                elementID = toRemove[i];
                idsUsed.Remove(elementID);
            }

            //Build new enum script
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");
            stringBuilder.Append("namespace Perell.Artemis.Generated\n{\n\tpublic enum FlagID\n\t{\n\t\tINVALID = -1");

            for (int i = 0; i < idsUsed.Count; i++)
            {
                elementID = idsUsed[i];
                elementInt = (int)elementID;

                stringBuilder.Append(",\n\t\t" + elementID.ToString() + " = " + elementInt);
            }

            for (int i = 0; i < toAdd.Count; i++)
            {
                elementName = toAdd.GetTupleAtIndex(i).Key;
                elementInt = toAdd.GetTupleAtIndex(i).Value;
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

            if (File.Exists(path))
            {
                File.WriteAllText(path, stringBuilder.ToString());
            }
            else
            {
                if (!System.IO.Directory.Exists(path.Substring(0, path.LastIndexOf('/'))))
                {
                    System.IO.Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));
                }

                StreamWriter stream = File.CreateText(path);
                stream.Write(stringBuilder.ToString());
                stream.Close();
            }

            //Reset toAdd/Remove
            toAdd.Clear();
            toRemove.Clear();
            intsReadyToConvert.Clear();

            AssetDatabase.ImportAsset(relativePath);

            //Write the other enum scripts
            for (int i = 0; i < flagSymbolTypes.Count; i++)
            {
                flagSymbolTypes.GetTupleAtIndex(i).Value.WriteFlagEnumScript();
            }

            Modify();
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

        public int FindSymbolValueOfFlag(FlagID id, string enumPossibly)
        {
            int rtn = -1;

            if(flagSymbolTypes.HasKey(id))
            {
                rtn = flagSymbolTypes[id].FindValueOfString(enumPossibly.ToUpper());
            }

            return rtn;
        }

        public System.Type GetFlagSymbolType(FlagID id)
        {
            System.Type rtn = typeof(Flag.ValueType);

            if (flagSymbolTypes.HasKey(id))
            {
                rtn = flagSymbolTypes[id].GetEnumType();
            }

            return rtn;
        }

        public void Reset()
        {

            flagsIdsToKeep.Clear();
            toAdd.Clear();
            intsReadyToConvert.Clear();
            flagValueTypes.Clear();
            flagIDConnections.Clear();
            for(int i = flagSymbolTypes.Count - 1; i >= 0; i--)
            {
                flagSymbolTypes.GetTupleAtIndex(i).Value.DeleteFlagEnumScript();
                flagSymbolTypes.RemoveAt(i);
            }
            globallyLoadedFlagBundles = new FlagBundle[0];

            foreach (FlagID e in Enum.GetValues(typeof(FlagID)))
            {
                if (e != FlagID.INVALID)
                {
                    toRemove.Add(e);
                }
            }

            WriteFlagEnumScript();

            toRemove.Clear();
            idsUsed.Clear();

            Modify();
        }
#endif

        public void Modify()
        {
            Save(true);
        }
    }
}
