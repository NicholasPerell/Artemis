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
    [System.Serializable]
    public class InternalSymbolCompiler
    {
        //For managing internal symbols
        [SerializeField]
        private SortedStrictList<int> idsUsed;
        [HideInInspector]
        private SortedStrictDictionary<string, int> toAdd;
        [HideInInspector]
        private SortedStrictList<int> intsReadyToConvert;
        [HideInInspector]
        private List<int> toRemove;

        [HideInInspector]
        private string fileLocation;
        [SerializeField]
        private string enumName;
        [SerializeField]
        private System.Type enumType = null;

        public InternalSymbolCompiler(string _fileLocation, string _enumPrefix)
        {
            fileLocation = _fileLocation;
            enumName = _enumPrefix + "_ID";

            idsUsed = new SortedStrictList<int>();
            toAdd = new SortedStrictDictionary<string, int>();
            intsReadyToConvert = new SortedStrictList<int>();
            toRemove = new List<int>();

            CheckForCompiledScript();

            string relativePath = fileLocation + enumName + ".cs";
            string path;
            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"
            path += relativePath;

        }

        ~InternalSymbolCompiler()
        {

            enumType = null;

            //Determine File Path
            string relativePath = fileLocation + enumName + ".cs";
            string path;
            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"
            path += relativePath;

            //Delete unused script
            if (!File.Exists(path))
            {
                File.Delete(path);
            }
        }

        void CheckForCompiledScript()
        {
            enumType = System.Type.GetType("Artemis."+enumName);
        }

        public void WriteFlagEnumScript()
        {
            string elementName;
            int elementInt;

            //Remove unused enums
            for (int i = 0; i < toRemove.Count; i++)
            {
                idsUsed.Remove(toRemove[i]);
            }

            //Build new enum script
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");
            stringBuilder.Append("namespace Artemis\n{\n\tpublic enum " + enumName + "\n\t{\n\t\tINVALID = -1");
            if (enumType != null)
            {
                for (int i = 0; i < idsUsed.Count; i++)
                {
                    elementInt = idsUsed[i];
                    stringBuilder.Append(",\n\t\t" + System.Enum.GetName(enumType, elementInt) + " = " + elementInt);
                }
            }

            for (int i = 0; i < toAdd.Count; i++)
            {
                elementName = toAdd[i].Key;
                elementInt = toAdd[i].Value;

                idsUsed.Add(elementInt);

                stringBuilder.Append(",\n\t\t" + elementName + " = " + elementInt);
            }

            stringBuilder.Append("\n\t}\n}");


            //Determine File Path
            string relativePath = fileLocation + enumName + ".cs";
            string path;
            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"
            path += relativePath;

            //Write new script
            if (!File.Exists(path))
            {
                File.Create(path);
            }

            File.WriteAllText(path, stringBuilder.ToString());

            //Reset toAdd/Remove
            toAdd.Clear();
            toRemove.Clear();
            intsReadyToConvert.Clear();

            AssetDatabase.ImportAsset(relativePath);
            CheckForCompiledScript();
        }

        private int FindValidUnusedFlagIDNumber()
        {
            int rtn;
            int start;
            int invalid = -1;

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

            if (rtn == int.MaxValue)
            {
                rtn = int.MinValue;
            }

            start = rtn;

            while (rtn == invalid || idsUsed.Has(rtn) || intsReadyToConvert.Has(rtn))
            {
                rtn++;
                if (rtn == int.MaxValue)
                {
                    rtn = int.MinValue;
                }

                if (rtn == start)
                {
                    //Looped the whole way around and had no luck!
                    Debug.LogError("You've run out of space for flags to be tracked. That's over (2^32)-1 flags!");
                    rtn = invalid;
                    break;
                }
            }

            if (rtn != invalid)
            {
                intsReadyToConvert.Add(rtn);
            }

            return rtn;
        }

        public int FindValueOfString(string id)
        {
            id = id.ToUpper();
            object symbol = null;
            CheckForCompiledScript();
            if (enumType != null && System.Enum.TryParse(enumType, id, out symbol))
            {
                return (int)symbol;
            }
            else if (toAdd.HasKey(id))
            {
                return toAdd[id];
            }
            else
            {
                int newIdValue = FindValidUnusedFlagIDNumber();

                if (newIdValue != -1)
                {
                    toAdd.Add(id, newIdValue);
                }

                return newIdValue;
            }
        }

        public System.Type GetEnumType()
        {
            CheckForCompiledScript();
            return enumType;
        }
    }
}
