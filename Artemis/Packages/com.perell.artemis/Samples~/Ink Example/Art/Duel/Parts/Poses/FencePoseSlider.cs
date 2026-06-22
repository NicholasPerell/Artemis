using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration.Posing
{
    public class FencePoseSlider : MonoBehaviour
    {
        [SerializeField]
        FencerPoseController player, roomie;

        [Space]
        [SerializeField]
        FencerPose[] up;
        [SerializeField]
        FencerPose[] down;
        [SerializeField]
        FencerPose neutral;
        
        [Space]
        [SerializeField]
        [Range(-5, 5)]
        public int points = 0;

        private void Update()
        {
            if(points == 0)
            {
                player.pose = neutral;
                roomie.pose = neutral;
            }
            else if(points > 0)
            {
                player.pose = up[points - 1];
                roomie.pose = down[points - 1];
            }
            else
            {
                player.pose = down[-points - 1];
                roomie.pose = up[-points - 1];
            }
        }
    }
}