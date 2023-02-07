using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(FlagBundle))]
    public class FlagBundleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FlagBundle e = (FlagBundle)target;
            
            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/FlagBundle.png"));

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (GUILayout.Button("Add To List"))
            {

                if (e.tempFlags != null)
                {
                    foreach (Flag flag in e.tempFlags)
                    {
                        e.Add(flag);
                    }
                    e.tempFlags = null;
                }
            }
            if (GUILayout.Button("Remove From List"))
            {
                if(e.tempFlags == null)
                {
                    e.tempFlags = new Flag[1];
                }

                foreach (Flag flag in e.tempFlags)
                {
                    e.Remove(flag);
                }
                e.tempFlags = null;
            }

            EditorGUILayout.Space();
            Flag[] flags = e.ToValueArray();

            EditorGUI.BeginDisabledGroup(true);
            foreach (Flag flag in flags)
            {
                EditorGUILayout.ObjectField(flag.GetFlagID().ToString(), flag, typeof(Flag), false);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}