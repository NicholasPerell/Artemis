using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Artemis.Editor {
    public class ArtemisMenuItems
    {
        [MenuItem("Assets/Create/Artemis/Archer")]
        private static void CreateArcher()
        {
            CreateArtemisItem(new Archer(), "Archer");
        }

        [MenuItem("Assets/Create/Artemis/Bundle")]
        private static void CreateBundle()
        {
            CreateArtemisItem(new Bundle(), "Bundle");
        }

        [MenuItem("Assets/Create/Artemis/Flag")]
        private static void CreateFlag()
        {
            CreateArtemisItem(new Flag(), "Flag");
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
        }
    }
}
