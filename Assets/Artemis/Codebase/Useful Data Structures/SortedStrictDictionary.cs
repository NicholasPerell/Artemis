using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SortedStrictDictionary<K,V> where K : IComparable
{
    [SerializeField]
    List<Tuple> list = new List<Tuple>();

    //Created in place of KeyValuePairs to allow for serialization
    [System.Serializable]
    public struct Tuple : IComparable<Tuple>
    {
        public K Key;
        public V Value;

        public Tuple(K _key, V _value)
        {
            Key = _key;
            Value = _value;
        }

        public int CompareTo(Tuple other)
        {
            return Key.CompareTo(other.Key);
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

    /// <summary>
    /// Cleans out any null Keys or Values
    /// </summary>
    public void Clean()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].Value.Equals(null) || list[i].Key.Equals(null))
            {
                list.RemoveAt(i);
            }
        }
    }

    public void Remove(K key)
    {
        V tempVal = default(V);
        int foundIndex = list.BinarySearch(new Tuple(key, tempVal));
        if (foundIndex > -1)
        {
            list.RemoveAt(foundIndex);
        }
    }

    public void RemoveAt(int i)
    {
        list.RemoveAt(i);
    }

    public void Clear()
    {
        list = new List<Tuple>();
    }


    public Tuple this[int key]
    {
        get { return list[key]; }
    }

    public V this[K key]
    {
        get 
        {
            V value = default(V);
            int foundIndex = list.BinarySearch(new Tuple(key, value));

            if(foundIndex > -1)
            {
                value = list[foundIndex].Value;
            }

            return value; 
        }

        set
        {
            int foundIndex = list.BinarySearch(new Tuple(key, value));
            if (foundIndex > -1)
            {
                list[foundIndex] = new Tuple(key,value);
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public bool HasKey(K key)
    {
        V tempVal = default(V);
        return list.BinarySearch(new Tuple(key, tempVal)) > -1;
    }

    public bool HasValue(V value)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(value.Equals(list[i].Value))
            {
                return true;
            }
        }
        return false;
    }

    public bool LinearSearch(K key, out V foundValue)
    {
        int tempInt = 0;
        return LinearSearch(key, ref tempInt, out foundValue);
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

    public List<K> GetKeyList()
    {
        List<K> keyList = new List<K>();
        foreach(Tuple e in list)
        {
            keyList.Add(e.Key);
        }
        return keyList;
    }
}
