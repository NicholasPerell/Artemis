using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    public class GoddessEditorWindow : EditorWindow
    {
        UnityEditor.Editor editor;

        [MenuItem("Window/Artemis Goddess")]
        static void Init()
        {
            //Get first existing window of this type, or (if none) make a new one:
            GoddessEditorWindow window = (GoddessEditorWindow)GetWindow(typeof(GoddessEditorWindow));
            window.titleContent = new GUIContent("Goddess", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/Goddess.png"));
            window.Show();
        }

        void OnGUI()
        {
            if(editor == null)
            {
                editor = GoddessEditor.CreateEditor(Goddess.instance);
            }
            editor.OnInspectorGUI();
        }   
    }
}