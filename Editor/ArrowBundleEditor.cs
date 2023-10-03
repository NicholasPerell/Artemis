using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ArrowBundle))]
    public class ArrowBundleEditor : IconObjectEditor
    {
        SerializedProperty arrowsProperty;

        private void OnEnable()
        {
            arrowsProperty = serializedObject.FindProperty("arrows");
        }

        public override void OnInspectorGUI()
        {
            ArrowBundle arrowBundle = (ArrowBundle)target;

            SetIcon("ArrowBundle");

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUILayout.PropertyField(arrowsProperty, new GUIContent("Arrows"));

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(arrowBundle);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}
