using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration.Posing
{
    [CreateAssetMenu(fileName = "New Pose", menuName = "Fencer Pose")]
    public class FencerPose : ScriptableObject
    {
        public Vector3 chestLocalPosition;
        [Range(0,360)]
        public float chestRotation, headRotation, mainUpperarmRotation, mainForearmRotation, mainHandRotation,
            swordRotation, backUpperarmRotation, backForearmRotation, backHandRotation,
            mainThighRotation, mainCalfRotation, mainFootRotation,
            backThighRotation, backCalfRotation, backFootRotation;
    }
}