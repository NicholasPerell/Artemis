using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Flags", menuName = "Narrative/Game Flags")]
public class GameFlags : ScriptableObject
{
    [System.Serializable]
    public struct StringBoolPair
    {
        public string key;
        public bool value;
    }
    [SerializeField]
    StringBoolPair[] defaultFlags;
    [SerializeField]
    StringBoolPair[] currentFlags;
    SortedDictionary<string, bool> realFlags;

    [ContextMenu("Init")]
    void Init()
    {
        currentFlags = new StringBoolPair[defaultFlags.Length];
        defaultFlags.CopyTo(currentFlags,0);
        Refresh();
    }

    void Refresh()
    {
        realFlags = new SortedDictionary<string, bool>();
        realFlags.Clear();
        foreach (StringBoolPair e in currentFlags)
        {
            realFlags.Add(e.key, e.value);
        }
    }

    public bool GetFlag(string flagKey)
    {
        if(realFlags == null || realFlags.Count != currentFlags.Length)
        {
            Refresh();
        }

        bool result = false;

        if(realFlags.ContainsKey(flagKey))
        {
            result = realFlags[flagKey];
        }

        return result;
    }

    void SetFlag(string flagKey,bool flagValue)
    {
        if (realFlags == null || realFlags.Count != currentFlags.Length)
        {
            Refresh();
        }


        if (realFlags.ContainsKey(flagKey))
        {
            realFlags[flagKey] = flagValue;

            for (int i = 0; i < currentFlags.Length; i++)
            {
                if (currentFlags[i].key == flagKey)
                {
                    currentFlags[i].value = flagValue;
                }
            }
        }
    }

    public void SetFlagTrue(string flagKey)
    {
        SetFlag(flagKey, true);
    }

    public void SetFlagFalse(string flagKey)
    {
        SetFlag(flagKey, false);
    }
}
