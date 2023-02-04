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

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            ArrowBundle example = (ArrowBundle)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/quiver.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
