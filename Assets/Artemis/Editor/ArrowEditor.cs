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

            EditorGUIUtility.SetIconForObject(e, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Arrow.png"));

            EditorGUI.BeginChangeCheck();

            //Delivery System
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Fletcher", e.GetFletcher(), typeof(PreDictionaryFletcher), false);
            EditorGUI.EndDisabledGroup();

            //ID
            string idDisplay = "" + e.GetArrowID();
            if (e.GetSymbolType() != null)
            {
                idDisplay = System.Enum.GetName(e.GetSymbolType(), e.GetArrowID());
            }
            EditorGUILayout.LabelField("Arrow ID", idDisplay);

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
            GUIStyle criterionStyle = new GUIStyle(EditorStyles.textArea);
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
    }
}
