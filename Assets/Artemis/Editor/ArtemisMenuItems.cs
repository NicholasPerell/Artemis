using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Artemis.EditorIntegration 
{
    public class ArtemisMenuItems
    {
        [MenuItem("Assets/Create/Artemis/Archer")]
        private static void CreateArcher()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Archer>(), "Archer");
        }

        [MenuItem("Assets/Create/Artemis/Bundle")]
        private static void CreateBundle()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Bundle>(), "Bundle");
        }

        [MenuItem("Assets/Create/Artemis/Flag")]
        private static void CreateFlag()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Flag>(), "Flag");
        }

        private static void CreateArtemisItem(Object created, string type)
        {
            Object obj = Selection.activeObject;

            string folderPath = AssetDatabase.GetAssetPath(obj);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                folderPath = folderPath.Substring(0, folderPath.LastIndexOf('/'));
            }

            ProjectWindowUtil.CreateAsset(created, folderPath + "/New Artemis "+type+".asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }
    }
}
