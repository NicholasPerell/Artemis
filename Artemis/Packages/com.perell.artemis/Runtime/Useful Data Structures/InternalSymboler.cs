using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Perell.Artemis.Saving;
using System.IO;
using Perell.Artemis.Debugging;
using UnityEditor.TerrainTools;

namespace Perell.Artemis
{
    [System.Serializable]
    public class InternalSymboler : ISerializationCallbackReceiver, IBinaryReadWriteable
    {
        [SerializeField]
        byte[] _guids;

        [SerializeField]
        string[] _names;

        [SerializeField]
        public Debugging.Logger _logger = new("InternalSymboler", Color.cyan) { Level = LogLevel.Info };

        private Dictionary<Guid, string> _toName = new();
        private Dictionary<string, Guid> _toGuid = new();

        public void OnAfterDeserialize()
        {
            _toName = new();
            _toGuid = new();

            for (int i = 0; i < _names.Length; i++)
            {
                var name = _names[i];
                var guid = new Guid(_guids.AsSpan(i * 16, 16));
                _toGuid[name] = guid;
                _toName[guid] = name;
            }
        }

        public void OnBeforeSerialize()
        {
            _names = new string[_toName.Count];
            _guids = new byte[_toName.Count * 16];

            var i = 0;
            foreach (var (guid, name) in _toName)
            {
                var bytes = guid.ToByteArray();

                for (var j = 0; j < 16; j++)
                {
                    _guids[i * 16 + j] = bytes[j];
                }

                _names[i] = name;
                i++;
            }
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            var count = binaryReader.ReadInt32();
            _guids = binaryReader.ReadBytes(count);
            _names = new string[binaryReader.ReadInt32()];

            for (int i = 0; i < _names.Length; i++)
            {
                _names[i] = binaryReader.ReadString();
            }
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_guids.Length);
            binaryWriter.Write(_guids);
            binaryWriter.Write(_names.Length);

            for (int i = 0; i < _names.Length; i++)
            {
                binaryWriter.Write(_names[i]);
            }
        }

        public bool HasName(string name)
        {
            return _toGuid.ContainsKey(name);
        }

        public bool TryGetGuid(string name, out Guid guid)
        {
            return _toGuid.TryGetValue(name, out guid);
        }

        public Guid GetOrAddName(string name)
        {
            if (!TryGetGuid(name, out var guid))
            {
                guid = GenerateName(name);
            }

            return guid;
        }

        private Guid GenerateName(string name)
        {
            var guid = Guid.Empty;

            while (guid != Guid.Empty && !_toName.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }

            return guid;
        }
    }

}