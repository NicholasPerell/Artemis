using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(MockUpArcher))]
    public class MockUpArcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {


            MockUpArcher mockUp = (MockUpArcher)target;


            //DrawDefaultInspector();

            EditorGUILayout.HelpBox("This is a help box", MessageType.Info);

            if (GUILayout.Button("Press me!"))
            {
                Debug.Log("Button pressed");
            }

            EditorGUILayout.Space();


            mockUp.SetChoosingSamePriority((Archer.ChooseSamePriority)EditorGUILayout.EnumPopup("Handling Same Priority", mockUp.GetChoosingSamePriority()));

            EditorGUILayout.Space();

            mockUp.loops = EditorGUILayout.Toggle("Loops", mockUp.loops);

            if (mockUp.loops)
            {
                mockUp.includeBundlesInLoop = EditorGUILayout.Toggle("\u21B3 Include Bundles", mockUp.includeBundlesInLoop);
                mockUp.includeHigherPrioritiesInLoop = EditorGUILayout.Toggle("\u21B3 Include Higher Priorities", mockUp.includeHigherPrioritiesInLoop);
            }

        }
    }
}
