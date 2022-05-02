using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ArtemisDebugDeliverySystem))]
public class ArtemisDeliveryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtemisDebugDeliverySystem system = (ArtemisDebugDeliverySystem)target;
        if(GUILayout.Button("Parse CSV into database"))
        {
            system.DeliverySystemDatabase();
        }
    }
}

#endif