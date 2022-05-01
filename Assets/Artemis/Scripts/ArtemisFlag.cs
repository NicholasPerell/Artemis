using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemisFlag : ScriptableObject
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
