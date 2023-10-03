using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Yarn.Unity;
using Yarn.Unity.Editor;
using Yarn;

namespace Perell.Artemis.Example.YarnSpinnerIntegration.Editor
{
    [CustomPropertyDrawer(typeof(YarnFletcher.ProjectStartingNodes))]
    public class YarnFletcherStartsDrawer : PropertyDrawer
    {
        readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty project = property.FindPropertyRelative(nameof(YarnFletcher.ProjectStartingNodes.project));
            SerializedProperty nodes = property.FindPropertyRelative(nameof(YarnFletcher.ProjectStartingNodes.startingNodes));

            int slots = 1;
            float add = 0;
            if (project.objectReferenceValue != null)
            {
                slots++;
                if (nodes.isExpanded)
                {
                    slots += Mathf.Max(1, nodes.arraySize);
                    add = 30;
                }
            }

            return add + base.GetPropertyHeight(project, label) * slots + spacing * (slots - 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty project = property.FindPropertyRelative(nameof(YarnFletcher.ProjectStartingNodes.project));
            SerializedProperty nodes = property.FindPropertyRelative(nameof(YarnFletcher.ProjectStartingNodes.startingNodes));

            bool projectFilled = project.objectReferenceValue != null;
            YarnProject yarnProject = null;
            if (projectFilled)
            {
                yarnProject = (YarnProject)project.objectReferenceValue;
            }

            float projectHeight = EditorGUIUtility.singleLineHeight;
            Rect projectRect = new Rect(position.x, position.y, position.width, projectHeight);
            EditorGUI.PropertyField(projectRect, project, GUIContent.none);

            if(projectFilled)
            {
                float listHeight = base.GetPropertyHeight(nodes, label);
                Rect listRect = new Rect(position.x, position.y + projectHeight + spacing, position.width, listHeight);
                EditorGUI.PropertyField(listRect, nodes, new GUIContent("Start Nodes"), true);
            }

        }
    }

    [CustomPropertyDrawer(typeof(YarnFletcher.ProjectStartingNodes.NodeString))]
    public class YarnFletcherStartsStringDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string path = property.propertyPath;
            SerializedProperty str = property.FindPropertyRelative(nameof(YarnFletcher.ProjectStartingNodes.NodeString.str));
            SerializedProperty project = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf("startingNodes.Array.data[")) + nameof(YarnFletcher.ProjectStartingNodes.project));

            if (project != null)
            {
                YarnProject yarnProject = (YarnProject)project.objectReferenceValue;
                List<GUIContent> optionsContent = new List<GUIContent>();
                int index = 0;
                int indexOf = 0;
                foreach (string name in yarnProject.NodeNames)
                {
                    if(name == str.stringValue)
                    {
                        indexOf = index;
                    }
                    optionsContent.Add(new GUIContent(name));
                    index++;
                }
                GUIContent[] optionsArray = optionsContent.ToArray();

                indexOf = EditorGUI.Popup(position, indexOf, optionsArray);
                str.stringValue = optionsArray[indexOf].text;
            }
        }
    }
}