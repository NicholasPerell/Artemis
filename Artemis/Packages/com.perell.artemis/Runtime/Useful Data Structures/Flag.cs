using System;
using System.Collections.Generic;
using System.IO;
using Perell.Artemis.Saving;
using UnityEngine;

namespace Perell.Artemis
{
    [Serializable]
    public struct Flag : IBinaryReadWriteable
    {
        [SerializeField]
        private Guid32 _key;
        public Guid32 Key => _key;

        [SerializeField]
        private float _value;
        public float Value { get => _value; set => _value = value; }

        public Flag(Guid32 key, bool value) : this(key, value ? 1 : 0)
        { }

        public Flag(Guid32 key, float value)
        {
            _key = key;
            _value = value;
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            _key = binaryReader.ReadGuid32();
            _value = binaryReader.ReadSingle();
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_key);
            binaryWriter.Write(_value);
        }
    }

    [Serializable]
    public class Query : IBinaryReadWriteable
    {
        private List<Flag> _flags;
        private Func<Query> _findUpdated;

        internal Query()
        {
            _flags = new();
            _findUpdated = () => this;
        }

        private Query(List<Flag> flags)
        {
            _flags = new(flags);
            _flags.Sort((a, b) => a.Key.CompareTo(b.Key));
            _findUpdated = () => this;
        }

        private Query(List<Flag> flags, Func<Query> findUpdated)
        {
            _flags = new(flags);
            _flags.Sort((a, b) => a.Key.CompareTo(b.Key));
            _findUpdated = findUpdated;
        }

        public static Query Storage() => new(new());
        public static Query Procedural(Func<Query> findUpdated) => new(new(), findUpdated);

        public bool LinearSearch(ref int index, Guid32 key, out Flag flag)
        {
            flag = default;

            for (; index >= 0 && index < _flags.Count; index++)
            {
                var item = _flags[index];

                if (item.Key == key)
                {
                    flag = item;
                    return true;
                }
                else if (item.Key > key)
                {
                    return false;
                }
            }

            return false;
        }

        public void Add(Flag flag)
        {
            int i = 0;

            for (; i < _flags.Count; i++)
            {
                if (flag.Key == _flags[i].Key)
                {
                    throw new Exception($"{nameof(Query)}.{nameof(Add)} failed; flag key {flag.Key} already found.");
                }
                else if (flag.Key < _flags[i].Key)
                {
                    break;
                }
            }

            _flags.Insert(i, flag);
        }

        public void Set(Guid32 key, float value)
        {
            int i = 0;

            for (; i < _flags.Count; i++)
            {
                if (key == _flags[i].Key)
                {
                    var flag = _flags[i];
                    flag.Value = value;
                    _flags[i] = flag;
                    return;
                }
                else if (key > _flags[i].Key)
                {
                    throw new Exception($"{nameof(Query)}.{nameof(Set)} failed; flag key {key} not found.");
                }
            }

            throw new Exception($"{nameof(Query)}.{nameof(Set)} failed; flag key {key} not found.");
        }

        public void AddOrSet(Guid32 key, float value)
        {
            int i = 0;

            for (; i < _flags.Count; i++)
            {
                if (key == _flags[i].Key)
                {
                    var flag = _flags[i];
                    flag.Value = value;
                    _flags[i] = flag;
                    return;
                }
                else if (key < _flags[i].Key)
                {
                    break;
                }
            }

            _flags.Insert(i, new(key, value));
        }

        public Query GetRefreshed() => _findUpdated();

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            _flags = binaryReader.ReadList<Flag>();
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_flags);
        }
    }
}
