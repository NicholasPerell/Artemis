using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
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
                flagsUsed.Add(_flag.GetFlagId(), _flag);
            }
        }

        public void Remove(Flag _flag)
        {
            if (_flag != null)
            {
                if (flagsUsed.HasValue(_flag))
                {
                    flagsUsed.Remove(_flag.GetFlagId());
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
                rtn[i] = flagsUsed[i].Value;
            }
            return rtn;
        }

        [ContextMenu("Clear Entire List")]
        private void Clear()
        {
            flagsUsed.Clear();
        }
    }
}
