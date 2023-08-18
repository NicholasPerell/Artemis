using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Perell.Artemis.Saving
{
    public interface IBinaryReadWriteable
    {
        void WriteToBinary(ref BinaryWriter binaryWriter);
        void ReadFromBinary(ref BinaryReader binaryReader);
    }
}