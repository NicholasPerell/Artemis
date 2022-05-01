using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Artemis Narrative System", menuName = "Artemis/Overall Narrative System")]
public class ArtemisNarrativeSystem : ScriptableObject
{
    //public SubtitleInk[] subtitleInk = null;
    public ArtemisFlagRepo narrativeFlags = null;

    public void OnApplicationQuit()
    {
    }

    public bool SendSubtitleIn(TextAsset inkFile)
    {
        return true; //subtitleInk[0].DeliverSubtitles(inkFile);
    }

    public bool MeetsConditions(ArtemisNarrativeDataPoint dataPoint)
    {
        string[] requiredTrue, requiredFalse;
        dataPoint.GetFlags(out requiredTrue, out requiredFalse);

        for (int i = 0; i < requiredTrue.Length; i++)
        {
            if (!narrativeFlags.GetFlag(requiredTrue[i]))
            {
                return false;
            }
        }

        for (int i = 0; i < requiredFalse.Length; i++)
        {
            if (narrativeFlags.GetFlag(requiredFalse[i]))
            {
                return false;
            }
        }

        return true;
    }

    public void Enqueue(ArtemisNarrativeDataPoint data)
    {
        //subtitleInk[0].Enqueue(data);
    }

    public void OnValidate()
    {
        
    }

}
