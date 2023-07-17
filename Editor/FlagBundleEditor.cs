using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(FlagBundle))]
    public class FlagBundleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FlagBundle flagBundle = (FlagBundle)target;
            
            EditorGUIUtility.SetIconForObject(flagBundle, AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.perell.artemis/Editor/Icons/FlagBundle.png"));

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (GUILayout.Button("Add To List"))
            {

                if (flagBundle.tempFlags != null)
                {
                    foreach (Flag flag in flagBundle.tempFlags)
                    {
                        flagBundle.Add(flag);
                    }
                    flagBundle.tempFlags = null;
                }
            }
            if (GUILayout.Button("Remove From List"))
            {
                if(flagBundle.tempFlags == null)
                {
                    flagBundle.tempFlags = new Flag[1];
                }

                foreach (Flag flag in flagBundle.tempFlags)
                {
                    flagBundle.Remove(flag);
                }
                flagBundle.tempFlags = null;
            }

            EditorGUILayout.Space();
            Flag[] flags = flagBundle.ToValueArray();

            EditorGUI.BeginDisabledGroup(true);
            foreach (Flag flag in flags)
            {
                EditorGUILayout.ObjectField(flag.GetFlagID().ToString(), flag, typeof(Flag), false);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(flagBundle);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}