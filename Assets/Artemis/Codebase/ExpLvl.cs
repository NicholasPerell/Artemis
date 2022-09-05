using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpLvl : MonoBehaviour
{
    public int experience;

    public int Level
    {
        get { return experience / 750; }
    }

    public bool loops;
    public bool includeBundlesInLoop;
}
