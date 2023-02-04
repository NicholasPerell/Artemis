using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ArrowBundle))]
    public class ArrowBundleEditor : Editor
    {
        SerializedProperty arrowsProperty;

        private void OnEnable()
        {
            arrowsProperty = serializedObject.FindProperty("arrows");
        }

        public override void OnInspectorGUI()
        {
            ArrowBundle e = (ArrowBundle)target;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/quiver.png"));

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUILayout.PropertyField(arrowsProperty, new GUIContent("Arrows"));

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}
