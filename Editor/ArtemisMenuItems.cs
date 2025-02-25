using System.Collections;
using UnityEngine;
using UnityEditor;
using Perell.Artemis.Generated;
using Perell.Artemis.Saving;

namespace Perell.Artemis.Editor 
{
    public class ArtemisMenuItems
    {

        [MenuItem("Assets/Create/Artemis/Archer")]
        private static void CreateArcher()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Archer>(), "Archer");
        }

        [MenuItem("Assets/Create/Artemis/Arrow Bundle")]
        private static void CreateBundle()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<ArrowBundle>(), "Arrow Bundle");
        }

        [MenuItem("Assets/Create/Artemis/Flags/Flag")]
        private static void CreateFlag()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Flag>(), "Flag");
        }

        [MenuItem("Assets/Create/Artemis/Flags/Flags of Every ID")]
        private static void CreateFlagsOfEveryId()
        {
            Object obj = Selection.activeObject;
            string folderPath = AssetDatabase.GetAssetPath(obj);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                folderPath = folderPath.Substring(0, folderPath.LastIndexOf('/'));
            }

            FlagID[] flagIDs = Goddess.instance.GetFlagIDs();
            Flag flag;
            Object created;
            for (int i = 0; i < flagIDs.Length; i++)
            {
                flag = ScriptableObject.CreateInstance<Flag>();
                //TO DO: Convert this to a InitFlag(ID) method
                flag.InitFlag(flagIDs[i]);
                created = flag;
                AssetDatabase.CreateAsset(created, folderPath + "/" + flagIDs[i].ToString() + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/Artemis/Flags/Flag Bundle")]
        private static void CreateFlagState()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<FlagBundle>(), "Flag Bundle");
        }

        [MenuItem("Assets/Create/Artemis/Constellation")]
        private static void CreateConstellation()
        {
            CreateArtemisItem(ScriptableObject.CreateInstance<Constellation>(), "Constellation");
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
