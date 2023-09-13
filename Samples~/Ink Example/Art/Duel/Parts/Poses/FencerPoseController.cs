using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration.Posing
{
    public class FencerPoseController : MonoBehaviour
    {
        [SerializeField]
        Transform chest, head, mainUpperarm, mainForearm, mainHand,
            sword, backUpperarm, backForearm, backHand,
            mainThigh, mainCalf, mainFoot,
            backThigh, backCalf, backFoot;
        [Space]
        public FencerPose pose;

        [ContextMenu("Save")]
        private void Save()
        {
            pose.chestLocalPosition = chest.localPosition;
            SaveRotation(chest, ref pose.chestRotation);
            SaveRotation(head, ref pose.headRotation);
            SaveRotation(mainUpperarm, ref pose.mainUpperarmRotation);
            SaveRotation(mainForearm, ref pose.mainForearmRotation);
            SaveRotation(mainHand, ref pose.mainHandRotation);
            SaveRotation(sword, ref pose.swordRotation);
            SaveRotation(backUpperarm, ref pose.backUpperarmRotation);
            SaveRotation(backForearm, ref pose.backForearmRotation);
            SaveRotation(backHand, ref pose.backHandRotation);
            SaveRotation(mainThigh, ref pose.mainThighRotation);
            SaveRotation(mainCalf, ref pose.mainCalfRotation);
            SaveRotation(mainFoot, ref pose.mainFootRotation);
            SaveRotation(backThigh, ref pose.backThighRotation);
            SaveRotation(backCalf, ref pose.backCalfRotation);
            SaveRotation(backFoot, ref pose.backFootRotation);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(pose);
#endif
        }

        private void SaveRotation(Transform transform, ref float rotation)
        {
            rotation = transform.localRotation.eulerAngles.z;
        }

        [ContextMenu("Load")]
        private void Load()
        {
            chest.localPosition = pose.chestLocalPosition;
            LoadRotation(chest, ref pose.chestRotation);
            LoadRotation(head, ref pose.headRotation);
            LoadRotation(mainUpperarm, ref pose.mainUpperarmRotation);
            LoadRotation(mainForearm, ref pose.mainForearmRotation);
            LoadRotation(mainHand, ref pose.mainHandRotation);
            LoadRotation(sword, ref pose.swordRotation);
            LoadRotation(backUpperarm, ref pose.backUpperarmRotation);
            LoadRotation(backForearm, ref pose.backForearmRotation);
            LoadRotation(backHand, ref pose.backHandRotation);
            LoadRotation(mainThigh, ref pose.mainThighRotation);
            LoadRotation(mainCalf, ref pose.mainCalfRotation);
            LoadRotation(mainFoot, ref pose.mainFootRotation);
            LoadRotation(backThigh, ref pose.backThighRotation);
            LoadRotation(backCalf, ref pose.backCalfRotation);
            LoadRotation(backFoot, ref pose.backFootRotation);
        }

        private void LoadRotation(Transform transform, ref float rotation)
        {
            transform.localRotation = Quaternion.Euler(0, 0, rotation);
        }

        private void Update()
        {
            if (pose)
            {
                Load();
            }
        }
    }
}