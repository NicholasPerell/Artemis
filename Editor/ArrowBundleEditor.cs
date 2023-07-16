using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ArrowBundle))]
    public class ArrowBundleEditor : UnityEditor.Editor
    {
        SerializedProperty arrowsProperty;

        private void OnEnable()
        {
            arrowsProperty = serializedObject.FindProperty("arrows");
        }

        public override void OnInspectorGUI()
        {
            ArrowBundle arrowBundle = (ArrowBundle)target;

            //TODO: Figure out why Resources works in the Ink Package but not in Artemis
            //AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            EditorGUIUtility.SetIconForObject(arrowBundle, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/ArrowBundle.png"));

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
