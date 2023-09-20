using System;
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
        public event UnityAction<int> OnHealthChanged;
        public event UnityAction TookDamage;
        public event UnityAction OnHealthLost;

        [SerializeField]
        Flag lastDamagedBy;

        [SerializeField]
        [Min(1)]
        int maxHealth = 20;
        int health;

        [SerializeField]
        [Min(0)]
        float invulnerabilityTime = 1;
        float damageCoolDown;

        Vector3 blockerDirection;
        float blockerAngle;

        private void OnEnable()
        {
            RefreshHealth();
            RemoverBlocker();
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

        public void ChangeHealth(int deltaHealth, HealthEffectSource healthEffectSource, Vector3 attackerPosition)
        {
            //Invincibility Frames or Blocking Damage
            if (deltaHealth < 0 && (damageCoolDown > 0 || CheckIfBlocked(attackerPosition)))
            {
                return;
            }

            if (deltaHealth < 0)
            {
                lastDamagedBy.SetValue((int)Enum.Parse<Generated.Cause_of_deathID>(healthEffectSource.ToString(),true));
            }

            health = Mathf.Clamp(health + deltaHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(health);
            if (health <= 0)
            {
                OnHealthLost?.Invoke();
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

        private bool CheckIfBlocked(Vector3 attackerPosition)
        {
            bool isBlocked = false;

            if(blockerDirection != Vector3.zero)
            {
                Vector3 diff = attackerPosition - transform.position;
                diff.Normalize();
                isBlocked = Vector3.Dot(diff, blockerDirection) > blockerAngle;
            }

            return isBlocked;
        }

        public void SetBlocker(Vector3 direction, float angle)
        {
            blockerDirection = direction;
            blockerAngle = Mathf.Cos(angle * 0.5f);
        }

        public void RemoverBlocker()
        {
            blockerDirection = Vector3.zero;
            blockerAngle = 0;
        }

        public void GiveTemporaryInvulnerability(float timeInvulnerable)
        {
            damageCoolDown = timeInvulnerable;
        }

        public void Die()
        {
            health = 0;
            OnHealthChanged?.Invoke(health);
            OnHealthLost?.Invoke();
        }
    }
}
