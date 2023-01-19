using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
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
    public struct Criterion : IComparable<Criterion>
    {
        /* left-hand side is the larger or equal, 
         * while right-hand side is the smaller or equal
         */
        [SerializeField]
        float lhs, rhs;

        [SerializeField]
        FlagID flagIdChecked;

        [SerializeField]
        CriterionComparisonType comparisonType;

        public Criterion(FlagID _stateChecked, CriterionComparisonType _comparisonType, float a, float b = 0)
        {
            flagIdChecked = _stateChecked;
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
            return flagIdChecked;
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
                    switch (Goddess.instance.GetFlagValueType(flagIdChecked))
                    {
                        case Flag.ValueType.BOOL:
                            rtn += flagIdChecked.ToString() + " = " + (lhs == 1).ToString().ToUpper();
                            break;
                        case Flag.ValueType.FLOAT:
                            rtn += flagIdChecked.ToString() + " = " + lhs;
                            break;
                        case Flag.ValueType.SYMBOL:
                            rtn += flagIdChecked.ToString() + " = " + (System.Enum)System.Enum.Parse(Goddess.instance.GetFlagSymbolType(flagIdChecked), "" + (Mathf.FloorToInt(lhs)));
                            break;
                    }
                    break;
                case CriterionComparisonType.GREATER:
                    rtn += flagIdChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
                case CriterionComparisonType.LESS:
                    rtn += flagIdChecked.ToString() + " < " + (lhs + float.Epsilon);
                    break;
                case CriterionComparisonType.GREATER_EQUAL:
                    rtn += flagIdChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.LESS_EQUAL:
                    rtn += flagIdChecked.ToString() + " <= " + (lhs);
                    break;
                case CriterionComparisonType.RANGE_OPEN:
                    rtn += (lhs + float.Epsilon) + " > " + flagIdChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
                case CriterionComparisonType.RANGE_CLOSED:
                    rtn += (lhs) + " >= " + flagIdChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.RANGE_OPEN_CLOSED:
                    rtn += (lhs + float.Epsilon) + " > " + flagIdChecked.ToString() + " >= " + (rhs);
                    break;
                case CriterionComparisonType.RANGE_CLOSED_OPEN:
                    rtn += (lhs) + " >= " + flagIdChecked.ToString() + " > " + (rhs - float.Epsilon);
                    break;
            }

            return rtn;
        }

        public int CompareTo(Criterion other)
        {
            return flagIdChecked.CompareTo(other.GetStateChecked());
        }
    }
}