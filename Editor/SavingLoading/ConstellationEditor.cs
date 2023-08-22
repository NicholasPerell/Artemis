using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Saving;

namespace Perell.Artemis.Editor.Saving
{
    [CustomEditor(typeof(Constellation))]
    public class ConstellationEditor : IconObjectEditor
    {
        SerializedProperty flags;
        SerializedProperty archers;
        SerializedProperty fileName;
        SerializedProperty assetToLoadIn;

        private void OnEnable()
        {
            SerializedProperty data = serializedObject.FindProperty("dataBlock");
            flags = data.FindPropertyRelative("flags");
            archers = data.FindPropertyRelative("archers");
            fileName = serializedObject.FindProperty("m_FileName");
            assetToLoadIn = serializedObject.FindProperty("m_AssetToLoadIn");
        }


        public override void OnInspectorGUI()
        {
            Constellation constellation = (Constellation)target;
            SetIcon("Constellation");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(flags);
            EditorGUILayout.PropertyField(archers);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Saving & Loading", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(fileName, new GUIContent("File Name"));
            if (GUILayout.Button("Save Data To Peristent Memory"))
            {
                constellation.SaveAsPeristentData(fileName.stringValue);
            }
            if (GUILayout.Button("Load Data From Peristent Memory"))
            {
                constellation.LoadAsPeristentData(fileName.stringValue);
            }
            if (GUILayout.Button("Save Data As Game Asset"))
            {
                constellation.SaveAsTextAsset(fileName.stringValue);
            }
            EditorGUILayout.PropertyField(assetToLoadIn, new GUIContent("Binary Asset"));
            if (GUILayout.Button("Load Data From Game Asset"))
            {
                constellation.LoadFromTextAsset((TextAsset)assetToLoadIn.objectReferenceValue);
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(constellation);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}