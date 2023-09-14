using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Example.DebugConsole.Editor
{
    [CustomEditor(typeof(DebugExampleInitializer))]
    public class DebugExampleInitializerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DebugExampleInitializer initializer = (DebugExampleInitializer)target;

            DrawDefaultInspector();
            if (GUILayout.Button("Initialize"))
            {
                initializer.InitializeExample();
            }
            if (GUILayout.Button("Deinitialize"))
            {
                initializer.DeinitializeExample();
            }
        }
    }
}