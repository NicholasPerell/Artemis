using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadingInFlagCheck : MonoBehaviour
{
    [SerializeField]
    string toDealWith;

    // Start is called before the first frame update
    void Start()
    {
        dealWithFlagList(toDealWith);
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
        CriterionComparisonType endResult = CriterionComparisonType.INVALID;
        bool valid = false;
        string flag;
        string[] tmp;
        float a,b;

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
                tmp = input.Split('>');
                if (tmp.Length == 2)
                {
                    if (hasEq)
                    {
                        if (float.TryParse(tmp[1].Substring(1), out a))
                        {
                            flag = tmp[0].Trim();
                            valid = flag.IndexOf(' ') == -1
                                && flag.IndexOf('=') == -1;
                            if (valid)
                            {
                                Debug.Log(flag + " >= " + a);
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        if (float.TryParse(tmp[1], out a))
                        {
                            flag = tmp[0].Trim();
                            valid = flag.IndexOf(' ') == -1;
                            if (valid)
                            {
                                Debug.Log(flag + " > " + a);
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                else if (tmp.Length == 3)
                {
                    //TODO: ranges!
                    valid = false;

                }
                else
                {
                    valid = false;
                }
            }
        }
        else if (hasLess)
        {
            tmp = input.Split('<');
            valid = !hasEx && tmp.Length == 2;

            if (valid)
            {
                if (hasEq)
                {
                    if (float.TryParse(tmp[1].Substring(1), out a))
                    {
                        flag = tmp[0].Trim();
                        valid = flag.IndexOf(' ') == -1 
                            && flag.IndexOf('=') == -1;
                        if (valid)
                        {
                            Debug.Log(flag + " <= " + a);
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
                else
                {
                    if (float.TryParse(tmp[1], out a))
                    {
                        flag = tmp[0].Trim();
                        valid = flag.IndexOf(' ') == -1;
                        if (valid)
                        {
                            Debug.Log(flag + " < " + a);
                        }
                    }
                    else
                    {
                        valid = false;
                    }
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
                    valid = flag.IndexOf(' ') == -1;
                    if(valid)
                    {
                        Debug.Log(flag + " = " + a);
                    }
                }
                else
                {
                    valid = false;
                    //TODO: At some point, handle enums/symbols
                }
            }
        }
        else if (hasEx)
        {
            valid = input.IndexOf(' ') == -1
                && input.IndexOf('!') == 0
                && input.LastIndexOf('!') == 0;

            if (valid)
            {
                flag = input.Substring(1);
                Debug.Log(flag + " = FALSE");
            }
        }
        else
        {
            valid = input.IndexOf(' ') == -1;
            if (valid)
            {
                flag = input;
                Debug.Log(flag + " = TRUE");
            }
        }

        if (!valid)
        {
            Debug.LogError("\"" + input + "\" was found INVALID");
        }
    }
}
