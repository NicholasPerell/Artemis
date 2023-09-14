using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Example.InkIntegration.Editor
{
    [CustomEditor(typeof(InkExampleInitializer))]
    public class InkExampleInitializerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            InkExampleInitializer initializer = (InkExampleInitializer)target;

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