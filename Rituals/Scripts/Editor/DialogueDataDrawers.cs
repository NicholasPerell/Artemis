using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Generated;

namespace Perell.Artemis.Example.Rituals.Editor
{
    [CustomPropertyDrawer(typeof(DialogueData.LineData))]
    public class LineDataDrawers : PropertyDrawer
    {
        private const float spacing = 5f;
        private const uint lines = 2;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty speaker = property.FindPropertyRelative("speaker");
            SerializedProperty text = property.FindPropertyRelative("text");

            return base.GetPropertyHeight(speaker, label) + base.GetPropertyHeight(text, label) * lines + spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty speaker = property.FindPropertyRelative("speaker");
            SerializedProperty text = property.FindPropertyRelative("text");

            EditorGUI.BeginProperty(position, label, property);

            float speakerHeight = base.GetPropertyHeight(speaker, label);
            float textHeight = base.GetPropertyHeight(text, label) * lines;

            var speakerRect = new Rect(position.x, position.y, position.width / 2, speakerHeight);
            var textRect = new Rect(position.x, position.y + spacing + speakerHeight, position.width, textHeight);

            EditorGUI.PropertyField(speakerRect, speaker, GUIContent.none);
            EditorStyles.textField.wordWrap = true;
            text.stringValue = EditorGUI.TextArea(textRect, text.stringValue);

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(DialogueData.FlagChangeData))]
    public class FlagChangeDataDrawers : PropertyDrawer
    {
        private const float spacing = 5f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty stringId = property.FindPropertyRelative("stringId");
            return base.GetPropertyHeight(stringId, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty stringId = property.FindPropertyRelative("stringId");
            SerializedProperty id = property.FindPropertyRelative("id");
            SerializedProperty stringValue = property.FindPropertyRelative("stringValue");
            SerializedProperty value = property.FindPropertyRelative("value");
            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects
            var leftRect = new Rect(position.x, position.y + spacing / 2, position.width / 2 - spacing / 2, position.height - spacing / 2);
            var rightRect = new Rect(position.x + spacing / 2 + position.width / 2, position.y + spacing / 2, position.width / 2 - spacing / 2, position.height - spacing / 2);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            if (id.intValue == -1)
            {
                EditorGUI.PropertyField(leftRect, stringId, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(leftRect, id, GUIContent.none);
            }

            if (stringValue.stringValue != null && stringValue.stringValue != "")
            {
                EditorGUI.PropertyField(rightRect, stringValue, GUIContent.none);
            }
            else
            {
                switch (Goddess.instance.GetFlagValueType((FlagID)id.intValue))
                {
                    case Flag.ValueType.BOOL:
                        bool toggle = EditorGUI.Toggle(rightRect, value.floatValue == 1);
                        value.floatValue = toggle ? 1 : 0;
                        break;
                    case Flag.ValueType.SYMBOL:
                        var takeIn = EditorGUI.EnumPopup(rightRect, (System.Enum)System.Enum.Parse(Goddess.instance.GetFlagSymbolType((FlagID)id.intValue), "" + ((int)value.floatValue)));
                        value.floatValue = (int)((object)takeIn);
                        break;
                    case Flag.ValueType.FLOAT:
                    default:
                        EditorGUI.PropertyField(rightRect, value, GUIContent.none);
                        break;
                }
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(DialogueData.ArcherChangeData))]
    public class ArcherChangeDataDrawers : PropertyDrawer
    {
        private const float spacing = 5f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty archer = property.FindPropertyRelative("archer");
            SerializedProperty arrowBundle = property.FindPropertyRelative("arrowBundle");
            int arrowsInBundle = 0;
            if (arrowBundle.objectReferenceValue != null)
            {
                arrowsInBundle = ((ArrowBundle)arrowBundle.objectReferenceValue).GetArrows().Length;
            }

            return (base.GetPropertyHeight(archer, label) * (3 + arrowsInBundle)) + (spacing * (2 + arrowsInBundle));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty archer = property.FindPropertyRelative("archer");
            SerializedProperty arrowBundle = property.FindPropertyRelative("arrowBundle");
            Arrow[] arrows = new Arrow[0];
            if (arrowBundle.objectReferenceValue != null)
            {
                arrows = ((ArrowBundle)arrowBundle.objectReferenceValue).GetArrows();
            }
            SerializedProperty dumping = property.FindPropertyRelative("dumping");

            EditorGUI.BeginProperty(position, label, property);

            string header = "";
            if (dumping.boolValue)
            {
                header = "(+) Dumping";
                EditorGUI.DrawRect(position, new Color(0, 1, 0, 0.1f));
            }
            else
            {
                header = "(-) Dropping";
                EditorGUI.DrawRect(position, new Color(1, 0, 0, 0.1f));
            }

            float objectRefHeight = base.GetPropertyHeight(archer, label);

            Rect headerRect = new Rect(position.x, position.y, position.width, objectRefHeight);
            EditorGUI.LabelField(headerRect, header, EditorStyles.boldLabel);

            Rect archerRect = new Rect(position.x, position.y + objectRefHeight + spacing, position.width, objectRefHeight);
            EditorGUI.PropertyField(archerRect, archer, GUIContent.none);

            Rect arrowBundleRect = new Rect(position.x, position.y + (objectRefHeight + spacing) * 2, position.width, objectRefHeight);
            EditorGUI.PropertyField(arrowBundleRect, arrowBundle, GUIContent.none);

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            Rect arrowRect;
            for (int i = 0; i < arrows.Length; i++)
            {
                arrowRect = new Rect(position.x, position.y + objectRefHeight * (i+3) + spacing * (i+3), position.width, objectRefHeight);
                EditorGUI.ObjectField(arrowRect, new GUIContent(), arrows[i], arrows[i].GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
}