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
        [SerializeField]
        private SortedStrictDictionary<string, int> toAdd;
        [SerializeField]
        private SortedStrictList<int> intsReadyToConvert;
        [SerializeField]
        private List<int> toRemove;

        [SerializeField]
        private string fileLocation;
        [SerializeField]
        private string enumName;
        [SerializeField]
        private System.Type enumType = null;

        private const int INVALID = -1;

        public InternalSymbolCompiler(string _fileLocation, string _enumPrefix)
        {
            fileLocation = _fileLocation;
            enumName = _enumPrefix.ElementAt(0) + _enumPrefix.Substring(1).ToLower() + "ID";

            idsUsed = new SortedStrictList<int>();
            toAdd = new SortedStrictDictionary<string, int>();
            intsReadyToConvert = new SortedStrictList<int>();
            toRemove = new List<int>();

            CheckForCompiledScript();
        }

        ~InternalSymbolCompiler()
        {
            enumType = null;
        }

        public void SetLocation(string _fileLocation, string _enumPrefix)
        {
            fileLocation = _fileLocation;
            enumName = _enumPrefix.ElementAt(0) + _enumPrefix.Substring(1).ToLower() + "ID";
            CheckForCompiledScript();

        }

        void CheckForCompiledScript()
        {
            enumType = System.Type.GetType("Artemis."+enumName);
        }

        public void WriteFlagEnumScript()
        {
            string elementName;
            int elementInt;

            CheckForCompiledScript();

            if(enumType != null)
            {
                idsUsed.Clear();
                foreach (int e in Enum.GetValues(enumType))
                {
                    if (e != INVALID)
                    {
                        idsUsed.Add(e);
                    }
                }
            }

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
                elementName = toAdd.GetTupleAtIndex(i).Key; //TODO: Consider a GetKey or GetValue method instead?
                elementInt = toAdd.GetTupleAtIndex(i).Value;

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

            File.WriteAllText(path, stringBuilder.ToString());

            //Reset toAdd/Remove
            toAdd.Clear();
            toRemove.Clear();
            intsReadyToConvert.Clear();

            AssetDatabase.ImportAsset(relativePath);
            CheckForCompiledScript();
        }

        public void DeleteFlagEnumScript()
        {
            //Determine File Path
            string relativePath = fileLocation + enumName + ".cs";
            string path = "";

            path = Application.dataPath;
            path = path.Substring(0, path.Length - 6); //removes the "Assets"
            path += relativePath;

            //Delete unused script
            AssetDatabase.DeleteAsset(relativePath);
            enumType = null;
        }

        private int FindValidIDNumber()
        {
            int rtn;
            int start;

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

            while (rtn == INVALID || idsUsed.Has(rtn) || intsReadyToConvert.Has(rtn))
            {
                rtn++;
                if (rtn == int.MaxValue)
                {
                    rtn = int.MinValue;
                }

                if (rtn == start)
                {
                    //Looped the whole way around and had no luck!
                    UnityEngine.Debug.LogError("You've run out of space for flags to be tracked. That's over (2^32)-1 flags!");
                    rtn = INVALID;
                    break;
                }
            }

            if (rtn != INVALID)
            {
                intsReadyToConvert.Add(rtn);
            }

            return rtn;
        }

        public int FindValueOfString(string id)
        {
            object symbol = null;
            CheckForCompiledScript();
            if (enumType != null && System.Enum.TryParse(enumType, id, true, out symbol))
            {
                return (int)symbol;
            }
            else if (toAdd.HasKey(id))
            {
                return toAdd[id];
            }
            else
            {
                int newIDValue = FindValidIDNumber();

                if (newIDValue != -1)
                {
                    toAdd.Add(id, newIDValue);
                }

                return newIDValue;
            }
        }

        public void SetToRemove(string id)
        {
            object symbol = null;
            CheckForCompiledScript();
            if (enumType != null && System.Enum.TryParse(enumType, id, true, out symbol))
            {
                SetToRemove((int)symbol);
            }
        }

        public void SetToRemove(int id)
        {
            toRemove.Add(id);
        }

        public string FindNameOfValue(int id)
        {
            string result = id + "";

            CheckForCompiledScript();
            if (enumType != null)
            {
                result = Enum.GetName(enumType, id);
            }

            if(result == id + "")
            {
                int index = toAdd.IndexOfValue(id);
                if (index > -1)
                {
                    result = toAdd.GetTupleAtIndex(index).Key;
                }
            }

            return result;
        }

        public System.Type GetEnumType()
        {
            CheckForCompiledScript();
            return enumType;
        }
    }
}
