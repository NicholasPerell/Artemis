using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadingInFlagCheck : MonoBehaviour
{
    [SerializeField]
    string toDealWith;

    [SerializeField]
    SortedStrictDictionary<ValveInternalSymbols,Criterion> rule;

    // Start is called before the first frame update
    void Start()
    {
        rule = new SortedStrictDictionary<ValveInternalSymbols, Criterion>();
        dealWithFlagList(toDealWith);
        TestSorting();
    }

    void dealWithFlagList(string str)
    {
        string[] inputs = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < inputs.Length; i++)
        {
            evalSpecificFlag(inputs[i]);
        }
    }

    void evalSpecificFlag(string input)
    {
        //Variable set-up
        CriterionComparisonType compareType = CriterionComparisonType.INVALID;
        bool valid = false;
        string flag = "";
        string[] tmp;
        float a = 0;
        float b = 0;

        //Trim input
        input = input.Trim();

        //Check input for key symbols
        bool hasLess = input.IndexOf('<') != -1;
        bool hasGreat = input.IndexOf('>') != -1;
        bool hasEq = input.IndexOf('=') != -1;
        bool hasEx = input.IndexOf('!') != -1;

        if (hasGreat)
        {
            valid = !hasEx && !hasLess;
            if (valid)
            {
                valid = IsValidLessGreat(input, '>', out a, out flag);
                if (!valid)
                {
                    tmp = input.Split('>');
                    valid = tmp.Length == 3;
                    if (valid)
                    {
                        bool hasLeftEq = tmp[1].IndexOf('=') != -1;
                        bool hasRightEq = tmp[2].IndexOf('=') != -1;

                        int leftStartIndex = hasLeftEq ? 1 : 0;
                        int rightStartIndex = hasRightEq ? 1 : 0;

                        if (float.TryParse(tmp[0], out a)
                            && float.TryParse(tmp[2].Substring(rightStartIndex), out b))
                        {
                            flag = tmp[1].Substring(leftStartIndex).Trim();
                            valid = IsFlagValid(flag);
                            if (valid)
                            {
                                //String
                                string output = a + " >";
                                if (hasLeftEq)
                                {
                                    output += "=";
                                }
                                output += " " + flag + " >";
                                if (hasRightEq)
                                {
                                    output += "=";
                                }
                                output += " " + b;
                                Debug.Log(output);

                                if(hasLeftEq)
                                {
                                    if(hasRightEq)
                                    {
                                        compareType = CriterionComparisonType.RANGE_CLOSED;
                                    }
                                    else
                                    {
                                        compareType = CriterionComparisonType.RANGE_CLOSED_OPEN;
                                    }
                                }
                                else
                                {
                                    if(hasRightEq)
                                    {
                                        compareType = CriterionComparisonType.RANGE_OPEN_CLOSED;
                                    }
                                    else
                                    {
                                        compareType = CriterionComparisonType.RANGE_OPEN;
                                    }
                                }
                            }
                        }
                        else
                        {
                            valid = false;
                        }

                    }
                }
                else
                {
                    if(hasEq)
                    {
                        compareType = CriterionComparisonType.GREATER_EQUAL;
                    }
                    else
                    {
                        compareType = CriterionComparisonType.GREATER;
                    }
                }
            }
        }
        else if (hasLess)
        {
            valid = IsValidLessGreat(input, '<', out a, out flag);
            if(valid)
            {
                if (hasEq)
                {
                    compareType = CriterionComparisonType.LESS_EQUAL;
                }
                else
                {
                    compareType = CriterionComparisonType.LESS;
                }
            }
        }
        else if (hasEq)
        {
            tmp = input.Split('=');

            valid = !hasEx && tmp.Length == 2;

            if (valid)
            {
                if (float.TryParse(tmp[1], out a)) //number value
                {
                    flag = tmp[0].Trim();
                    valid = IsFlagValid(flag);
                    if (valid)
                    {
                        compareType = CriterionComparisonType.EQUALS;
                        Debug.Log(flag + " = " + a);
                    }
                }
                else
                {
                    flag = tmp[0].Trim();
                    string enumPossibly = tmp[1].Trim();
                    valid = IsFlagValid(flag) && IsFlagValid(enumPossibly);
                    if(valid)
                    {
                        if (Enum.TryParse(enumPossibly, out ValveInternalSymbols internalSymbol))
                        {
                            a = (float)internalSymbol;
                            compareType = CriterionComparisonType.EQUALS;
                            Debug.Log(flag + " = " + enumPossibly);
                        }
                        else
                        {
                            Debug.LogError("Could not parse \"" + enumPossibly + "\" into a ValveInternalSymbols. Perhaps try to recompile?");
                            valid = false;
                        }
                    }
                }
            }
        }
        else if (hasEx)
        {
            valid = input.IndexOf('!') == 0
                && input.LastIndexOf('!') == 0;

            if (valid)
            {
                flag = input.Substring(1);
                a = 0;
                compareType = CriterionComparisonType.EQUALS;

                valid = IsFlagValid(flag);
                if (valid)
                {
                    Debug.Log(flag + " = FALSE");
                }
            }
        }
        else
        {
            valid = IsFlagValid(input);
            if (valid)
            {
                flag = input;
                a = 1;
                compareType = CriterionComparisonType.EQUALS;
                Debug.Log(flag + " = TRUE");
            }
        }

        if (valid)
        {
            ProcessCriterion(flag, compareType, a, b);
        }
        else
        {
            Debug.LogError("\"" + input + "\" was found INVALID");
        }
    }

    bool IsValidLessGreat(string input, char compareChar, out float a, out string flag)
    {
        bool valid;
        flag = "";
        a = 0;

        bool hasEq = input.IndexOf('=') != -1;
        bool hasEx = input.IndexOf('!') != -1;

        valid = !hasEx;

        if (valid)
        {
            string[] tmp = input.Split(compareChar);
            valid = tmp.Length == 2;
            if (valid)
            {
                int startIndex = hasEq ? 1 : 0;
                if (float.TryParse(tmp[1].Substring(1), out a))
                {
                    flag = tmp[0].Trim();
                    valid = IsFlagValid(flag);
                    if (valid)
                    {
                        string output = flag + " " + compareChar;
                        if(hasEq)
                        {
                            output += "=";
                        }
                        output += " " + a;

                        Debug.Log(output);
                    }
                }
                else
                {
                    valid = false;
                }
            }
        }

        return valid;
    }

    bool IsFlagValid(string flag)
    {
        bool valid = true;

        char[] arr = flag.ToCharArray();

        if (char.IsLetter(arr[0]))
        {
            foreach (char e in arr)
            {
                if (!char.IsLetterOrDigit(e) && e != '_')
                {
                    valid = false;
                    break;
                }
            }
        }
        else
        {
            valid = false;
        }

        return valid;
    }

    void ProcessCriterion(string flag, CriterionComparisonType comparisonType, float a, float b)
    {
        ValveInternalSymbols symbol;
        if (Enum.TryParse(flag, out symbol))
        {
            rule.Add(symbol,new Criterion(symbol,comparisonType,a,b));
        }
        else
        {
            Debug.LogError("Did not recognize flag \"" + flag + "\" in internal symbols. Perhaps recompile?");
        }
    }

    void TestSorting()
    {
        int index = 0;
        Criterion found;
        bool success;
        ValveInternalSymbols search;
        for(int i = 0; i <= (int)ValveInternalSymbols.END; i++)
        {
            search = (ValveInternalSymbols)i;
            success = rule.LinearSearch(search, ref index, out found);

            if(success)
            {
                Debug.Log("Found " + search + " (" + found.GetStringRepresentation() + "). Next index point is " + index + ".");
            }
            else
            {
                Debug.Log("No " + search + " found. Next index point is " + index + ".");
            }
        }
    }


}
