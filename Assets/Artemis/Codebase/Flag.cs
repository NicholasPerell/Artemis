using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    public class Flag : ScriptableObject
    {
        [SerializeField]
        bool value;

        public void SetValue(bool _value)
        {
            value = _value;
        }

        public bool GetValue()
        {
            return value;
        }
    }
}
