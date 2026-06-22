using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;

namespace Perell.Artemis
{
    public class FlagBundle : ScriptableObject
    {
        [HideInInspector]
        public SortedStrictDictionary<FlagID, FlagV2> flagsUsed;
        [SerializeField]
        public FlagV2[] tempFlags;

        public void Add(FlagV2 _flag)
        {
            if (_flag != null)
            {
                flagsUsed.Add(_flag.GetFlagID(), _flag);
            }
        }

        public void Remove(FlagV2 _flag)
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

        public FlagV2[] ToValueArray()
        {
            FlagV2[] rtn = new FlagV2[flagsUsed.Count];
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
            created.flagsUsed = new SortedStrictDictionary<FlagID, FlagV2>();
            return created;
        }
    }
}
