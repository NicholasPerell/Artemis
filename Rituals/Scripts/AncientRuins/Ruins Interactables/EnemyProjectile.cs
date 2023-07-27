using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField]
        float speed;
        [SerializeField]
        float lifeSpan;

        [SerializeField]
        Rigidbody rb;

        private void Start()
        {
            transform.parent.parent = AncientRuinsManager.Dungeon;
            rb.velocity = -transform.right * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Destroy(transform.parent.gameObject, Time.deltaTime);
            }
            else if (other.tag == "Walling")
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Destroy(transform.parent.gameObject, Time.deltaTime);
            }
            else if (other.tag == "Walling")
            {
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            lifeSpan -= Time.deltaTime;
            if(lifeSpan <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}