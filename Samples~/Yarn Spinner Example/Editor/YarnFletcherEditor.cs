using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Editor;

namespace Perell.Artemis.Example.YarnSpinnerIntegration.Editor
{
    [CustomEditor(typeof(YarnFletcher))]
    public class YarnFletcherEditor : FletcherEditor
    {
        SerializedProperty yarnList;

        protected override void OnEnable()
        {
            base.OnEnable();
            yarnList = serializedObject.FindProperty("yarnProjectsToCompile");
        }

        protected override void LayoutDataForCompilation()
        {
            EditorGUILayout.PropertyField(yarnList);
        }

        protected override string GetButtonString()
        {
            return "Compile Yarn Project files into database";
        }

        protected override void PerformButtonAction(PreDictionaryFletcher preDictionaryFletcher)
        {
            ((YarnFletcher)preDictionaryFletcher).CompileYarnProjects();
        }
    }
}