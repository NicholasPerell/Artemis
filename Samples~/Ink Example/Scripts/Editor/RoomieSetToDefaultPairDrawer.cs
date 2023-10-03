using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Perell.Artemis.Generated;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration.Editor
{
    [CustomPropertyDrawer(typeof(RoomieSetToDefault.NameToValue))]
    public class RoomieSetToDefaultPairDrawer : PropertyDrawer
    {

        private const float spacing = 5f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty id = property.FindPropertyRelative("flagId");
            SerializedProperty data = property.FindPropertyRelative("value");

            return base.GetPropertyHeight(id, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty id = property.FindPropertyRelative("flagId");
            SerializedProperty data = property.FindPropertyRelative("value");

            EditorGUI.BeginProperty(position, label, property);

            FlagID temp;
            bool found = System.Enum.TryParse<FlagID>(id.stringValue, true, out temp);
            
            if (!found)
            {
                float warnWidth = 15f;
                var warnRect = new Rect(position.x, position.y + spacing / 2, warnWidth, position.height - spacing / 2);
                position.x += warnWidth + spacing;
                position.width -= warnWidth + spacing;
                EditorGUI.LabelField(warnRect, new GUIContent("\u26A0", "Flag ID not found."));
            }


            // Calculate rects
            var amountRect = new Rect(position.x, position.y + spacing / 2, position.width/2 - spacing / 2, position.height - spacing / 2);
            var valRect = new Rect(position.x + spacing / 2 + position.width / 2, position.y + spacing / 2, position.width/ 2 - spacing / 2, position.height - spacing / 2);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, id, GUIContent.none);
            EditorGUI.PropertyField(valRect, data, GUIContent.none);


            EditorGUI.EndProperty();
        }
    }
}