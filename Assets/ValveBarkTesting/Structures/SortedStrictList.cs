using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SortedStrictList<T> where T : IComparable
{
    [SerializeField]
    List<T> list;

    public SortedStrictList()
    {
        list = new List<T>();
    }

    public void Add(T value)
    {
        int i;
        int tmp;

        for (i = 0; i < list.Count; i++)
        {
            tmp = value.CompareTo(list[i]);
            if (tmp == 0)
            {
                list[i] = value;
                break;
            }
            else if (tmp < 0)
            {
                list.Insert(i, value);
                break;
            }
        }

        if (i == list.Count)
        {
            list.Insert(i, value);
        }
    }

    public bool Has(T element)
    {
        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int index = min + (max - min) / 2;
            if (list[index].CompareTo(element) == 0)
            {
                return true;
            }
            else if (list[index].CompareTo(element) < 0)
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

    public void RemoveAt(int i)
    {
        list.RemoveAt(i);
    }

    public void Remove(T element)
    {
        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int index = min + (max - min) / 2;
            if (list[index].CompareTo(element) == 0)
            {
                list.RemoveAt(index);
                break;
            }
            else if (list[index].CompareTo(element) < 0)
            {
                min = index + 1;
            }
            else
            {
                max = index - 1;
            }
        }
    }

    public void Clear()
    {
        list = new List<T>();
    }

    public T this[int key]
    {
        get { return list[key]; }
    }

    public int Count => list.Count;
}
