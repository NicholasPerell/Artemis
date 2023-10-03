using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Editor;

namespace Perell.Artemis.Example.InkIntegration.Editor
{
    [CustomEditor(typeof(InkFletcher))]
    public class InkFletcherEditor : FletcherEditor
    {
        SerializedProperty inkList;

        protected override void OnEnable()
        {
            base.OnEnable();
            inkList = serializedObject.FindProperty("inksToCompile");
        }

        protected override void LayoutDataForCompilation()
        {
            EditorGUILayout.PropertyField(inkList);
        }

        protected override string GetButtonString()
        {
            return "Compile Ink files into database";
        }

        protected override void PerformButtonAction(PreDictionaryFletcher preDictionaryFletcher)
        {
            ((InkFletcher)preDictionaryFletcher).CompileInks();
        }

    }
}