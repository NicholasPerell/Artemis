using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;
using UnityEngine.Events;

namespace Perell.Artemis
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

        public event UnityAction OnValueChanged;

        public void SetValue(float _value)
        {
            float formerValue = value;
            value = _value;
            if(formerValue != value)
            {
                OnValueChanged?.Invoke();
            }
        }

        public void SetValue(bool _value)
        {
            SetValue(_value ? 1 : 0);
        }

        public void SetValue(string _value)
        {
            object parsed;
            if (GetValueType() == ValueType.SYMBOL && System.Enum.TryParse(GetSymbolType(), _value, true, out parsed))
            {
                SetValue((int)parsed);
            }
            else if(GetValueType() == ValueType.SYMBOL)
            {
                Debug.LogError(name + ": " + _value + "not recognized as a value for " + GetFlagID().ToString() + ". Setting to INVALID."); ;
                SetValue((int)FlagID.INVALID);
            }
            else
            {
                Debug.LogError(name + " could not set value to: " + _value);
            }
        }

        public float GetValue()
        {
            return value;
        }

        public FlagID GetFlagID()
        {
            return flagID;
        }

        public void InitFlag(FlagID _flagID)
        {
            if(flagID == FlagID.INVALID && _flagID != FlagID.INVALID)
            {
                SetFlagID(_flagID);
                SetValueType(Goddess.instance.GetFlagValueType(_flagID));
                if (GetValueType() == ValueType.SYMBOL)
                {
                    SetSymbolType(Goddess.instance.GetFlagSymbolType(_flagID));
                }
            }
        }

        public void ValidateFlag()
        {
           if (flagID.ToString() == ((int)flagID).ToString())
            {
                SetFlagID(FlagID.INVALID);
            }
        }

        private void SetFlagID(FlagID _flagID)
        {
            flagID = _flagID;
        }

        public ValueType GetValueType()
        {
            return flagValueType;
        }

        private void SetValueType(ValueType type)
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

        private void SetSymbolType(Type type)
        {
            symbolType = type;
            symbolTypeName = type.AssemblyQualifiedName;
        }
    }
}
