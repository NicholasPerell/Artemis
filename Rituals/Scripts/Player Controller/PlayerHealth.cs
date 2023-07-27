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
        public event UnityAction<int> HealthChanged;
        public event UnityAction TookDamage;
        public event UnityAction Died;

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

            health = Mathf.Clamp(health + deltaHealth, 0, maxHealth);
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
    }
}
