using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Editor
{
    public class IconObjectEditor : UnityEditor.Editor
    {
        protected void SetIcon(string fileName)
        {
            EditorGUIUtility.SetIconForObject(target, LoadIconTexture(fileName));
        }

        protected Texture2D LoadIconTexture(string fileName)
        {
            Texture2D iconTexture;
            iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.perell.artemis/Editor/Icons/" + fileName + ".png");
            if (iconTexture == null)
            {
                iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Artemis/Editor/Icons/" + fileName + ".png");
            }
            return iconTexture;
        }
    }
}