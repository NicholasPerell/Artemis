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
        Vector2 scrollPos;

        protected virtual void OnEnable()
        {
            flagsIdsToKeep = serializedObject.FindProperty("flagsIdsToKeep");
            globallyLoadedFlagBundles = serializedObject.FindProperty("globallyLoadedFlagBundles");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            Goddess e = (Goddess)target;

            GUILayout.BeginHorizontal();
            int iconSize = 70;
            GUILayout.Space((EditorGUIUtility.currentViewWidth / 2) - (iconSize/2));
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Goddess.png"), GUILayout.Height(iconSize), GUILayout.Width(iconSize));
            GUILayout.EndHorizontal();

            serializedObject.Update();

            EditorGUILayout.PropertyField(flagsIdsToKeep);
            EditorGUILayout.PropertyField(globallyLoadedFlagBundles);

            EditorGUILayout.Space();

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            FlagID[] flagIds = e.GetFlagIDs();
            foreach (FlagID id in flagIds)
            {
                EditorGUILayout.LabelField(id.ToString(), e.GetFlagValueType(id).ToString());
            }
            GUILayout.EndScrollView();

            EditorGUILayout.Space();
            if (GUILayout.Button("RESET \u26A0"))
            {
                e.Reset();
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
    }
}