using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Flag))]
    public class FlagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Flag e = (Flag)target;

            EditorGUI.BeginChangeCheck();

            Flag.ValueType valueType = e.GetValueType();
            FlagID flagId = e.GetFlagId();

            //Turn this into a check inside its class?
            if (flagId.ToString() == ((int)flagId).ToString())
            {
                e.SetFlagId(FlagID.INVALID);
                flagId = FlagID.INVALID;

                //Repaint!
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }

            if (flagId != FlagID.INVALID)
            {
                EditorGUILayout.LabelField("Flag ID", flagId.ToString());

                switch (valueType)
                {
                    case Flag.ValueType.FLOAT:
                        e.SetValue(EditorGUILayout.FloatField("Value", e.GetValue()));
                        break;
                    case Flag.ValueType.BOOL:
                        e.SetValue(EditorGUILayout.Toggle("Value", e.GetValue() == 1));
                        break;
                    case Flag.ValueType.SYMBOL:
                        //TODO: Convert to flag's compiled type
                        e.SetValue((float)(ValveInternalSymbols)EditorGUILayout.EnumPopup("Value", (ValveInternalSymbols)(e.GetValue())));

                        break;
                    default:
                        break;
                }
            }
            else
            {
                e.SetFlagId((FlagID)EditorGUILayout.EnumPopup("Flag ID", flagId));

                if (e.GetFlagId() != FlagID.INVALID)
                {
                    Flag.ValueType temp = Goddess.instance.GetFlagValueType(e.GetFlagId());
                    e.SetValueType(temp);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
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

            if(example.GetFlagId() != FlagID.INVALID)
            {
                switch(example.GetValueType())
                {
                    case Flag.ValueType.FLOAT:
                        fileSuffix = " Float";
                        break;
                    case Flag.ValueType.BOOL:
                        fileSuffix = " Bool";
                        break;
                    case Flag.ValueType.SYMBOL:
                        fileSuffix = " Symbol";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if(example.GetValueType() == Flag.ValueType.INVALID)
                {
                    fileSuffix = " Unknown";
                }
                else
                {
                    fileSuffix = " Issue";
                }
            }

            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/ArtemisFlag Icon" + fileSuffix + ".png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
