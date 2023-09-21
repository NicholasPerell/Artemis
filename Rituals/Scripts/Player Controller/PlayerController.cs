using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Example.Rituals.Controls;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerController : PossessableMonoBehaviour
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
        PlayerAbilityData[] startingAbilities;
        [SerializeField]//
        List<PlayerAbilityData> playerAbilities;
        public PlayerAbilityData[] StartingAbilities { get { return startingAbilities; } }
        [SerializeField]//
        int abilityIndexPrimary = 0;
        int abilityIndexSecondary => CalcNextIndexOfAbilityWheel(abilityIndexPrimary);
        Dictionary<PlayerAbilityData, PlayerAbilityController> abilitiesHeld;
        [SerializeField]
        PlayerAbilityData[] demonsAbilities;
        [SerializeField]
        [Min(0.1f)]
        float timeBetweenDemonAbilities = 1;

        [SerializeField]
        [Min(1)]
        float scrollSensitivity = 1;
        float scrollTracker;
#if ENABLE_INPUT_SYSTEM
        SorcererInputs inputActions;
        InputsCheckDownUp abilityInputPrimary;
        InputsCheckDownUp abilityInputSecondary;
#else
        InputsCheckDownUp abilityInputPrimary = new InputsCheckDownUp(new InputCheckDownUp(0), new InputCheckDownUp(KeyCode.Space));
        InputsCheckDownUp abilityInputSecondary = new InputsCheckDownUp(new InputCheckDownUp(1), new InputCheckDownUp(KeyCode.LeftShift));
#endif
        float cooldownTimer = 0;

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

        public event UnityAction<PlayerAbilityData[], int> OnChangedAbilityWheel;

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            inputActions = new SorcererInputs();
            inputActions.Dungeon.Enable();
            abilityInputPrimary = new InputsCheckDownUp(new InputCheckDownUp(inputActions.Dungeon.PrimaryAbility));
            abilityInputSecondary = new InputsCheckDownUp(new InputCheckDownUp(inputActions.Dungeon.SecondaryAbility));
#endif

            ResetAbilities();
            abilitiesHeld = new Dictionary<PlayerAbilityData, PlayerAbilityController>();
        }

        private void Start()
        {
            OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ResetAbilities();
            OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ResetAbilities();
            OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
        }

        protected override void OnPossessed(bool isPossessed)
        {
            base.OnPossessed(isPossessed);
            if(isPossessed)
            {
                AttemptReleaseAbility(playerAbilities[abilityIndexPrimary]);
                AttemptReleaseAbility(playerAbilities[abilityIndexSecondary]);
                cooldownTimer = 0;
            }
            else
            {
                abilitiesHeld.Clear();
                cooldownTimer = 0;
            }
        }

        private void Update()
        {
            RunTimers();
            if (IsPossessed)
            {
                CheckDemonAble();
            }
            else
            {
                CheckInputs();
            }
        }

        private void ResetAbilities()
        {
            abilityIndexPrimary = 0;
            playerAbilities = new List<PlayerAbilityData>(startingAbilities);
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
#if ENABLE_INPUT_SYSTEM
            scrollTracker += inputActions.Dungeon.Scroll.ReadValue<Vector2>().y;
#else
            scrollTracker += Input.mouseScrollDelta.y;
#endif
            if(Mathf.Abs(scrollTracker) > scrollSensitivity)
            {
                Debugging.ArtemisDebug.Instance.OpenReportLine("Scroll Tracker Reached");
                Debugging.ArtemisDebug.Instance.Report("scrollTracker: ").ReportLine(scrollTracker);
                Debugging.ArtemisDebug.Instance.Report("scrollSensitivity: ").ReportLine(scrollSensitivity);


                AttemptReleaseAbility(playerAbilities[abilityIndexPrimary]);
                AttemptReleaseAbility(playerAbilities[abilityIndexSecondary]);

                Debugging.ArtemisDebug.Instance.ReportLine("Releasing" + playerAbilities[abilityIndexPrimary].name).ReportLine("Releasing" + playerAbilities[abilityIndexSecondary].name);

                int slotsToMove = Mathf.FloorToInt(Mathf.Abs(scrollTracker) / scrollSensitivity);
                
                Debugging.ArtemisDebug.Instance.Report("slotsToMove: ").ReportLine(slotsToMove);

                if (scrollTracker < 0)
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

                Debugging.ArtemisDebug.Instance.CloseReport();

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

        private void CheckDemonAble()
        {
            if(cooldownTimer == 0)
            {
                abilitiesHeld.Clear();
                UseAbility(demonsAbilities[Random.Range(0, demonsAbilities.Length)]);
                cooldownTimer = timeBetweenDemonAbilities;
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
                            abilityObj.transform.parent = AncientRuinsManager.Dungeon;
                            break;
                    }

                    PlayerAbilityController abilityController = abilityObj.GetComponent<PlayerAbilityController>();
                    abilityController.Initialize(this);
                    abilitiesHeld.Add(abilityData, abilityController);
                }
            }
        }

        private void UseAbility(PlayerAbilityData abilityData)
        {
            GameObject abilityObj = Instantiate(abilityData.Prefab, transform.position, Quaternion.identity);

            switch (abilityData.PrefabParent)
            {
                case PlayerAbilityData.PrefabIntendedParent.PLAYER:
                    abilityObj.transform.parent = transform;
                    break;
                case PlayerAbilityData.PrefabIntendedParent.TILEMAP:
                    abilityObj.transform.parent = AncientRuinsManager.Dungeon;
                    break;
            }

            PlayerAbilityController abilityController = abilityObj.GetComponent<PlayerAbilityController>();
            abilityController.Initialize(this);
            abilitiesHeld.Add(abilityData, abilityController);
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

        public void GainAbility(PlayerAbilityData playerAbility)
        {
            if(!playerAbilities.Contains(playerAbility))
            {
                playerAbilities.Insert(abilityIndexPrimary, playerAbility);
                OnChangedAbilityWheel?.Invoke(playerAbilities.ToArray(), abilityIndexPrimary);
            }
        }
    }
}