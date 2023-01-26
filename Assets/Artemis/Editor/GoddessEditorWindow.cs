using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    public class GoddessEditorWindow : EditorWindow
    {
        [MenuItem("Window/Artemis Goddess")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            GoddessEditorWindow window = (GoddessEditorWindow)EditorWindow.GetWindow(typeof(GoddessEditorWindow));
            window.titleContent = new GUIContent("Goddess");
            window.Show();
        }

        Editor editor;
        void OnGUI()
        {
            editor ??= GoddessEditor.CreateEditor(Goddess.instance);
            editor.OnInspectorGUI();
        }

        
    }
}
