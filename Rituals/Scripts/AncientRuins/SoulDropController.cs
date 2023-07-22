using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class SoulDropController : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer spriteRenderer;

        [Space]
        [Header("Variables")]
        [SerializeField]
        [Min(0.1f)]
        float lifespan = 0.1f;
        [SerializeField]
        int healthIncrease;
        [SerializeField]
        float manaIncrease;
        [SerializeField]
        float corruptionIncrease;

        float timeLeft;

        private void Start()
        {
            timeLeft = lifespan;
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                Color fadedColor = spriteRenderer.color;
                fadedColor.a = timeLeft / lifespan;
                spriteRenderer.color = fadedColor;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                AffectPlayer();
                Destroy(gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                AffectPlayer();
                Destroy(gameObject);
            }
        }

        private void AffectPlayer()
        {
            AncientRuinsManager.PlayerHealth.ChangeHealth(healthIncrease, HealthEffectSource.SOUL);
            AncientRuinsManager.PlayerMana.GainMana(manaIncrease);
            AncientRuinsManager.PlayerCorruption.AddToCorruptionChance(corruptionIncrease);
        }
    }
}