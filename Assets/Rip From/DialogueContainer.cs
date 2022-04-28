using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
enum DialoguePriority
{
    UNSELECTED = 0,
    CUSTOM_STORY_PIVOTAL = 6,
    STORY_PIVOTAL = 5,
    CONTINUED_DIALOGUE = 4,
    KEY_CONTEXT_TRIVIA = 3,
    CONTEXT_SPECIFIC_TRIVIA = 2,
    GENERAL_CHATTER = 1
}

enum DialogueStatus
{
    CURRENTLY_PLAYING,
    INTERRUPTED,
    UNTOUCHED,
    FINISHED
}
public class DialogueContainer: ScriptableObject
{
    TextAsset inkText;



    // Everything past here is just for displaying stuff
#if UNITY_EDITOR
    [CustomEditor(typeof(DialogueContainer))]
    public class DialogueContainer_Editor : Editor
    {
        public override void OnInspectorGUI()
        {

            
            DrawDefaultInspector(); // for other non-HideInInspector fields

            DialogueContainer script = (DialogueContainer)target;

            //script.type = (ELEMENTAL_TYPES)EditorGUILayout.EnumPopup("Elemental type", script.type);

                //script.effectTag = EditorGUILayout.ObjectField(new GUIContent("Lit Tag", "The tag is applied to any object that is ends up being lit by this light source"),
                //    script.effectTag, typeof(Tag), true) as Tag;

            EditorUtility.SetDirty(script);
        }
    }
#endif
}
