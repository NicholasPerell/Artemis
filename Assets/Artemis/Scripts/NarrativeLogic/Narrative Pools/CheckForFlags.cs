using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForFlags : MonoBehaviour
{
    [SerializeField]
    ArtemisFlagRepo flags;

    [SerializeField]
    string[] needTrue;

    [SerializeField]
    string[] needFalse;

    // Start is called before the first frame update
    void Start()
    {
        foreach(string e in needTrue)
        {
            if(!flags.GetFlag(e))
            {
                Destroy(this.gameObject);
            }
        }
        foreach (string e in needFalse)
        {
            if (flags.GetFlag(e))
            {
                Destroy(this.gameObject);
            }
        }
    }

}
