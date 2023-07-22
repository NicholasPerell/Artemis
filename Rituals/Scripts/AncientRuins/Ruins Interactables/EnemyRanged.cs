using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class EnemyRanged : MonoBehaviour
    {
        [SerializeField]
        [Min(0.01f)]
        float minTimePerLoogie = 1;
        [SerializeField]
        [Min(0.01f)]
        float maxTimePerLoogie = 1;
        [SerializeField]
        [Min(0.01f)]
        float timeBeforeFirstLoogie = 1.5f;
        [SerializeField]
        GameObject loogiePrefab;
        [Space]

        [SerializeField]
        Transform playerTransform;

        [SerializeField]
        Animator anim;

        float timeUntilNextLoogie;

        private void OnEnable()
        {
            timeUntilNextLoogie = timeBeforeFirstLoogie;

            if (playerTransform == null)
            {
                playerTransform = AncientRuinsManager.Player;
            }
        }

        private void Update()
        {
            UpdateFacing();
            CheckForAttacking();
        }

        private void CheckForAttacking()
        {
            if (timeUntilNextLoogie > 0)
            {
                timeUntilNextLoogie -= Time.deltaTime;
                if (timeUntilNextLoogie <= 0)
                {
                    SpawnLoogie();
                }
            }
        }

        private void UpdateFacing()
        {
            Vector3 facing = playerTransform.position - transform.position;

            anim.SetFloat("FacingX", facing.x);
            anim.SetFloat("FacingY", facing.z);
        }

        void SpawnLoogie()
        {
            timeUntilNextLoogie = Random.Range(minTimePerLoogie, maxTimePerLoogie);
            Instantiate(loogiePrefab, transform.position, Quaternion.identity).transform.right = -(playerTransform.position - transform.position);
        }
    }
}