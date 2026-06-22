using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    [CustomEditor(typeof(FlagBundle))]
    public class FlagBundleEditor : IconObjectEditor
    {
        public override void OnInspectorGUI()
        {
            FlagBundle flagBundle = (FlagBundle)target;

            SetIcon("FlagBundle");

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (GUILayout.Button("Add To List"))
            {

                if (flagBundle.tempFlags != null)
                {
                    foreach (FlagV2 flag in flagBundle.tempFlags)
                    {
                        flagBundle.Add(flag);
                    }
                    flagBundle.tempFlags = null;
                }
            }
            if (GUILayout.Button("Remove From List"))
            {
                if (flagBundle.tempFlags == null)
                {
                    flagBundle.tempFlags = new FlagV2[1];
                }

                foreach (FlagV2 flag in flagBundle.tempFlags)
                {
                    flagBundle.Remove(flag);
                }
                flagBundle.tempFlags = null;
            }

            EditorGUILayout.Space();
            FlagV2[] flags = flagBundle.ToValueArray();

            EditorGUI.BeginDisabledGroup(true);
            foreach (FlagV2 flag in flags)
            {
                EditorGUILayout.ObjectField(flag.GetFlagID().ToString(), flag, typeof(FlagV2), false);
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