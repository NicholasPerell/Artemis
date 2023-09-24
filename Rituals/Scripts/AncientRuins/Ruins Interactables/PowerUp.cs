using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class PowerUp : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer icon;
        [SerializeField]
        SpriteRenderer background;
        [Space]
        [SerializeField]
        Color magicTint, demonicTint;

        bool initialized = false;
#if UNITY_EDITOR
        [SerializeField]
#endif
        PlayerAbilityData playerAbility;
        [Space]
        [SerializeField]
        Archer tutorialArcher;


#if UNITY_EDITOR
        private void Awake()
        {
            if(playerAbility != null)
            {
                Initialize(playerAbility);
            }
        }

        private void OnValidate()
        {
            if (playerAbility != null)
            {
                SetUpVisuals();
            }
            else
            {
                icon.sprite = null;
                background.color = Color.white;
                icon.enabled = false;
                background.enabled = false;
            }
        }
#endif

        public void Initialize(PlayerAbilityData _playerAbility)
        {
            playerAbility = _playerAbility;
            SetUpVisuals();
            initialized = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(initialized && other.tag == "Player")
            {
                other.GetComponent<PlayerController>().GainAbility(playerAbility);
                tutorialArcher.IgnoreSuccessAttemptDelivery();
            }

            Destroy(this.gameObject);
        }

        private void SetUpVisuals()
        {
            icon.sprite = playerAbility.WheelIcon;

            Color tint = magicTint;
            if (playerAbility.IsDemonic)
            {
                tint = demonicTint;
            }
            background.color = tint;

            icon.enabled = true;
            background.enabled = true;
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if(playerAbility == null)
            {
                Gizmos.color = magicTint;
                Gizmos.DrawSphere(transform.position,GetComponent<SphereCollider>().radius);
            }
        }
#endif
    }
}