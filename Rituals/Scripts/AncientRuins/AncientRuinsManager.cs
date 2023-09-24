using Perell.Artemis.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
        Archer runStarted;
        [SerializeField]
        EscapeRune escapeRune;

        [Space]
        [Header("Demon")]
        [SerializeField]
        DemonSpiritController demonSpirit;
        [SerializeField]
        AntipossesionScrollController antipossesionScoll;
        [SerializeField]
        Archer demonSpawned;
        [SerializeField]
        Archer possessed;

        [Space]
        [Header("Dungeon")]
        [SerializeField]
        Transform dungeon;

        [Space]
        [Header("Narrative Data")]
        [SerializeField]
        FlagBundle gameOverallFlags;
        [SerializeField]
        FlagBundle runSpecificFlags;
        [SerializeField]
        Constellation runSpecificSave;
        [SerializeField]
        TextAsset runSpecificDefaultValues;
        [SerializeField]
        Flag runsAttempted;

        public static event UnityAction<bool> OnPossessed;

        public event UnityAction OnEscaped;

        static AncientRuinsManager instance;

        public static bool IsActive { get { return instance.isActiveAndEnabled; } }

        public static Transform Player { get { return instance.player; } }
        public static PlayerController PlayerController { get { return instance.playerController; } }
        public static Transform Dungeon { get { return instance.dungeon; } }

        Controls.SorcererInputs inputActions;

        private void Awake()
        {
            inputActions = new Controls.SorcererInputs();
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            instance = this;
        }

        private void OnEnable()
        {
            runsAttempted.SetValue(runsAttempted.GetValue() + 1);

            runStarted.AttemptDelivery(new FlagBundle[]{ gameOverallFlags, runSpecificFlags });
            
            if(runSpecificSave && runSpecificDefaultValues)
            {
                runSpecificSave.LoadFromTextAsset(runSpecificDefaultValues);
            }
            
            playerController.Health.OnHealthLost += RespondToHealthLost;
            playerController.Corruption.OnCorrupted += RespondToCorrupted;
            demonSpirit.OnCaughtPlayer += RespondToCaughtPlayer;
            antipossesionScoll.OnScrollComplete += RespondToScrollComplete;
            escapeRune.OnComplete += RespondToEscapeRuneComplete;

            inputActions.Dungeon.DEBUGESCAPE.Enable();
            inputActions.Dungeon.DEBUGESCAPE.performed += DebugKill;
        }

        private void OnDisable()
        {
            playerController.Health.OnHealthLost -= RespondToHealthLost;
            playerController.Corruption.OnCorrupted -= RespondToCorrupted;
            demonSpirit.OnCaughtPlayer -= RespondToCaughtPlayer;
            antipossesionScoll.OnScrollComplete -= RespondToScrollComplete;
            escapeRune.OnComplete -= RespondToEscapeRuneComplete;
            OnPossessed?.Invoke(false);

            inputActions.Dungeon.DEBUGESCAPE.performed -= DebugKill;
            inputActions.Dungeon.DEBUGESCAPE.Disable();
        }

        private void RespondToHealthLost()
        {
            PreformEscapeRune();
        }

        private void PreformEscapeRune()
        { 
            Time.timeScale = 0;
            escapeRune.gameObject.SetActive(true);
        }

        private void RespondToCorrupted()
        {
            SpawnSpirit();
        }

        private void SpawnSpirit()
        {
            demonSpawned.AttemptDelivery(new FlagBundle[] { gameOverallFlags, runSpecificFlags });
            demonSpirit.transform.parent = dungeon;
            demonSpirit.gameObject.SetActive(true);
        }

        private void RespondToCaughtPlayer()
        {
            demonSpirit.gameObject.SetActive(false);
            OnPossessed?.Invoke(true);
            possessed.IgnoreSuccessAttemptDelivery();
            antipossesionScoll.gameObject.SetActive(true);
        }

        private void RespondToScrollComplete()
        {
            antipossesionScoll.gameObject.SetActive(false);
            OnPossessed?.Invoke(false);
        }

        private void RespondToEscapeRuneComplete()
        {
            antipossesionScoll.gameObject.SetActive(false);
            escapeRune.gameObject.SetActive(false);
            OnPossessed?.Invoke(false);
            OnEscaped?.Invoke();
        }

        private void DebugKill(InputAction.CallbackContext callbackContext)
        {
            playerController.Health.Die();
        }
    }
}