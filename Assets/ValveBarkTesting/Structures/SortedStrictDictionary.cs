using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SortedStrictDictionary<K,V> where K : Enum
{
    [SerializeField]
    List<Tuple> list;

    //Created in place of KeyValuePairs to allow for serialization
    [System.Serializable]
    struct Tuple
    {
        public K Key;
        public V Value;

        public Tuple(K _key, V _value)
        {
            Key = _key;
            Value = _value;
        }
    }

    public SortedStrictDictionary()
    {
        list = new List<Tuple>();
    }

    public void Add(K key, V value)
    {
        int i;
        int tmp;
        Tuple element = new Tuple(key, value);

        for (i = 0; i < list.Count; i++)
        {
            tmp = key.CompareTo(list[i].Key);
            if (tmp == 0)
            {
                list[i] = element;
                break;
            }
            else if(tmp < 0)
            {
                list.Insert(i, element);
                break;
            }
        }

        if (i == list.Count)
        {
            list.Insert(i, element);
        }
    }

    //public bool Remove(K key)
    //{
    //    return list.Remove();
    //}

    public void RemoveAt(int i)
    {
        list.RemoveAt(i);
    }

    public void Clear()
    {
        list = new List<Tuple>();
    }

    public bool LinearSearch(K key, ref int startAt, out V foundValue)
    {
        foundValue = default(V);
        bool result = false;

        //Validate the startAt value is an index in the list
        if (startAt >= list.Count || startAt < 0)
        {
            startAt = -1;
        }
        else
        {
            int i;
            int tmp;
            for(i = startAt; i < list.Count; i++)
            {
                tmp = key.CompareTo(list[i].Key);
                if (tmp == 0)
                {
                    startAt = i+1;
                    foundValue = list[i].Value;
                    result = true;
                    break;
                }
                else if(tmp < 0)
                {
                    startAt = i;
                    break;
                }
            }

            if(i == list.Count)
            {
                startAt = -1;
            }
        }

        return result;
    }

    public int Count => list.Count;
}
