using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(PreDictionaryFletcher),true)]
    public class PreDictionaryFletcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PreDictionaryFletcher e = (PreDictionaryFletcher)target;

            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/table.png"));

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (GUILayout.Button("Parse CSV into database"))
            {
                e.DeliverySystemDatabase();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }

}
