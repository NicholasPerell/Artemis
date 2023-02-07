using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    public class Flag : ScriptableObject
    {
        public enum ValueType
        {
            INVALID = -1,
            FLOAT,
            BOOL,
            SYMBOL
        }

        [SerializeField]
        System.Type symbolType;

        [SerializeField]
        string symbolTypeName;

        [SerializeField]
        ValueType flagValueType = ValueType.INVALID;

        [SerializeField]
        FlagID flagID = FlagID.INVALID;

        [SerializeField]
        float value = 0;

        public void SetValue(float _value)
        {
            value = _value;
        }

        public void SetValue(bool _value)
        {
            value = _value ? 1 : 0;
        }

        public float GetValue()
        {
            return value;
        }

        public FlagID GetFlagID()
        {
            return flagID;
        }

        public void SetFlagID(FlagID _flagID)
        {
            flagID = _flagID;
        }

        public ValueType GetValueType()
        {
            return flagValueType;
        }

        public void SetValueType(ValueType type)
        {
            flagValueType = type;
        }

        public System.Type GetSymbolType()
        {
            if (symbolType == null)
            {
                symbolType = System.Type.GetType(symbolTypeName);
            }

            if (symbolType == null)
            {
                symbolType = typeof(ValueType);
            }

            return symbolType;
        }

        public void SetSymbolType(Type type)
        {
            symbolType = type;
            symbolTypeName = type.FullName;
        }
    }
}
