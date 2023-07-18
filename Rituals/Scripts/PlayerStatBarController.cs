using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerStatBarController : MonoBehaviour
    {
        [SerializeField]
        PlayerHealth playerHealth;
        [SerializeField]
        Image healthBar;
        float healthMax;
        [Space]
        [SerializeField]
        PlayerMana playerMana;
        [SerializeField]
        Image manaBar;
        float manaMax;

        private void OnEnable()
        {
            healthMax = playerHealth.GetMaxHealth();
            playerHealth.HealthChanged += DisplayHealth;

            manaMax = playerMana.GetMaxMana();
            playerMana.ManaChanged += DisplayMana;
        }

        private void OnDisable()
        {
            playerHealth.HealthChanged -= DisplayHealth;
            playerMana.ManaChanged -= DisplayMana;
        }

        void DisplayHealth(int health)
        {
            healthBar.fillAmount = health / healthMax;
        }

        void DisplayMana(float mana)
        {
            manaBar.fillAmount = mana / manaMax;
        }
    }
}
