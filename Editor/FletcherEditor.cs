using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(Fletcher<>),true)]
    public class FletcherEditor : IconObjectEditor
    {
        SerializedProperty database;
        bool showDatabase;

        private void OnEnable()
        {
            database = serializedObject.FindProperty("database").FindPropertyRelative("list");
        }


        public override void OnInspectorGUI()
        {
            PreDictionaryFletcher preDictionaryFletcher = (PreDictionaryFletcher)target;

            SetIcon("Fletcher");
            
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            DrawPropertiesExcluding(serializedObject, "database");

            showDatabase = EditorGUILayout.Foldout(showDatabase, "Database");
            if(showDatabase)
            {
                int id;
                EditorGUI.indentLevel++;
                for (int i = 0; i < database.arraySize; i++)
                {
                    id = database.GetArrayElementAtIndex(i).FindPropertyRelative("Key").intValue;
                    EditorGUILayout.PropertyField(database.GetArrayElementAtIndex(i).FindPropertyRelative("Value"), 
                        new GUIContent(System.Enum.GetName(preDictionaryFletcher.GetSymbolType(),id)),
                        true);
                }
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("Parse CSV into database"))
            {
                preDictionaryFletcher.GeneratorArrowDatabase();
            }

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(preDictionaryFletcher);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }

}
