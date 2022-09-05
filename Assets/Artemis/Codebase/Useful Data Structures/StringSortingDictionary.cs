using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    [System.Serializable]
    public class StringSortingDictionary<TValue>
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

        public StringSortingDictionary()
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
            foreach (KeyValuePair<string, TValue> e in list)
            {
                rtn.Add(e.Key);
            }
            return rtn;
        }

        public void Remove(string id)
        {
            int min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int index = min + (max - min) / 2;
                if (list[index].Key == id)
                {
                    list.RemoveAt(index);
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
        }
    }


    //When I get a better hold on how to make the key type something I can use for comparison I'll be sure to make the above and below into one template
    [System.Serializable]
    public class FlagSortingDictionary<TValue>
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
        List<KeyValuePair<Flag, TValue>> list;

        public FlagSortingDictionary()
        {
            list = new List<KeyValuePair<Flag, TValue>>();
        }

        public void Add(Flag key, TValue value)
        {
            if (list.Count > 0)
            {
                int index = 0;
                while (index < list.Count && list[index].Key.name.CompareTo(key.name) < 0)//.CompareTo(key) < 0)
                {
                    index++;
                }
                list.Insert(index, new KeyValuePair<Flag, TValue>(key, value));
            }
            else
            {
                list.Add(new KeyValuePair<Flag, TValue>(key, value));
            }
        }

        public bool ContainsKey(Flag key)
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
                else if (list[index].Key.name.CompareTo(key.name) < 0)
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

        public TValue this[Flag id]
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
                    else if (list[index].Key.name.CompareTo(id.name) < 0)
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

            set
            {
                int min = 0;
                int max = list.Count - 1;
                while (min <= max)
                {
                    int index = min + (max - min) / 2;
                    if (list[index].Key == id)
                    {
                        list[index] = new KeyValuePair<Flag, TValue>(id, value);
                    }
                    else if (list[index].Key.name.CompareTo(id.name) < 0)
                    {
                        min = index + 1;
                    }
                    else
                    {
                        max = index - 1;
                    }
                }
            }
        }

        public List<Flag> GetKeyList()
        {
            List<Flag> rtn = new List<Flag>();
            foreach (KeyValuePair<Flag, TValue> e in list)
            {
                rtn.Add(e.Key);
            }
            return rtn;
        }

        public void Remove(Flag id)
        {
            int min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int index = min + (max - min) / 2;
                if (list[index].Key == id)
                {
                    list.RemoveAt(index);
                }
                else if (list[index].Key.name.CompareTo(id.name) < 0)
                {
                    min = index + 1;
                }
                else
                {
                    max = index - 1;
                }
            }
        }
    }
}
