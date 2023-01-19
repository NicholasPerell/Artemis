using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Arrow))]
    public class ArrowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Arrow e = (Arrow)target;

            //DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();

            //Delivery System
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Fletcher", e.GetFletcher(), typeof(PreDictionaryFletcher), false);
            EditorGUI.EndDisabledGroup();

            //ID
            EditorGUILayout.LabelField("Arrow ID", e.GetArrowID()+"");

            //Priority
            EditorGUILayout.Space();

            Arrow.HowPriorityCalculated howPriorityCalculated = e.GetHowPriorityCalculated();
            string priorityValueRepresentation = "";
            switch(howPriorityCalculated)
            {
                case Arrow.HowPriorityCalculated.SET_VALUE:
                    priorityValueRepresentation = "" + e.GetPriority();
                    break;
                case Arrow.HowPriorityCalculated.CRITERIA:
                    priorityValueRepresentation = PreDictionaryFletcher.CRITERIA_KEY_WORD + " = " + e.GetPriority();
                    break;
                case Arrow.HowPriorityCalculated.SUM:
                    priorityValueRepresentation = (e.GetPriority() - e.GetRuleSize()) + " + "+ PreDictionaryFletcher.CRITERIA_KEY_WORD + " = " + e.GetPriority();
                    break;
            }
            EditorGUILayout.LabelField("Priority Value", priorityValueRepresentation, EditorStyles.label);


            //Rule
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rule",EditorStyles.label);
            GUIStyle criterionStyle = new GUIStyle(EditorStyles.label);
             criterionStyle = new GUIStyle(EditorStyles.textArea);
            criterionStyle.alignment = TextAnchor.UpperCenter;
            string criterions = e.RecieveRuleStringRepresentation();
            if(criterions.Length > 0)
            {
                EditorGUILayout.LabelField(criterions, criterionStyle);
            }
            else
            {
                EditorGUILayout.LabelField("No Criteria Required", criterionStyle);
            }

            //How To Handle Busy
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("How to Handle Busy", e.GetWhenBusyDescision().ToString());

            //Repaint Arrow
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Arrow example = (Arrow)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/branch-arrow.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
