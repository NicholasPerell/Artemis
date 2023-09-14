using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Perell.Artemis.Generated;

namespace Perell.Artemis.Example.InkIntegration
{
    public class InkExampleInitializer : MonoBehaviour
    { 
#if UNITY_EDITOR
        enum InitSteps
        {
            EMPTY,
            FLETCHER,
            ARCHER_FLAGS,
            INITIALIZED,
            FLAGS_DELETE
        }

        [SerializeField]
        InkFletcher fletcher;
        [SerializeField]
        FlagBundle flagBundle;
        [SerializeField]
        Archer archer;

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
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            InkExampleInitializer instance = GameObject.FindObjectOfType<InkExampleInitializer>();

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
            fletcher.CompileInks();        }

        private void GenerateFlagAssets()
        {
            string bundleLocation = AssetDatabase.GetAssetPath(flagBundle);
            string flagLocation = bundleLocation.Substring(0, bundleLocation.LastIndexOf('/'));
            //flagLocation = flagLocation.Substring(0, flagLocation.LastIndexOf('/'));
            AssetDatabase.CreateFolder(flagLocation, "Flags");

            flagBundle.Remove(null);
            //flagBundle = FlagBundle.CreateInstance();
            Flag flag;
            foreach (FlagID flagID in System.Enum.GetValues(typeof(FlagID)))
            {
                if (flagID != FlagID.INVALID)
                {
                    flag = ScriptableObject.CreateInstance<Flag>();
                    flag.InitFlag(flagID);

                    AssetDatabase.DeleteAsset(flagLocation + "/Flags/" + flagID.ToString() + ".asset");
                    AssetDatabase.CreateAsset(flag, flagLocation + "/Flags/" + flagID.ToString() + ".asset");
                    AssetDatabase.ImportAsset(flagLocation + "/Flags/" + flagID.ToString() + ".asset");

                    flagBundle.Add(flag);
                }
            }
        }

        private void LoadArrowsToArcher()
        {
            Arrow[] arrowsGenerated = fletcher.RetrieveAllGeneratedArrows();

            archer.defaultContents.Clear();
            archer.defaultContents.AddRange(arrowsGenerated);
            archer.Init();
        }

        private void ClearOutData()
        {
            //TODO: Remove arrows & flagIDs using fletcher

            //Remove Flag Assets
            string bundleLocation = AssetDatabase.GetAssetPath(flagBundle);
            string flagLocation = bundleLocation.Substring(0, bundleLocation.LastIndexOf('/') + 1);
            AssetDatabase.DeleteAsset(flagLocation + "Flags");
            AssetDatabase.Refresh();
        }
#endif
    }
}