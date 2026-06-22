using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(PreTemplateBow), true)]
    public class BowEditor : IconObjectEditor
    {
        public override void OnInspectorGUI()
        {
            PreTemplateBow preTemplateBow = (PreTemplateBow)target;
            
            SetIcon("Bow");

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