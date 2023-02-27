using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Artemis
{
    [System.Serializable]
    public struct ComparableIntArray : System.IComparable
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
    }
}