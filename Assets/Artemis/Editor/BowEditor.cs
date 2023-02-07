using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(PreTemplateBow), true)]
    public class BowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PreTemplateBow preTemplateBow = (PreTemplateBow)target;
            
            EditorGUIUtility.SetIconForObject(preTemplateBow, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Bow.png"));
            
            DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(preTemplateBow);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }

}