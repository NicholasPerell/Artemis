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
    private ArtemisFlagSortingDictionary<List<ArtemisPreDictionarySystem>> flagConnections;

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
    public ArtemisFlag ConnectFlag(string name, ArtemisPreDictionarySystem connector)
    {
        ArtemisFlag rtn = null;
        stringToFlag ??= new ArtemisStringSortingDictionary<ArtemisFlag>();
        if (stringToFlag.ContainsKey(name))
        {
            rtn = stringToFlag[name];
            flagConnections ??= new ArtemisFlagSortingDictionary<List<ArtemisPreDictionarySystem>>();
            flagConnections[rtn] ??= new List<ArtemisPreDictionarySystem>();
            if (!flagConnections[rtn].Contains(connector))
            {
                flagConnections[rtn].Add(connector);
            }
        }
        else
        {
            if (!AssetDatabase.IsValidFolder(GetContainingFolder() + "/" + GetDataPointFolderName()))
            {
                AssetDatabase.CreateFolder(GetContainingFolder(), GetDataPointFolderName());
            }

            rtn = AssetDatabase.LoadAssetAtPath<ArtemisFlag>(GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + name + ".asset");

            bool exists = rtn != null;

            if (!exists)
            {
                rtn = ScriptableObject.CreateInstance<ArtemisFlag>();
                rtn.SetValue(false);
                AssetDatabase.CreateAsset(rtn, GetContainingFolder() + "/" + GetDataPointFolderName() + "/" + name + ".asset");
            }

            stringToFlag.Add(name, rtn);
            flagConnections.Add(rtn, new List<ArtemisPreDictionarySystem>());
            flagConnections[rtn].Add(connector);
        }
        EditorUtility.SetDirty(this);
        return rtn;
    }

    public void DisconnectFlag(string name, ArtemisPreDictionarySystem connector)
    {
        ArtemisFlag rtn = null;
        Debug.Log(name);
        Debug.Log("A");
        if (stringToFlag.ContainsKey(name))
        {
        Debug.Log("b");
            rtn = stringToFlag[name];
        Debug.Log("c");
            if (flagConnections[rtn].Contains(connector))
            {
        Debug.Log("d");
                flagConnections[rtn].Remove(connector);
        Debug.Log("e");
            }

        Debug.Log("f");
            if (flagConnections[rtn].Count == 0 && !flagsToKeep.Contains(rtn))
            {
        Debug.Log("g");
                stringToFlag.Remove(name);
        Debug.Log("h");
                flagConnections.Remove(rtn);
            }
        Debug.Log("i");
        }
        Debug.Log("j");

        EditorUtility.SetDirty(this);
    }

    private string GetContainingFolder()
    {
        string rtn = AssetDatabase.GetAssetPath(this);
        rtn = rtn.Substring(0, rtn.LastIndexOf('/'));
        return rtn;
    }

    private string GetDataPointFolderName()
    {
        return this.name + " Flag Repo";
    }
#endif
}
