using Perell.Artemis.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine;
using System.Threading.Tasks;

namespace Perell.Artemis.Example.DebugConsole
{
    public class DebugExampleInitializer : MonoBehaviour
    {
#if UNITY_EDITOR
        enum InitSteps
        {
            EMPTY,
            FLETCHER,
            ARCHER_FLAGS,
            INITIALIZED,
            FLAGS_DELETE,
            FLAG_FOLDER,
        }


        [SerializeField]
        ArtemisDebugExampleFletcher fletcher;
        [SerializeField]
        Archer archer;
        [SerializeField]
        ArrowBundle arrowBundle;

        [HideInInspector]
        [SerializeField]
        InitSteps currentStep = InitSteps.EMPTY;

        [ContextMenu("Initialize")]
        public void InitializeExample()
        {
            if (currentStep == InitSteps.EMPTY)
            {
                currentStep = InitSteps.FLETCHER;
                CheckForSteps();
            }
        }

        [ContextMenu("Deinitialize")]
        public void DeinitializeExample()
        {
            if (currentStep == InitSteps.INITIALIZED)
            {
                currentStep = InitSteps.FLAGS_DELETE;
                CheckForSteps();
            }
            CheckForSteps();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            DebugExampleInitializer instance = GameObject.FindObjectOfType<DebugExampleInitializer>();

            if (instance != null)
            {
                instance.CheckForSteps();
            }
        }

        private void CheckForSteps()
        {
            switch (currentStep)
            {
                case InitSteps.FLETCHER:
                    TriggerFletcher();
                    currentStep = InitSteps.ARCHER_FLAGS;
                    break;
                case InitSteps.ARCHER_FLAGS:
                    LoadArrowsToArcher();
                    GenerateFlagAssets();
                    currentStep = InitSteps.INITIALIZED;
                    break;
                case InitSteps.FLAGS_DELETE:
                    ClearOutData();
                    currentStep = InitSteps.EMPTY;
                    break;
            }
        }
        private void TriggerFletcher()
        {
            fletcher.GeneratorArrowDatabase();
        }

        private void GenerateFlagAssets()
        {
            string bundleLocation = AssetDatabase.GetAssetPath(arrowBundle);
            string flagLocation = bundleLocation.Substring(0, bundleLocation.LastIndexOf('/'));
            flagLocation = flagLocation.Substring(0, flagLocation.LastIndexOf('/'));
            AssetDatabase.CreateFolder(flagLocation, "Flags");
            flagLocation += "/Flags";

            FlagBundle flagBundle = FlagBundle.CreateInstance();
            Flag flag;
            foreach (FlagID flagID in System.Enum.GetValues(typeof(FlagID)))
            {
                if (flagID != FlagID.INVALID)
                {
                    flag = ScriptableObject.CreateInstance<Flag>();
                    flag.InitFlag(flagID);

                    if (flagID.ToString().Contains("TRUE"))
                    {
                        flag.SetValue(true);
                    }

                    AssetDatabase.DeleteAsset(flagLocation + "/" + flagID.ToString() + ".asset");
                    AssetDatabase.CreateAsset(flag, flagLocation + "/" + flagID.ToString() + ".asset");
                    AssetDatabase.ImportAsset(flagLocation + "/" + flagID.ToString() + ".asset");

                    flagBundle.Add(flag);
                }
            }
            AssetDatabase.DeleteAsset(flagLocation + "/DebugConsoleFlagBundle.asset");
            AssetDatabase.CreateAsset(flagBundle, flagLocation + "/DebugConsoleFlagBundle.asset");
            AssetDatabase.ImportAsset(flagLocation + "/DebugConsoleFlagBundle.asset");

            FlagBundle[] newGlobalBundles = new FlagBundle[Goddess.instance.globallyLoadedFlagBundles.Length + 1];
            for (int i = 0; i < Goddess.instance.globallyLoadedFlagBundles.Length; i++)
            {
                newGlobalBundles[i] = Goddess.instance.globallyLoadedFlagBundles[i];
            }
            newGlobalBundles[Goddess.instance.globallyLoadedFlagBundles.Length] = flagBundle;
            Goddess.instance.globallyLoadedFlagBundles = newGlobalBundles;
        }

        private void LoadArrowsToArcher()
        {
            Arrow[] arrowsGenerated = fletcher.RetrieveAllGeneratedArrows();

            archer.defaultContents.Clear();
            archer.defaultContents.AddRange(arrowsGenerated);
            archer.Init();

            string bundleLocation = AssetDatabase.GetAssetPath(arrowBundle);
            arrowBundle = ArrowBundle.CreateInstance(new Arrow[] { arrowsGenerated[0], arrowsGenerated[1] });
            AssetDatabase.DeleteAsset(bundleLocation);
            AssetDatabase.CreateAsset(arrowBundle, bundleLocation);
            AssetDatabase.ImportAsset(bundleLocation);
        }

        private void ClearOutData()
        {
            //TODO: Remove arrows & flagIDs using fletcher

            //Remove Flag Bundle On End of Global States
            if (Goddess.instance.globallyLoadedFlagBundles.Length > 0)
            {
                FlagBundle[] newGlobalBundles = new FlagBundle[Goddess.instance.globallyLoadedFlagBundles.Length - 1];
                for (int i = 0; i < newGlobalBundles.Length; i++)
                {
                    newGlobalBundles[i] = Goddess.instance.globallyLoadedFlagBundles[i];
                }
                Goddess.instance.globallyLoadedFlagBundles = newGlobalBundles;
            }

            //Remove Flag Assets
            string bundleLocation = AssetDatabase.GetAssetPath(arrowBundle);
            string flagLocation = bundleLocation.Substring(0, bundleLocation.LastIndexOf('/')) + "/../";
            AssetDatabase.DeleteAsset(flagLocation + "Flags");
            AssetDatabase.Refresh();
        }
#endif
    }
}