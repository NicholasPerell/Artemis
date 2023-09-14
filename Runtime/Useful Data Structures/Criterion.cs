using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;
using Perell.Artemis.Saving;
using System.IO;

namespace Perell.Artemis
{
    public enum CriterionComparisonType
    {
        INVALID = -1,
        EQUALS,
        GREATER,
        LESS,
        GREATER_EQUAL,
        LESS_EQUAL,
        RANGE_OPEN, //ex: {x | a > x > b}
        RANGE_CLOSED, //ex: {x | a >= x >= b}
        RANGE_OPEN_CLOSED, //ex: {x | a > x >= b}
        RANGE_CLOSED_OPEN //ex: {x | a >= x > b}
    }

    [System.Serializable]
    public struct Criterion : IComparable<Criterion>, IBinaryReadWriteable
    {
        /* left-hand side is the larger or equal, 
         * while right-hand side is the smaller or equal
         */
        [SerializeField]
        float lhs, rhs;

        [SerializeField]
        FlagID flagIDChecked;

        [SerializeField]
        CriterionComparisonType comparisonType;

        public Criterion(FlagID _stateChecked, CriterionComparisonType _comparisonType, float a, float b = 0)
        {
            flagIDChecked = _stateChecked;
            comparisonType = _comparisonType;

            lhs = 0;
            rhs = 0;

            switch (comparisonType)
            {
                case CriterionComparisonType.EQUALS:
                    lhs = a;
                    rhs = a;
                    break;
                case CriterionComparisonType.LESS_EQUAL:
                    lhs = a;
                    rhs = float.NegativeInfinity;
                    break;
                case CriterionComparisonType.GREATER_EQUAL:
                    lhs = float.PositiveInfinity;
                    rhs = a;
                    break;
                case CriterionComparisonType.GREATER:
                    lhs = float.PositiveInfinity;
                    rhs = a + float.Epsilon;
                    break;
                case CriterionComparisonType.LESS:
                    lhs = a - float.Epsilon;
                    rhs = float.NegativeInfinity;
                    break;
                case CriterionComparisonType.RANGE_OPEN:
                    lhs = a - float.Epsilon;
                    rhs = b + float.Epsilon;
                    break;
                case CriterionComparisonType.RANGE_CLOSED:
                    lhs = a;
                    rhs = b;
                    break;
                case CriterionComparisonType.RANGE_OPEN_CLOSED:
                    lhs = a - float.Epsilon;
                    rhs = b;
                    break;
                case CriterionComparisonType.RANGE_CLOSED_OPEN:
                    lhs = a;
                    rhs = b + float.Epsilon;
                    break;
            }
        }

        public bool Compare(float x)
        {
            return lhs >= x && x >= rhs;
        }

        public FlagID GetStateChecked()
        {
            return flagIDChecked;
        }

        public CriterionComparisonType GetComparisonType()
        {
            return comparisonType;
        }

        public string GetStringRepresentation()
        {
            string rtn = "";

            switch (comparisonType)
            {
                case CriterionComparisonType.EQUALS:
                    switch (Goddess.instance.GetFlagValueType(flagIDChecked))
                    {
                        case Flag.ValueType.BOOL:
                            rtn += flagIDChecked.ToString() + " = " + (lhs == 1).ToString().ToUpper();
                            break;
                        case Flag.ValueType.FLOAT:
                            rtn += flagIDChecked.ToString() + " = " + lhs;
                            break;
                        case Flag.ValueType.SYMBOL:
                            rtn += flagIDChecked.ToString() + " = " + (System.Enum)System.Enum.Parse(Goddess.instance.GetFlagSymbolType(flagIDChecked), "" + (Mathf.FloorToInt(lhs)));
                            break;
                    }
                    break;
                case CriterionComparisonType.GREATER:
                    rtn += flagIDChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
                case CriterionComparisonType.LESS:
                    rtn += flagIDChecked.ToString() + " < " + (lhs + float.Epsilon);
                    break;
                case CriterionComparisonType.GREATER_EQUAL:
                    rtn += flagIDChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.LESS_EQUAL:
                    rtn += flagIDChecked.ToString() + " <= " + (lhs);
                    break;
                case CriterionComparisonType.RANGE_OPEN:
                    rtn += (lhs + float.Epsilon) + " > " + flagIDChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
                case CriterionComparisonType.RANGE_CLOSED:
                    rtn += (lhs) + " >= " + flagIDChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.RANGE_OPEN_CLOSED:
                    rtn += (lhs + float.Epsilon) + " > " + flagIDChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.RANGE_CLOSED_OPEN:
                    rtn += (lhs) + " >= " + flagIDChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
            }

            return rtn;
        }

        public int CompareTo(Criterion other)
        {
            return flagIDChecked.CompareTo(other.GetStateChecked());
        }

        public float getA()
        {
            return lhs;
        }

        public float getB()
        {
            return rhs;
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(lhs);
            binaryWriter.Write(rhs);
            binaryWriter.Write((int)flagIDChecked);
            binaryWriter.Write((int)comparisonType);
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            lhs = binaryReader.ReadSingle();
            rhs = binaryReader.ReadSingle();
            flagIDChecked = (FlagID)binaryReader.ReadInt32();
            comparisonType = (CriterionComparisonType)binaryReader.ReadInt32();
        }
    }
}