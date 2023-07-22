using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class SteerTowardsPlayer : MonoBehaviour
    {
        [SerializeField]
        float force;
        [SerializeField]
        [Min(0.001f)]
        float maxSpeed = 0.1f;

        [SerializeField]
        Rigidbody soulRigidbody;

        protected Transform player;

        protected virtual void Start()
        {
            player = AncientRuinsManager.Player;
        }

        protected virtual void FixedUpdate()
        {
            ApproachPlayer();
        }

        private void ApproachPlayer()
        {
            if (player != null)
            {
                Vector3 diff = player.position - transform.position;
                soulRigidbody.AddForce(diff.normalized * force);
                if (soulRigidbody.velocity.sqrMagnitude >= maxSpeed * maxSpeed)
                {
                    soulRigidbody.velocity = soulRigidbody.velocity.normalized * maxSpeed;
                }
            }
        }
    }
}