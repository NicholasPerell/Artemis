using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerAbilityData : ScriptableObject
    {
        public enum PrefabIntendedParent
        {
            PLAYER,
            TILEMAP
        }

        [Header("Stats")]
        [SerializeField]
        private int manaCost;
        public int ManaCost { get { return manaCost; } }
        [SerializeField]
        [Range(0,1)]
        private float corruptionAdd;
        public float CorruptionAdd { get { return corruptionAdd; } }
        public bool IsDemonic => corruptionAdd > 0;
        [SerializeField]
        private float cooldownTime;
        public float CooldownTime { get { return cooldownTime; } }

        [Header("Instantiation")]
        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab { get { return prefab; } }
        [SerializeField]
        private PrefabIntendedParent prefabParent;
        public PrefabIntendedParent PrefabParent { get { return prefabParent; } }

        [Header("UI")]
        [SerializeField]
        Sprite wheelIcon;
        public Sprite WheelIcon { get { return wheelIcon; } }
    }
}