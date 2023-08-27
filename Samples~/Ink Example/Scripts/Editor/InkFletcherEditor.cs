using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Editor;

namespace Perell.Artemis.Example.InkIntegration.Editor
{
    [CustomEditor(typeof(InkFletcher))]
    public class InkFletcherEditor : IconObjectEditor
    {
        SerializedProperty inkList;
        SerializedProperty dataBase;

        private void OnEnable()
        {
            inkList = serializedObject.FindProperty("inksToCompile");
            dataBase = serializedObject.FindProperty("database");
        }

        public override void OnInspectorGUI()
        {
            InkFletcher inkFletcher = (InkFletcher)target;

            serializedObject.Update();

            SetIcon("Fletcher");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(inkList);
            EditorGUILayout.PropertyField(dataBase);

            if (GUILayout.Button("Compile Ink files into database"))
            {
                inkFletcher.CompileInks();
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(inkFletcher);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}