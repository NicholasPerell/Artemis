using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Bow), true)]
    public class BowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Bow e = (Bow)target;
            
            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/bow-arrow.png"));
            
            DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }

}