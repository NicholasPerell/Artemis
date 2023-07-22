using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class AncientRuinsManager : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField]
        Transform player;
        [SerializeField]
        PlayerController playerController;
        [SerializeField]
        PlayerHealth playerHealth;
        [SerializeField]
        PlayerMana playerMana;
        [SerializeField]
        PlayerCorruption playerCorruption;

        [Space]
        [Header("Dungeon")]
        [SerializeField]
        Transform dungeon;

        static AncientRuinsManager instance;
        public static Transform Player { get { return instance.player; } }
        public static PlayerController PlayerController { get { return instance.playerController; } }
        public static PlayerHealth PlayerHealth { get { return instance.playerHealth; } }
        public static PlayerMana PlayerMana { get { return instance.playerMana; } }
        public static PlayerCorruption PlayerCorruption { get { return instance.playerCorruption; } }
        public static Transform Dungeon { get { return instance.dungeon; } }

        private void Awake()
        {
            instance = this;
        }

    }
}