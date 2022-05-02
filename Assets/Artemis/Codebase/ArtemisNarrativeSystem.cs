using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Artemis Narrative System", menuName = "Artemis/Overall Narrative System")]
public class ArtemisNarrativeSystem : ScriptableSingleton<ArtemisNarrativeSystem>
{
    [SerializeField]
    private List<ArtemisFlag> flagsToKeep;

    [SerializeField]
    private ArtemisStringSortingDictionary<ArtemisFlag> stringToFlag;

    [SerializeField]
    private ArtemisFlagSortingDictionary<List<ArtemisPreDictionaryDeliverySystem>> flagConnections;

    public void Awake()
    {
    }

    public ArtemisFlag GetFlag(string name)
    {
        ArtemisFlag rtn = null;
        if(stringToFlag.ContainsKey(name))
        {
            rtn = stringToFlag[name];
        }
        return rtn;
    }

#if UNITY_EDITOR
    public ArtemisFlag ConnectFlag(string name, ArtemisPreDictionaryDeliverySystem connector)
    {
        ArtemisFlag rtn = null;
        stringToFlag ??= new ArtemisStringSortingDictionary<ArtemisFlag>();
        if (stringToFlag.ContainsKey(name))
        {
            rtn = stringToFlag[name];
            flagConnections ??= new ArtemisFlagSortingDictionary<List<ArtemisPreDictionaryDeliverySystem>>();
            flagConnections[rtn] ??= new List<ArtemisPreDictionaryDeliverySystem>();
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

            rtn = AssetDatabase.LoadAssetAtPath<ArtemisFlag>(GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");

            bool exists = rtn != null;

            if (!exists)
            {
                rtn = ScriptableObject.CreateInstance<ArtemisFlag>();
                rtn.SetValue(false);
                AssetDatabase.CreateAsset(rtn, GetContainingFolder() + "/" + GetFlagRepoFolderName() + "/" + name + ".asset");
            }

            stringToFlag.Add(name, rtn);
            flagConnections.Add(rtn, new List<ArtemisPreDictionaryDeliverySystem>());
            flagConnections[rtn].Add(connector);
        }
        EditorUtility.SetDirty(this);
        return rtn;
    }

    public void DisconnectFlag(string name, ArtemisPreDictionaryDeliverySystem connector)
    {
        ArtemisFlag rtn = null;
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
