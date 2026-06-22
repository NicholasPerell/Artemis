using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Perell.Artemis.Debugging;

namespace Perell.Artemis.Editor
{
    [CustomPropertyDrawer(typeof(Debugging.Logger))]
    public class LoggerPropertyDrawer : PropertyDrawer
    {
        private bool _foldout;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_foldout)
            {
                return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            var full = property.FindPropertyRelative("_prefix");
            var fullString = full.stringValue;
            label.text = $"{fullString} {label.text}";
            _foldout = EditorGUI.BeginFoldoutHeaderGroup(position, _foldout, label, new GUIStyle(EditorStyles.foldout) { richText = true });

            if (_foldout)
            {
                var bracketOpeningIndex = fullString.IndexOf(">[", 7);
                var colorStr = fullString.Substring(7, bracketOpeningIndex - 7);
                var colorParsed = ColorUtility.TryParseHtmlString(colorStr, out var color);
                var prefix = fullString.Substring(bracketOpeningIndex + 2, fullString.Length - bracketOpeningIndex - 2 - 9);

                EditorGUI.indentLevel++;

                position.position += Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                prefix = EditorGUI.TextField(position, "Prefix", prefix);
                position.position += Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                color = EditorGUI.ColorField(position, "Prefix Color", color);
                position.position += Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("Level"));

                fullString = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>[{prefix}]</color>";
                full.stringValue = fullString;

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndFoldoutHeaderGroup();


            // base.OnGUI(position, property, label);
        }
    }
}