using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Archer))]
    public class ArcherEditor : Editor
    {
        bool showBundleOptions = true;
        bool showDescisionMaking = true;
        bool showDataStructuring = true;

        string partitionInfo = "By choosing symbol flags to be in every single arrow an Archer will consider, the arrows can be divvied up into seperate tables.";

        public override void OnInspectorGUI()
        {
            Archer mockUp = (Archer)target;

            DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
            }
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Attempt Delivery"))
            {
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            showDescisionMaking = EditorGUILayout.BeginFoldoutHeaderGroup(showDescisionMaking, "Decision Making");
            if (showDescisionMaking)
            {
                mockUp.SetChoosingSamePriority((Archer.ChooseSamePriority)EditorGUILayout.EnumPopup("Handling Same Priority", mockUp.GetChoosingSamePriority()));

                mockUp.discardArrowsAfterUse = EditorGUILayout.Toggle("Discard Arrows After Use", mockUp.discardArrowsAfterUse);

                mockUp.loops = EditorGUILayout.Toggle("Loops", mockUp.loops);

                if (mockUp.loops)
                {
                    mockUp.includeBundlesInLoop = EditorGUILayout.Toggle("\u21B3 Include Bundles", mockUp.includeBundlesInLoop);
                    mockUp.includeHigherPrioritiesInLoop = EditorGUILayout.Toggle("\u21B3 Include Higher Priorities", mockUp.includeHigherPrioritiesInLoop);
                    if (GUILayout.Button("Set To Looped State"))
                    {
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            showBundleOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showBundleOptions, "Arrow Bundles");
            if (showBundleOptions)
            {
                mockUp.tempArrowBundle = (ArrowBundle)EditorGUILayout.ObjectField("Temp Bundle ", mockUp.tempArrowBundle, typeof(ArrowBundle), false);
                if (GUILayout.Button("Dump Bundle"))
                {
                    mockUp.DumpBundle(mockUp.tempArrowBundle);
                    mockUp.tempArrowBundle = null;
                }
                if (GUILayout.Button("Drop Bundle"))
                {
                    mockUp.RemoveBundle(mockUp.tempArrowBundle);
                    mockUp.tempArrowBundle = null;
                }

                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(true);
                foreach (Archer.BundleLog e in mockUp.GetBundleHistory())
                {
                    EditorGUILayout.ObjectField(e.isAdding ? "+" : "-", e.bundle, typeof(ArrowBundle), false);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            showDataStructuring = EditorGUILayout.BeginFoldoutHeaderGroup(showDataStructuring, "Data Structuring");
            if (showDataStructuring)
            {

                EditorGUILayout.HelpBox(partitionInfo, MessageType.Info);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(mockUp);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Archer example = (Archer)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/bowman.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
