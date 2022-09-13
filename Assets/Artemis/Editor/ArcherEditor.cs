using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(Archer))]
    public class ArcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Archer e = (Archer)target;

            DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(e);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Archer example = (Archer)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/bowman.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
