using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArtemisSortingDictionaries<TValue>
{
    [System.Serializable]
    public struct KeyValuePair<TTKey, TTValue>
    {
        public KeyValuePair(TTKey key, TTValue value)
        {
            Key = key;
            Value = value;
        }

        public TTKey Key;
        public TTValue Value;
    }

    [SerializeField]
    List<KeyValuePair<string, TValue>> list;

    public ArtemisSortingDictionaries()
    {
        list = new List<KeyValuePair<string, TValue>>();
    }

    public void Add(string key, TValue value)
    {
        if (list.Count > 0)
        {
            int index = 0;
            while (index < list.Count && list[index].Key.CompareTo(key) < 0)
            {
                index++;
            }
            list.Insert(index, new KeyValuePair<string, TValue>(key, value));
        }
        else
        {
            list.Add(new KeyValuePair<string, TValue>(key, value));
        }
    }

    public bool ContainsKey(string key)
    {
        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int index = min + (max - min) / 2;
            if (list[index].Key == key)
            {
                return true;
            }
            else if (list[index].Key.CompareTo(key) < 0)
            {
                min = index + 1;
            }
            else
            {
                max = index - 1;
            }
        }

        return false;
    }

    public TValue this[string id]
    {
        get
        {
            int min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int index = min + (max - min) / 2;
                if (list[index].Key == id)
                {
                    return list[index].Value;
                }
                else if (list[index].Key.CompareTo(id) < 0)
                {
                    min = index + 1;
                }
                else
                {
                    max = index - 1;
                }
            }

            return default(TValue);
        }

        private set { }
    }

    public List<string> GetKeyList()
    {
        List<string> rtn = new List<string>();
        foreach(KeyValuePair<string, TValue> e in list)
        {
            rtn.Add(e.Key);
        }
        return rtn;
    }
}
