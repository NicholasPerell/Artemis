using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Flag))]
    public class FlagEditor : Editor
    {
        SerializedProperty valueProperty;

        private void OnEnable()
        {
            valueProperty = serializedObject.FindProperty("value");
        }

        public override void OnInspectorGUI()
        {
            Flag[] flags = new Flag[targets.Length];
            for (int i = 0; i < flags.Length; i++)
            {
                flags[i] = (Flag)targets[i];
            }

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Flag.ValueType valueType = flags[0].GetValueType();
            FlagID flagId = flags[0].GetFlagID();

            //Turn this into a check inside its class?
            FlagID tempFlagId;
            int validityAmount = 0;
            foreach (Flag flag in flags)
            {
                tempFlagId = flag.GetFlagID();
                if (tempFlagId.ToString() == ((int)tempFlagId).ToString())
                {
                    flag.SetFlagID(FlagID.INVALID);
                    tempFlagId = FlagID.INVALID;

                    //Repaint!
                    EditorUtility.SetDirty(flag);
                    AssetDatabase.SaveAssets();
                    Repaint();
                }

                if (flagId != tempFlagId)
                {
                    flagId = FlagID.INVALID;
                }

                if (valueType != flag.GetValueType())
                {
                    valueType = Flag.ValueType.INVALID;
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
                    EditorGUILayout.LabelField("Flag ID", "—");
                }

                switch (valueType)
                {
                    case Flag.ValueType.FLOAT:
                        EditorGUILayout.PropertyField(valueProperty, new GUIContent("Value"));
                        break;
                    case Flag.ValueType.BOOL:
                        bool oldBoolValue = flags[0].GetValue() == 1;
                        if (valueProperty.hasMultipleDifferentValues)
                        {
                            EditorGUI.showMixedValue = true;
                            oldBoolValue = false;
                        }
                        bool newBoolValue = EditorGUILayout.Toggle("Value", oldBoolValue);
                        if (oldBoolValue != newBoolValue)
                        {
                            foreach (Flag flag in flags)
                            {
                                flag.SetValue(newBoolValue);
                            }
                        }
                        if (valueProperty.hasMultipleDifferentValues)
                        {
                            EditorGUI.showMixedValue = false;
                        }
                        break;
                    case Flag.ValueType.SYMBOL:
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
                                foreach (Flag flag in flags)
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
                            EditorGUILayout.LabelField("Value", "—");
                        }
                        break;
                    default:
                        EditorGUILayout.LabelField("Value", "—");
                        break;
                }
            }
            else if (validityAmount == -flags.Length) //All invalid Flag IDs
            {
                flags[0].SetFlagID((FlagID)EditorGUILayout.EnumPopup("Flag ID", flagId));

                FlagID tempId = flags[0].GetFlagID();
                if (tempId != FlagID.INVALID)
                {
                    Flag.ValueType tempValueType = Goddess.instance.GetFlagValueType(tempId);
                    flags[0].SetValueType(tempValueType);

                    System.Type tempSymbolType = null;
                    if (tempValueType == Flag.ValueType.SYMBOL)
                    {
                        tempSymbolType = Goddess.instance.GetFlagSymbolType(tempId);
                    }

                    foreach (Flag flag in flags)
                    {
                        flag.SetFlagID(tempId);
                        flag.SetValueType(tempValueType);
                        if (tempValueType == Flag.ValueType.SYMBOL)
                        {
                            flag.SetSymbolType(tempSymbolType);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                foreach (Flag flag in flags)
                {
                    EditorUtility.SetDirty(flag);
                }
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Flag example = (Flag)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");

            string fileSuffix = "";

            if (example.GetFlagID() != FlagID.INVALID)
            {
                switch (example.GetValueType())
                {
                    case Flag.ValueType.FLOAT:
                        fileSuffix = "Float";
                        break;
                    case Flag.ValueType.BOOL:
                        fileSuffix = "Bool";
                        break;
                    case Flag.ValueType.SYMBOL:
                        fileSuffix = "Symbol";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (example.GetValueType() == Flag.ValueType.INVALID)
                {
                    fileSuffix = "Unknown";
                }
                else
                {
                    fileSuffix = "Issue";
                }
            }

            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Flag" + fileSuffix + ".png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
