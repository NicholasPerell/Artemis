using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;

namespace Perell.Artemis
{
    public class FlagBundle : ScriptableObject
    { 
        [HideInInspector]
        public SortedStrictDictionary<FlagID, Flag> flagsUsed;
        [SerializeField]
        public Flag[] tempFlags;

        public void Add(Flag _flag)
        {
            if(_flag != null)
            {
                flagsUsed.Add(_flag.GetFlagID(), _flag);
            }
        }

        public void Remove(Flag _flag)
        {
            if (_flag != null)
            {
                if (flagsUsed.HasValue(_flag))
                {
                    flagsUsed.Remove(_flag.GetFlagID());
                }
            }
            else
            {
                flagsUsed.Clean();
            }
        }

        public Flag[] ToValueArray()
        {
            Flag[] rtn = new Flag[flagsUsed.Count];
            for (int i = 0; i < flagsUsed.Count; i++)
            {
                rtn[i] = flagsUsed.GetTupleAtIndex(i).Value;
            }
            return rtn;
        }

        [ContextMenu("Clear Entire List")]
        private void Clear()
        {
            flagsUsed.Clear();
        }

        public static FlagBundle CreateInstance()
        {
            FlagBundle created = ScriptableObject.CreateInstance<FlagBundle>();
            created.flagsUsed = new SortedStrictDictionary<FlagID, Flag>();
            return created;
        }
    }
}
