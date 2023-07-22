using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        PlayerHealth health;
        [SerializeField]
        PlayerMana mana;
        [SerializeField]
        PlayerCorruption corruption;
        [SerializeField]
        PlayerMovement movement;

        [SerializeField]
        List<PlayerAbilityData> playerAbilities;
        int playerAbilityScroll = 0;

        [SerializeField]
        PlayerAbilityData[] demonsAbilities;

        float cooldownTimer = 0;

        bool possessed = false;
        public bool IsPossessed { get { return possessed; } }

        public Vector3 PlayerPosition => transform.position;
        public Vector3 MousePosition
        {
            get
            {
                Vector3 result = movement.MousePosition;
                result.y = PlayerPosition.y;
                return result;
            }
        }

        private void OnEnable()
        {

        }

        private void Update()
        {
            RunTimers();
            CheckInputs();
        }

        private void RunTimers()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer < 0)
                {
                    cooldownTimer = 0;
                }
            }
        }

        private void CheckInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttemptUseAbility(playerAbilities[playerAbilityScroll]);
            }
            if (Input.GetMouseButtonUp(0))
            {
                AttemptReleaseAbility(playerAbilities[playerAbilityScroll]);
            }
        }

        private void AttemptUseAbility(PlayerAbilityData abilityData)
        {
            if (cooldownTimer == 0)
            {
                if (mana.SpendMana(abilityData.ManaCost))
                {
                    cooldownTimer = abilityData.CooldownTime;
                    corruption.AddToCorruptionChance(abilityData.CorruptionAdd);

                    GameObject abilityObj = Instantiate(abilityData.Prefab, transform.position, Quaternion.identity);

                    switch (abilityData.PrefabParent)
                    {
                        case PlayerAbilityData.PrefabIntendedParent.PLAYER:
                            abilityObj.transform.parent = transform;
                            break;
                        case PlayerAbilityData.PrefabIntendedParent.TILEMAP:
                            //TODO
                            abilityObj.transform.parent = transform;
                            break;
                    }

                    abilityObj.GetComponent<PlayerAbilityController>().Initialize(this);
                }
            }
        }

        private void AttemptReleaseAbility(PlayerAbilityData abilityData)
        {
            if (cooldownTimer == -1)
            {
                cooldownTimer = 0;
            }
        }
    }
}