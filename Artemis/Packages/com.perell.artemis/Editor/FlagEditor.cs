using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Generated;

namespace Perell.Artemis.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlagV2))]
    public class FlagEditor : IconObjectEditor
    {
        SerializedProperty valueProperty;

        private void OnEnable()
        {
            valueProperty = serializedObject.FindProperty("value");
        }

        public override void OnInspectorGUI()
        {
            FlagV2[] flags = new FlagV2[targets.Length];
            for (int i = 0; i < flags.Length; i++)
            {
                flags[i] = (FlagV2)targets[i];
            }

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            FlagV2.ValueType valueType = flags[0].GetValueType();
            FlagID flagId = flags[0].GetFlagID();

            //Turn this into a check inside its class?
            FlagID tempFlagId;
            int validityAmount = 0;
            foreach (FlagV2 flag in flags)
            {
                flag.ValidateFlag();
                tempFlagId = flag.GetFlagID();

                if (flagId != tempFlagId)
                {
                    flagId = FlagID.INVALID;
                }

                if (valueType != flag.GetValueType())
                {
                    valueType = FlagV2.ValueType.INVALID;
                }

                if (tempFlagId == FlagID.INVALID)
                {
                    validityAmount--;
                }
                else
                {
                    validityAmount++;
                }
            }

            if (validityAmount == flags.Length) //All valid Flag IDs
            {
                if (flagId != FlagID.INVALID)
                {
                    EditorGUILayout.LabelField("Flag ID", flagId.ToString());
                }
                else
                {
                    EditorGUILayout.LabelField("Flag ID", "�");
                }

                switch (valueType)
                {
                    case FlagV2.ValueType.FLOAT:
                        EditorGUILayout.PropertyField(valueProperty, new GUIContent("Value"));
                        break;
                    case FlagV2.ValueType.BOOL:
                        bool oldBoolValue = flags[0].GetValue() == 1;
                        if (valueProperty.hasMultipleDifferentValues)
                        {
                            EditorGUI.showMixedValue = true;
                            oldBoolValue = false;
                        }
                        bool newBoolValue = EditorGUILayout.Toggle("Value", oldBoolValue);
                        if (oldBoolValue != newBoolValue)
                        {
                            foreach (FlagV2 flag in flags)
                            {
                                flag.SetValue(newBoolValue);
                            }
                        }
                        if (valueProperty.hasMultipleDifferentValues)
                        {
                            EditorGUI.showMixedValue = false;
                        }
                        break;
                    case FlagV2.ValueType.SYMBOL:
                        if (flagId != FlagID.INVALID)
                        {
                            int oldEnumValue = Mathf.FloorToInt(flags[0].GetValue());
                            if (valueProperty.hasMultipleDifferentValues)
                            {
                                EditorGUI.showMixedValue = true;
                                oldEnumValue = int.MaxValue; //This value isn't reached for any of the compiled enum flags
                            }
                            var takeIn = EditorGUILayout.EnumPopup("Value", (System.Enum)System.Enum.Parse(flags[0].GetSymbolType(), "" + (oldEnumValue)));
                            int newEnumValue = (int)((object)takeIn);
                            if (oldEnumValue != newEnumValue)
                            {
                                foreach (FlagV2 flag in flags)
                                {
                                    flag.SetValue(newEnumValue);
                                }
                            }
                            if (valueProperty.hasMultipleDifferentValues)
                            {
                                EditorGUI.showMixedValue = false;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Value", "�");
                        }
                        break;
                    default:
                        EditorGUILayout.LabelField("Value", "�");
                        break;
                }
            }
            else if (validityAmount == -flags.Length) //All invalid Flag IDs
            {
                FlagID tempId = (FlagID)EditorGUILayout.EnumPopup("Flag ID", flagId);
                if (tempId != FlagID.INVALID)
                {
                    foreach (FlagV2 flag in flags)
                    {
                        flag.InitFlag(tempId);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                foreach (FlagV2 flag in flags)
                {
                    EditorUtility.SetDirty(flag);
                }
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            FlagV2 example = (FlagV2)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            string fileSuffix = "";

            if (example.GetFlagID() != FlagID.INVALID)
            {
                switch (example.GetValueType())
                {
                    case FlagV2.ValueType.FLOAT:
                        fileSuffix = "Float";
                        break;
                    case FlagV2.ValueType.BOOL:
                        fileSuffix = "Bool";
                        break;
                    case FlagV2.ValueType.SYMBOL:
                        fileSuffix = "Symbol";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (example.GetValueType() == FlagV2.ValueType.INVALID)
                {
                    fileSuffix = "Unknown";
                }
                else
                {
                    fileSuffix = "Issue";
                }
            }

            copyFrom = LoadIconTexture("Flag" + fileSuffix);

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
