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
            Arrow arrow = (Arrow)target;

            EditorGUIUtility.SetIconForObject(arrow, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Arrow.png"));

            EditorGUI.BeginChangeCheck();

            //Delivery System
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Fletcher", arrow.GetFletcher(), typeof(PreDictionaryFletcher), false);
            EditorGUI.EndDisabledGroup();

            //ID
            string idDisplay = "" + arrow.GetArrowID();
            if (arrow.GetSymbolType() != null)
            {
                idDisplay = System.Enum.GetName(arrow.GetSymbolType(), arrow.GetArrowID());
            }
            EditorGUILayout.LabelField("Arrow ID", idDisplay);

            //Priority
            EditorGUILayout.Space();
            Arrow.HowPriorityCalculated howPriorityCalculated = arrow.GetHowPriorityCalculated();
            string priorityValueRepresentation = "";
            switch(howPriorityCalculated)
            {
                case Arrow.HowPriorityCalculated.SET_VALUE:
                    priorityValueRepresentation = "" + arrow.GetPriority();
                    break;
                case Arrow.HowPriorityCalculated.CRITERIA:
                    priorityValueRepresentation = PreDictionaryFletcher.CRITERIA_KEY_WORD + " = " + arrow.GetPriority();
                    break;
                case Arrow.HowPriorityCalculated.SUM:
                    priorityValueRepresentation = (arrow.GetPriority() - arrow.GetRuleSize()) + " + "+ PreDictionaryFletcher.CRITERIA_KEY_WORD + " = " + arrow.GetPriority();
                    break;
            }
            EditorGUILayout.LabelField("Priority Value", priorityValueRepresentation, EditorStyles.label);


            //Rule
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rule",EditorStyles.label);
            GUIStyle criterionStyle = new GUIStyle(EditorStyles.textArea);
            criterionStyle.alignment = TextAnchor.UpperCenter;
            string criterions = arrow.RecieveRuleStringRepresentation();
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
            EditorGUILayout.LabelField("How to Handle Busy", arrow.GetWhenBusyDescision().ToString());

            //Repaint Arrow
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(arrow);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
    }
}
