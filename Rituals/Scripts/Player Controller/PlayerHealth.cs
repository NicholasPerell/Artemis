using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public enum HealthEffectSource
    {
        SPIKE,
        BOSS,
        TRICLOPS,
        BLOB,
        CACODEMON,
        SOUL
    }

    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        int maxHealth = 20;
        int health;

        [SerializeField]
        [Min(0)]
        float invulnerabilityTime = 1;
        float damageCoolDown;

        public event UnityAction<int> HealthChanged;
        public event UnityAction TookDamage;
        public event UnityAction Died;

        private void OnEnable()
        {
            RefreshHealth();

            DamagingBox.DamageDelt += ChangeHealth;
        }

        private void OnDisable()
        {
            DamagingBox.DamageDelt -= ChangeHealth;
        }

        private void RefreshHealth()
        {
            health = maxHealth;
            damageCoolDown = 0;
        }

        private void Update()
        {
            if (damageCoolDown > 0)
            {
                damageCoolDown -= Time.deltaTime;
            }
        }

        private void ChangeHealth(int deltaHealth, HealthEffectSource healthEffectSource)
        {
            if (deltaHealth < 0 && damageCoolDown > 0)
            {
                return;
            }

            health += deltaHealth;
            HealthChanged.Invoke(health);
            if (health <= 0)
            {
                Died?.Invoke();
            }
            else if (deltaHealth < 0)
            {
                damageCoolDown = invulnerabilityTime;
                TookDamage?.Invoke();
            }
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
