using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class AncientRuinsManager : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField]
        Transform player;
        [SerializeField]
        PlayerController playerController;

        [Space]
        [Header("Demon")]
        [SerializeField]
        DemonSpiritController demonSpirit;
        [SerializeField]
        AntipossesionScrollController antipossesionScoll;

        [Space]
        [Header("Dungeon")]
        [SerializeField]
        Transform dungeon;

        public static event UnityAction<bool> OnPossessed;

        static AncientRuinsManager instance;
        public static Transform Player { get { return instance.player; } }
        public static PlayerController PlayerController { get { return instance.playerController; } }
        public static Transform Dungeon { get { return instance.dungeon; } }

        private void Awake()
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            instance = this;
        }

        private void OnEnable()
        {
            playerController.Corruption.OnCorrupted += RespondToCorrupted;
            demonSpirit.OnCaughtPlayer += RespondToCaughtPlayer;
            antipossesionScoll.OnScrollComplete += RespondToScrollComplete;
        }

        private void OnDisable()
        {
            playerController.Corruption.OnCorrupted -= RespondToCorrupted;
            demonSpirit.OnCaughtPlayer -= RespondToCaughtPlayer;
            antipossesionScoll.OnScrollComplete -= RespondToScrollComplete;
        }

        private void RespondToCorrupted()
        {
            SpawnSpirit();
        }

        private void SpawnSpirit()
        {
            demonSpirit.transform.parent = dungeon;
            demonSpirit.gameObject.SetActive(true);
        }

        private void RespondToCaughtPlayer()
        {
            demonSpirit.gameObject.SetActive(false);
            OnPossessed?.Invoke(true);
            antipossesionScoll.gameObject.SetActive(true);
        }

        private void RespondToScrollComplete()
        {
            antipossesionScoll.gameObject.SetActive(false);
            OnPossessed?.Invoke(false);
        }
    }
}