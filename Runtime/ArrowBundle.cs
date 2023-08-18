using Perell.Artemis.Saving;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Perell.Artemis
{
    public class ArrowBundle : ScriptableObject, IBinaryReadWriteable
    {
        [SerializeField]
        private Arrow[] arrows;

        public Arrow[] GetArrows()
        {
            return arrows;
        }

        public static ArrowBundle CreateInstance(Arrow[] _arrows)
        {
            ArrowBundle created = ScriptableObject.CreateInstance<ArrowBundle>();
            created.arrows = _arrows;
            return created;
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.name);
            arrows.WriteToBinary(ref binaryWriter);
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            this.name = binaryReader.ReadString();
            arrows = binaryReader.ReadScriptableObjectArray<Arrow>();
        }
    }
}