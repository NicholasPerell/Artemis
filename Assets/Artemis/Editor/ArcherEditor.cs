using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Archer))]
    public class ArcherEditor : Editor
    {
        bool showBundleOptions = true;
        bool showDescisionMaking = true;
        bool showDataStructuring = true;

        string partitionInfo = "By choosing symbol flags to be in every single arrow an Archer will consider, the arrows can be divvied up into seperate tables.";

        SerializedProperty overallData;
        SerializedProperty partitioningFlags;
        SerializedProperty partitionedData;

        protected virtual void OnEnable()
        {
            overallData = serializedObject.FindProperty("overallData");
            partitioningFlags = serializedObject.FindProperty("partitioningFlags");
            partitionedData = serializedObject.FindProperty("partitionedData");
        }

        public override void OnInspectorGUI()
        {
            Archer archer = (Archer)target;

            DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                archer.Init();
            }
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Attempt Delivery"))
            {
                archer.IgnoreSuccessAttemptDelivery();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            showDescisionMaking = EditorGUILayout.BeginFoldoutHeaderGroup(showDescisionMaking, "Decision Making");
            if (showDescisionMaking)
            {
                archer.SetChoosingSamePriority((Archer.ChooseSamePriority)EditorGUILayout.EnumPopup("Handling Same Priority", archer.GetChoosingSamePriority()));

                archer.discardArrowsAfterUse = EditorGUILayout.Toggle("Discard Arrows After Use", archer.discardArrowsAfterUse);

                archer.loops = EditorGUILayout.Toggle("Loops", archer.loops);

                if (archer.loops)
                {
                    archer.includeBundlesInLoop = EditorGUILayout.Toggle("\u21B3 Include Bundles", archer.includeBundlesInLoop);
                    archer.includeHigherPrioritiesInLoop = EditorGUILayout.Toggle("\u21B3 Include Higher Priorities", archer.includeHigherPrioritiesInLoop);
                    if (GUILayout.Button("Set To Looped State"))
                    {
                        archer.SetLoopedState();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            showBundleOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showBundleOptions, "Arrow Bundles");
            if (showBundleOptions)
            {
                archer.tempArrowBundle = (ArrowBundle)EditorGUILayout.ObjectField("Temp Bundle ", archer.tempArrowBundle, typeof(ArrowBundle), false);
                if (GUILayout.Button("Dump Bundle"))
                {
                    archer.DumpBundle(archer.tempArrowBundle);
                    archer.tempArrowBundle = null;
                }
                if (GUILayout.Button("Drop Bundle"))
                {
                    archer.DropBundle(archer.tempArrowBundle);
                    archer.tempArrowBundle = null;
                }

                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(true);
                foreach (Archer.BundleLog e in archer.GetBundleHistory())
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
            EditorGUILayout.LabelField("Preview",EditorStyles.boldLabel);
            if (partitioningFlags.arraySize > 0)
            {
                //Test finding values

                for(int l = 0; l < partitioningFlags.arraySize; l++)
                EditorGUILayout.HelpBox("" + partitioningFlags.GetArrayElementAtIndex(l).enumNames[partitioningFlags.GetArrayElementAtIndex(l).enumValueIndex],MessageType.Info);


                SerializedProperty tempProperty2k;
                SerializedProperty tempProperty2v;
                SerializedProperty tempProperty3;

                string[] keyStrings;
                string sectionName = "";
                float value;

                SerializedProperty tempProperty = partitionedData.FindPropertyRelative("list");
                for (int j = 0; j < tempProperty.arraySize; j++)
                {
                    tempProperty2k = tempProperty.GetArrayElementAtIndex(j).FindPropertyRelative("Key");
                    tempProperty2v = tempProperty.GetArrayElementAtIndex(j).FindPropertyRelative("Value");

                    keyStrings = tempProperty2k.stringValue.Split('#',StringSplitOptions.RemoveEmptyEntries);

                    sectionName = "";
                    for(int i = 0; i < partitioningFlags.arraySize; i++)
                    {
                        value = float.Parse(keyStrings[i]);


                        System.Type enumType = Goddess.instance.GetFlagSymbolType(Enum.Parse<FlagID>(partitioningFlags.GetArrayElementAtIndex(i).enumNames[partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex]));
                        if (enumType != null)
                        {
                            var takeIn = (System.Enum)System.Enum.Parse(enumType, "" + (Mathf.FloorToInt(value)));
                            sectionName += takeIn.ToString();
                        }
                        else
                        sectionName += Mathf.FloorToInt(value);
                        if(i + 1 < partitioningFlags.arraySize)
                        {
                            sectionName += "-";
                        }
                    }
                    
                    EditorGUILayout.LabelField(sectionName);



                    for (int k = 0; k < tempProperty2v.arraySize; k++)
                    {
                        tempProperty3 = tempProperty2v.GetArrayElementAtIndex(k);
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(tempProperty3, new GUIContent(""));
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                SerializedProperty tempProperty;
                for (int i = 0; i < overallData.arraySize; i++)
                {
                    tempProperty = overallData.GetArrayElementAtIndex(i);
                    EditorGUILayout.ObjectField(tempProperty, new GUIContent(""));
                }
                EditorGUI.EndDisabledGroup();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(archer);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
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
