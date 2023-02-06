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

            //TODO: Get Goddess to use its icon
            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Goddess.png"));


            serializedObject.Update();

            EditorGUILayout.PropertyField(flagsIdsToKeep);
            EditorGUILayout.PropertyField(globallyLoadedFlagBundles);

            EditorGUILayout.Space();
            FlagID[] flagIds = e.GetFlagIDs();
            foreach (FlagID id in flagIds)
            {
                EditorGUILayout.LabelField(id.ToString(), e.GetFlagValueType(id).ToString());
            }

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
   