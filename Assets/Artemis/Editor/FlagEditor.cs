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

            //For Debugging Purposes
            e.SetValueType((Flag.ValueType)EditorGUILayout.EnumPopup("Type",valueType));


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
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/ArtemisFlag Icon.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
