using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(PreDictionaryFletcher),true)]
    public class PreDictionaryFletcherEditor : IconObjectEditor
    {
        public override void OnInspectorGUI()
        {
            PreDictionaryFletcher preDictionaryFletcher = (PreDictionaryFletcher)target;

            SetIcon("Fletcher");
            
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (GUILayout.Button("Parse CSV into database"))
            {
                preDictionaryFletcher.GeneratorArrowDatabase();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(preDictionaryFletcher);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }

}
