using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration
{
    [CustomEditor(typeof(PreDictionaryFletcher))]
    public class PreDictionaryFletcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PreDictionaryFletcher e = (PreDictionaryFletcher)target;

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
            PreDictionaryFletcher example = (PreDictionaryFletcher)target;

            if (example == null)
                return null;

            Texture2D tex = new Texture2D(width, height);
            Texture2D copyFrom;

            //TODO: figure out a way to give fletcher coded children a way to have the nice icon.

            //Figure out why Resources works in the Ink Package but not in Artemis
            //copyFrom = AssetDatabase.Resources<Texture2D>("ArcherFileIcon-Large.png");
            copyFrom = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Resources/table.png");

            EditorUtility.CopySerialized(copyFrom, tex);

            return tex;
        }
    }
}
