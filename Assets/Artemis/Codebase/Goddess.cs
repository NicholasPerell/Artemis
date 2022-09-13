using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField]
        private StringSortingDictionary<Flag> stringToFlag;

        [SerializeField]
        private FlagSortingDictionary<List<PreDictionaryFletcher>> flagConnections;

        public void Awake()
        {
        }

        public Flag GetFlag(string name)
        {
            Flag rtn = null;
            if (stringToFlag.ContainsKey(name))
            {
                rtn = stringToFlag[name];
            }
            return rtn;
        }

#if UNITY_EDITOR
        public Flag ConnectFlag(string name, PreDictionaryFletcher connector)
        {
            Flag rtn = null;
            stringToFlag ??= new StringSortingDictionary<Flag>();
            if (stringToFlag.ContainsKey(name))
            {
                rtn = stringToFlag[name];
                flagConnections ??= new FlagSortingDictionary<List<PreDictionaryFletcher>>();
                flagConnections[rtn] ??= new List<PreDictionaryFletcher>();
                if (!flagConnections[rtn].Contains(connector))
                {
                    flagConnections[rtn].Add(connector);
                }
            }
            else
            {
                if (!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetFlagRepoFolderName()))
                {
                    AssetDatabase.CreateFolder(GetContainingFolder(), GetFlagRepoFolderName());
                }

                rtn = AssetDatabase.LoadAssetAtPath<Flag>(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");

                bool exists = rtn != null;

                if (!exists)
                {
                    rtn = ScriptableObject.CreateInstance<Flag>();
                    rtn.SetValue(false);
                    AssetDatabase.CreateAsset(rtn, GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");
                }

                stringToFlag.Add(name, rtn);
                flagConnections.Add(rtn, new List<PreDictionaryFletcher>());
                flagConnections[rtn].Add(connector);
            }
            EditorUtility.SetDirty(this);
            return rtn;
        }

        public void DisconnectFlag(string name, PreDictionaryFletcher connector)
        {
            Flag rtn = null;
            if (stringToFlag.ContainsKey(name))
            {
                rtn = stringToFlag[name];
                if (flagConnections[rtn].Contains(connector))
                {
                    flagConnections[rtn].Remove(connector);
                }

                if (flagConnections[rtn].Count == 0 && !flagsToKeep.Contains(rtn))
                {
                    stringToFlag.Remove(name);
                    flagConnections.Remove(rtn);
                    AssetDatabase.DeleteAsset(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");
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
#endif
    }
}
