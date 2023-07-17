using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(PreTemplateBow), true)]
    public class BowEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PreTemplateBow preTemplateBow = (PreTemplateBow)target;
            
            EditorGUIUtility.SetIconForObject(preTemplateBow, AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.perell.artemis/Editor/Icons/Bow.png"));
            
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