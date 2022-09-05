using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExpLvl))]
public class LevelShowScripts : Editor
{
    public override void OnInspectorGUI()
    {
        ExpLvl myExpLvl = (ExpLvl)target;


        //DrawDefaultInspector();

        myExpLvl.experience = EditorGUILayout.IntField("Experience", myExpLvl.experience);
        EditorGUILayout.LabelField("Level", myExpLvl.Level.ToString());

        EditorGUILayout.HelpBox("This is a help box", MessageType.Info);

        if (GUILayout.Button("Press me!"))
        {
            Debug.Log("Button pressed");
        }

        EditorGUILayout.Space();

        myExpLvl.loops = EditorGUILayout.Toggle("Loops", myExpLvl.loops);

        if(myExpLvl.loops)
        {
            myExpLvl.includeBundlesInLoop = EditorGUILayout.Toggle("  Include Bundles", myExpLvl.includeBundlesInLoop);
        }

    }
}
