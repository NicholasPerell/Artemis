using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Goddess))]
    public class GoddessEditor : Editor
    {
        SerializedProperty flagsIdsToKeep;
        SerializedProperty globallyLoadedFlagBundles;

        protected virtual void OnEnable()
        {
            flagsIdsToKeep = serializedObject.FindProperty("flagsIdsToKeep");
            globallyLoadedFlagBundles = serializedObject.FindProperty("globallyLoadedFlagBundles");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            Goddess e = (Goddess)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(flagsIdsToKeep);
            EditorGUILayout.PropertyField(globallyLoadedFlagBundles);

            EditorGUILayout.Space();
            FlagID[] flagIds = e.GetFlagIDs();
            foreach (FlagID id in flagIds)
            {
                EditorGUILayout.LabelField(id.ToString(), e.GetFlagValueType(id).ToString());
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                e.Modify();
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Goddess example = (Goddess)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/night-sky.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
