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

        string partitionInfo = "By choosing symbol flags to be in every single arrow an Archer will consider, the arrows can be divvied up into seperate tables.";

        SerializedProperty overallData;
        SerializedProperty partitioningFlags;
        SerializedProperty partitionedData;
        SerializedProperty defaultContents;
        SerializedProperty tempPartitioningFlags;

        SerializedProperty loops;
        SerializedProperty includeBundlesInLoop;
        SerializedProperty includeHigherPrioritiesInLoop;

        SerializedProperty discardArrowsAfterUse;

        protected virtual void OnEnable()
        {
            overallData = serializedObject.FindProperty("overallData");
            partitioningFlags = serializedObject.FindProperty("partitioningFlags");
            partitionedData = serializedObject.FindProperty("partitionedData");
            defaultContents = serializedObject.FindProperty("defaultContents");
            tempPartitioningFlags = serializedObject.FindProperty("tempPartitioningFlags");

            loops = serializedObject.FindProperty("loops");
            includeBundlesInLoop = serializedObject.FindProperty("includeBundlesInLoop");
            includeHigherPrioritiesInLoop = serializedObject.FindProperty("includeHigherPrioritiesInLoop");

            discardArrowsAfterUse = serializedObject.FindProperty("discardArrowsAfterUse");
        }

        public override void OnInspectorGUI()
        {
            Archer archer = (Archer)target;

            EditorGUIUtility.SetIconForObject(archer, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Archer.png"));

            serializedObject.Update();

            //Default contents
            EditorGUILayout.PropertyField(defaultContents);

            EditorGUI.BeginChangeCheck();
            bool changed = false;

            if (GUILayout.Button("Initialize"))
            {
                archer.Init();
                changed = true;
            }
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Attempt Delivery"))
            {
                archer.IgnoreSuccessAttemptDelivery();
                changed = true;
            }
            EditorGUI.EndDisabledGroup();

            //Decision making
            EditorGUILayout.Space();
            showDescisionMaking = EditorGUILayout.BeginFoldoutHeaderGroup(showDescisionMaking, "Decision Making");
            if (showDescisionMaking)
            {
                archer.SetChoosingSamePriority((Archer.ChooseSamePriority)EditorGUILayout.EnumPopup("Handling Same Priority", archer.GetChoosingSamePriority()));

                EditorGUILayout.PropertyField(discardArrowsAfterUse, new GUIContent("Discard Arrows After Use"));

                EditorGUILayout.PropertyField(loops, new GUIContent("Loops"));
                if (loops.boolValue)
                {
                    EditorGUILayout.PropertyField(includeBundlesInLoop, new GUIContent("\u21B3 Include Bundles"));
                    EditorGUILayout.PropertyField(includeHigherPrioritiesInLoop, new GUIContent("\u21B3 Include Higher Priorities"));
                    if (GUILayout.Button("Set To Looped State"))
                    {
                        archer.SetToLoopedState();
                        changed = true;
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Arrow bundles
            EditorGUILayout.Space();
            showBundleOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showBundleOptions, "Arrow Bundles");
            if (showBundleOptions)
            {
                archer.tempArrowBundle = (ArrowBundle)EditorGUILayout.ObjectField("Temp Bundle ", archer.tempArrowBundle, typeof(ArrowBundle), false);
                if (GUILayout.Button("Dump Bundle"))
                {
                    archer.DumpBundle(archer.tempArrowBundle);
                    archer.tempArrowBundle = null;
                    changed = true;
                }
                if (GUILayout.Button("Drop Bundle"))
                {
                    archer.DropBundle(archer.tempArrowBundle);
                    archer.tempArrowBundle = null;
                    changed = true;
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

            //Partitioning Flags
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(partitionInfo, MessageType.Info);
            EditorGUILayout.PropertyField(tempPartitioningFlags, new GUIContent("Temp Partitioning Flags"));
            if (GUILayout.Button("Partition"))
            {
                archer.Repartition();
                changed = true;
            }
            GUIStyle partitionLayoutStyle = new GUIStyle(EditorStyles.label);
            partitionLayoutStyle.alignment = TextAnchor.MiddleCenter;
            string partitionOfficialText = "";
            for (int i = 0; i < partitioningFlags.arraySize; i++)
            {
                if (partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex > -1 && partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex < partitioningFlags.GetArrayElementAtIndex(i).enumNames.Length)
                {
                    partitionOfficialText += partitioningFlags.GetArrayElementAtIndex(i).enumNames[partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex];
                }
                else
                {
                    partitionOfficialText += partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex;
                }
                if (i + 1 < partitioningFlags.arraySize)
                {
                    partitionOfficialText += "—";
                }
            }

            //Partioning Key
            if (partitionOfficialText.Length > 0)
            {
                EditorGUILayout.LabelField(partitionOfficialText, partitionLayoutStyle);
            }

            //Preview
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview",EditorStyles.boldLabel);
            if (partitioningFlags.arraySize > 0)
            {
                SerializedProperty tempProperty2k;
                SerializedProperty tempProperty2v;
                SerializedProperty tempProperty3;

                int[] keyParts;
                string sectionName = "";
                float value;

                SerializedProperty tempProperty = partitionedData.FindPropertyRelative("list");
                for (int j = 0; j < tempProperty.arraySize; j++)
                {
                    tempProperty2k = tempProperty.GetArrayElementAtIndex(j).FindPropertyRelative("Key").FindPropertyRelative("mArray");
                    tempProperty2v = tempProperty.GetArrayElementAtIndex(j).FindPropertyRelative("Value").FindPropertyRelative("mArrows");

                    keyParts = new int[tempProperty2k.arraySize];
                    for (int i = 0; i < tempProperty2k.arraySize; i++)
                    {
                        keyParts[i] = tempProperty2k.GetArrayElementAtIndex(i).intValue;
                    }

                    sectionName = "";
                    for(int i = 0; i < keyParts.Length; i++)
                    {
                        value = keyParts[i];
                        System.Type enumType = null;
                        if (partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex > -1 && partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex < partitioningFlags.GetArrayElementAtIndex(i).enumNames.Length)
                        {
                            enumType = Goddess.instance.GetFlagSymbolType(Enum.Parse<FlagID>(partitioningFlags.GetArrayElementAtIndex(i).enumNames[partitioningFlags.GetArrayElementAtIndex(i).enumValueIndex]));
                        }
                        if (enumType != null)
                        {
                            var takeIn = (System.Enum)System.Enum.Parse(enumType, "" + (Mathf.FloorToInt(value)));
                            sectionName += takeIn.ToString();
                        }
                        else
                        sectionName += Mathf.FloorToInt(value);
                        if(i + 1 < partitioningFlags.arraySize)
                        {
                            sectionName += "—";
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
                    EditorGUILayout.Space();
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

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() || changed)
            {
                EditorUtility.SetDirty(archer);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}
