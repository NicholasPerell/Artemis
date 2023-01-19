using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(FlagState))]
    public class FlagStateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FlagState e = (FlagState)target;

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            //e.tempFlag = (Flag)EditorGUILayout.ObjectField("Flag ", e.tempFlag, typeof(Flag), false);

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
                EditorGUILayout.ObjectField(flag.GetFlagId().ToString(), flag, typeof(Flag), false);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            FlagState example = (FlagState)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Database.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}