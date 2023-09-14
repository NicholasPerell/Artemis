using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Perell.Artemis.Saving
{
    [System.Serializable]
    public struct BitFlagList : IBinaryReadWriteable
    {
        [SerializeField]
        private int[] data;

        const int bitsPerElement = 31;

        public void SetFlag(int index, bool value)
        {
            if (index < 0)
            {
                return;
            }

            int elementIndex = index / bitsPerElement;
            int bitIndex = index % bitsPerElement;

            if (data == null)
            {
                data = new int[elementIndex + 1];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = 0;
                }
            }
            else if (data.Length <= elementIndex)
            {
                int[] oldData = data;
                data = new int[elementIndex + 1];
                for (int i = 0; i < oldData.Length; i++)
                {
                    data[i] = oldData[i];
                }
                for (int i = oldData.Length; i < data.Length; i++)
                {
                    data[i] = 0;
                }
            }

            if (value)
            {
                data[elementIndex] |= 1 << bitIndex;
            }
            else
            {
                data[elementIndex] &= ~(1 << bitIndex);
            }
        }

        public bool GetFlag(int index)
        {
            if (index < 0)
            {
                return false;
            }

            int value = 0;

            int elementIndex = index / bitsPerElement;
            int bitIndex = index % bitsPerElement;

            if (data == null)
            {
                return false;
            }
            else if (data.Length <= elementIndex)
            {
                return false;
            }
            else
            {
                value = data[elementIndex] & (1 << bitIndex);
            }

            return value != 0;
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            data = new int[binaryReader.ReadInt32()];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = binaryReader.ReadInt32();
            }
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                binaryWriter.Write(data[i]);
            }
        }
    }

    [System.Serializable]
    public struct BitFlagList<E> : IBinaryReadWriteable where E : System.Enum
    {
        [SerializeField]
        BitFlagList bits;

        public void SetFlag(E index, bool value)
        {
            bits.SetFlag(System.Convert.ToInt32(index), value);
        }

        public bool GetFlag(E index)
        {
            return bits.GetFlag(System.Convert.ToInt32(index));
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            bits.ReadFromBinary(ref binaryReader);
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            bits.WriteToBinary(ref binaryWriter);
        }
    }
}