using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Example.Rituals.Controls;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        PlayerHealth health;
        public PlayerHealth Health { get { return health; } }
        [SerializeField]
        PlayerMana mana;
        public PlayerMana Mana { get { return mana; } }
        [SerializeField]
        PlayerCorruption corruption;
        public PlayerCorruption Corruption { get { return corruption; } }
        [SerializeField]
        PlayerMovement movement;
        public PlayerMovement Movement { get { return movement; } }

        [Space]
        [Header("Abilities")]
        [SerializeField]
        List<PlayerAbilityData> playerAbilities;
        [SerializeField]
        PlayerAbilityData[] demonsAbilities;
        int abilityIndexPrimary = 0;
        int abilityIndexSecondary => CalcNextIndexOfAbilityWheel(abilityIndexPrimary);
        Dictionary<PlayerAbilityData, PlayerAbilityController> abilitiesHeld;

        [SerializeField]
        [Min(1)]
        float scrollSensitivity = 1;
        float scrollTracker;
        InputsCheckDownUp abilityInputPrimary = new InputsCheckDownUp(new InputCheckDownUp(0));
        InputsCheckDownUp abilityInputSecondary = new InputsCheckDownUp(new InputCheckDownUp(1), new InputCheckDownUp(KeyCode.LeftShift));

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

        public UnityAction<PlayerAbilityData[], int> OnChangedAbilityWheel;

        private void Awake()
        {
            abilitiesHeld = new Dictionary<PlayerAbilityData, PlayerAbilityController>();
        }

        private void OnEnable()
        {
            OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
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
            CheckAbilityScroll();
            CheckAbilityInput(abilityInputPrimary, abilityIndexPrimary);
            CheckAbilityInput(abilityInputSecondary, abilityIndexSecondary);
        }

        private void CheckAbilityScroll()
        {
            scrollTracker += Input.mouseScrollDelta.y;
            if(Mathf.Abs(scrollTracker) > scrollSensitivity)
            {
                int slotsToMove = Mathf.FloorToInt(Mathf.Abs(scrollTracker) / scrollSensitivity);
                if(scrollTracker < 0)
                {
                    for(int i = 0; i < slotsToMove; i++)
                    {
                        abilityIndexPrimary = CalcNextIndexOfAbilityWheel(abilityIndexPrimary);
                    }
                }
                else
                {
                    for (int i = 0; i < slotsToMove; i++)
                    {
                        abilityIndexPrimary = CalcPreviousIndexOfAbilityWheel(abilityIndexPrimary);
                    }
                }
                scrollTracker = 0;

                OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
            }
        }

        private void CheckAbilityInput(InputsCheckDownUp controls, int index)
        {
            if (controls.InputUp)
            {
                AttemptReleaseAbility(playerAbilities[index]);
            }
            if (controls.InputDown)
            {
                AttemptUseAbility(playerAbilities[index]);
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
                            abilityObj.transform.parent = AncientRuinsManager.Dungeon;
                            break;
                    }

                    PlayerAbilityController abilityController = abilityObj.GetComponent<PlayerAbilityController>();
                    abilityController.Initialize(this);
                    abilitiesHeld.Add(abilityData, abilityController);
                }
            }
        }

        private void AttemptReleaseAbility(PlayerAbilityData abilityData)
        {
            if (cooldownTimer == -1)
            {
                cooldownTimer = 0;
            }

            PlayerAbilityController abilityController;
            if(abilitiesHeld.TryGetValue(abilityData, out abilityController))
            {
                abilityController?.Release();
                abilitiesHeld.Remove(abilityData);
            }
        }

        public int CalcNextIndexOfAbilityWheel(int index)
        {
            return (index + 1) % playerAbilities.Count;
        }

        public int CalcPreviousIndexOfAbilityWheel(int index)
        {
            return index == 0 ? playerAbilities.Count - 1 : index - 1;
        }
    }
}