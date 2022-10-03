using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    public class Flag : ScriptableObject
    {
        public enum ValueType
        {
            FLOAT,
            BOOL,
            SYMBOL
        }

        [SerializeField]
        ValueType flagValueType = ValueType.BOOL;

        [SerializeField]
        float value;

        [SerializeField]
        bool boolValue;


        public void SetValue(float _value)
        {
            boolValue = _value != 0;
            value = _value;
        }

        public void SetValue(bool _value)
        {
            boolValue = _value;
            value = _value ? 1 : 0;
        }

        public bool GetBoolValue()
        {
            return boolValue;
        }

        public float GetValue()
        {
            return value;
        }

        public ValueType GetValueType()
        {
            return flagValueType;
        }

        public void SetValueType(ValueType type)
        {
            flagValueType = type;
        }
    }
}
