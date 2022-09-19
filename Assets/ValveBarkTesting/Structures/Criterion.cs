using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateValueType
{
    FLOAT,
    BOOL,
    SYMBOL
}

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

public class Criterion : ScriptableObject
{
    /* left-hand side is the larger or equal, 
     * while right-hand side is the smaller or equal
     */
    [SerializeField]
    float lhs, rhs;

    [SerializeField]
    ValveInternalSymbols stateChecked;

    [SerializeField]
    CriterionComparisonType comparisonType;

    public Criterion(ValveInternalSymbols _stateChecked, CriterionComparisonType _comparisonType, float a, float b = 0)
    {
        stateChecked = _stateChecked;
        comparisonType = _comparisonType;

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

    public ValveInternalSymbols GetStateChecked()
    {
        return stateChecked;
    }

    public CriterionComparisonType GetComparisonType()
    {
        return comparisonType;
    }
}
