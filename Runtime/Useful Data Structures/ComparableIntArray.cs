using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Perell.Artemis.Saving;
using System.IO;

namespace Perell.Artemis
{
    [System.Serializable]
    public struct ComparableIntArray : System.IComparable, IBinaryReadWriteable
    {
        [SerializeField]
        private int[] mArray;

        public ComparableIntArray(int[] array)
        {
            mArray = array;
        }

        public ComparableIntArray(Vector2Int array)
        {
            mArray = new int[2] { array.x, array.y };
        }

        private int CompareToSame(ComparableIntArray obj)
        {
            if (mArray.Length.CompareTo(obj.mArray.Length) != 0)
            {
                return mArray.Length.CompareTo(obj.mArray.Length);
            }

            for (int i = 0; i < mArray.Length; i++)
            {
                if (mArray[i].CompareTo(obj.mArray[i]) != 0)
                {
                    return mArray[i].CompareTo(obj.mArray[i]);
                }
            }

            return 0;
        }

        public int CompareTo(object obj)
        {
            return CompareToSame((ComparableIntArray)obj);
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            mArray = new int[binaryReader.ReadInt32()];
            for (int i = 0; i < mArray.Length; i++)
            {
                mArray[i] = binaryReader.ReadInt32();
            }
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(mArray.Length);
            for (int i = 0; i < mArray.Length; i++)
            {
                binaryWriter.Write(mArray[i]);
            }
        }
    }
}