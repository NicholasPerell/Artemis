using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerMana : PossessableMonoBehaviour
    {
        [SerializeField]
        float manaMax;
        [SerializeField]
        float currentMana;
        [SerializeField]
        float manaRegenPerSecond;

        public event UnityAction<float> ManaChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            Refill();
        }

        protected override void OnPossessed(bool isPossessed)
        {
            Refill();
        }

        void Update()
        {
            if (currentMana < manaMax)
            {
                currentMana += manaRegenPerSecond * Time.deltaTime;
                if (currentMana > manaMax)
                {
                    currentMana = manaMax;
                }
                ManaChanged?.Invoke(currentMana);
            }
        }

        void Refill()
        {
            currentMana = manaMax;
            ManaChanged?.Invoke(currentMana);
        }

        public void GainMana(float gained)
        {
            if (currentMana < manaMax)
            {
                currentMana += gained;
                if (currentMana > manaMax)
                {
                    currentMana = manaMax;
                }
                ManaChanged?.Invoke(currentMana);
            }
        }

        public bool SpendMana(float spent)
        {
            if (spent > currentMana)
            {
                return false;
            }

            currentMana -= spent;
            ManaChanged?.Invoke(currentMana);

            return true;
        }

        public float GetMaxMana()
        {
            return manaMax;
        }
    }
}