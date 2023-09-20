using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Perell.Artemis.Example.Rituals.Editor
{
    public class ArtemisMenuItems
    {
        [MenuItem("Assets/Rituals/Kill Player %#k")]
        private static void KillPlayer()
        {
            if(AncientRuinsManager.IsActive)
            {
                AncientRuinsManager.PlayerController.Health.Die();
            }
        }
    }
}