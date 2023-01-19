using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Artemis;

[CustomEditor(typeof(ArtemisDebugExampleFletcher))]
public class ArtemisDeliveryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtemisDebugExampleFletcher system = (ArtemisDebugExampleFletcher)target;
        if(GUILayout.Button("Parse CSV into database"))
        {
            system.DeliverySystemDatabase();
        }
    }
}

#endif