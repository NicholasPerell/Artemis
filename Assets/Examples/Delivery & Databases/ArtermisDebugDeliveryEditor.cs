using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ArtemisDebugDelivery))]
public class ArtemisDeliveryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtemisDebugDelivery system = (ArtemisDebugDelivery)target;
        if(GUILayout.Button("Parse CSV into database"))
        {
            system.DeliverySystemDatabase();
        }
    }
}

#endif