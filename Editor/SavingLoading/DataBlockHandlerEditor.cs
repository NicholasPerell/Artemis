using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Saving;

namespace Perell.Artemis.Editor.Saving
{
    [CustomEditor(typeof(DataBlockHandler))]
    public class DataBlockHandlerEditor : UnityEditor.Editor
    {
        SerializedProperty flags;
        SerializedProperty archers;
        SerializedProperty fileName;
        SerializedProperty assetToLoadIn;

        private void OnEnable()
        {
            SerializedProperty dataBlock = serializedObject.FindProperty("dataBlock");
            flags = dataBlock.FindPropertyRelative("flags");
            archers = dataBlock.FindPropertyRelative("archers");
            fileName = serializedObject.FindProperty("m_FileName");
            assetToLoadIn = serializedObject.FindProperty("m_AssetToLoadIn");
        }


        public override void OnInspectorGUI()
        {
            DataBlockHandler dataBlockHandler = (DataBlockHandler)target;
            EditorGUIUtility.SetIconForObject(dataBlockHandler, AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.perell.artemis/Editor/Icons/Archer.png"));

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
                dataBlockHandler.SaveAsPeristentData(fileName.stringValue);
            }
            if (GUILayout.Button("Load Data From Peristent Memory"))
            {
                dataBlockHandler.LoadAsPeristentData(fileName.stringValue);
            }
            if (GUILayout.Button("Save Data As Game Asset"))
            {
                dataBlockHandler.SaveAsTextAsset(fileName.stringValue);
            }
            EditorGUILayout.PropertyField(assetToLoadIn, new GUIContent("Binary Asset"));
            if (GUILayout.Button("Load Data From Game Asset"))
            {
                dataBlockHandler.LoadFromTextAsset((TextAsset)assetToLoadIn.objectReferenceValue);
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(dataBlockHandler);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}