using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Example.Rituals.Controls;

namespace Perell.Artemis.Example.Rituals
{
    public class LobbyNpc : MonoBehaviour
    {
        [SerializeField]
        float distanceToInteractWithPlayer;
        [SerializeField]
        GameObject icon;
        [SerializeField]
        Archer archer;
        [SerializeField]
        FlagBundle[] bundles;

        bool talkedToPlayer;

        SorcererInputs inputs;

        private void Awake()
        {
            inputs = new SorcererInputs();
        }

        private void OnEnable()
        {
            talkedToPlayer = false;
            inputs.Narrative.Enable();
            inputs.Narrative.Interact.performed += RespondToInteractAttempted;
        }

        private void OnDisable()
        {
            inputs.Narrative.Interact.performed -= RespondToInteractAttempted;
            inputs.Narrative.Disable();
        }

        private void Update()
        {
            SetEIcon();
        }

        private void SetEIcon()
        {
            icon.SetActive(CanBeTalkedTo());
        }

        private void RespondToInteractAttempted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if(CanBeTalkedTo())
            {
                talkedToPlayer = true;
                Debug.Log("Input Attempt Success");
                archer.AttemptDelivery(bundles);
            }
        }

        private bool CanBeTalkedTo()
        {
            return !talkedToPlayer && Time.timeScale != 0 && Vector3.Distance(LobbyManager.Player.position, transform.position) <= distanceToInteractWithPlayer;
        }
    }
}