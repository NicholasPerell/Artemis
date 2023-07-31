using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

namespace Perell.Artemis.Example.Rituals
{
    public class DemonSpiritController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        PixelPerfectCamera pixelPerfectCamera;

        [Header("Internal References")]
        [SerializeField]
        ParticleSystem trailingParticles;
        [SerializeField]
        GameObject enterScreenPrefab;

        [Header("Variables")]
        [SerializeField]
        float spawnOffCameraDistance;
        [SerializeField]
        float distanceToConsiderCaught;

        public event UnityAction OnCaughtPlayer;

        Transform player;
        Vector2 cameraSize;
        bool onCamera = true;

        private void Start()
        {
            player = AncientRuinsManager.Player;
        }

        private void OnEnable()
        {
            CalculateCameraDistanceVariables();
            RepositionOffCamera();
            SetCustomSimulationSpaceAsParent();
            onCamera = true;
        }

        private void Update()
        {
            CheckForEnterCamera();
        }

        private void FixedUpdate()
        {
            CheckForCaughtPlayer();
        }

        private void CalculateCameraDistanceVariables()
        {
            cameraSize.x = 0.5f * pixelPerfectCamera.refResolutionX / pixelPerfectCamera.assetsPPU;
            cameraSize.y = 0.5f * pixelPerfectCamera.refResolutionY / pixelPerfectCamera.assetsPPU;
        }

        private void RepositionOffCamera()
        {
            Vector2 direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    direction.x = cameraSize.x + spawnOffCameraDistance;
                }
                else
                {
                    direction.x = -(cameraSize.x + spawnOffCameraDistance);
                }

                direction.y *= cameraSize.y;
            }
            else
            {
                if (direction.y > 0)
                {
                    direction.y = (cameraSize.y + spawnOffCameraDistance);
                }
                else
                {
                    direction.y = -(cameraSize.y + spawnOffCameraDistance);
                }

                direction.x *= cameraSize.x;
            }

            transform.position = new Vector3(direction.x, 0, direction.y);
        }

        private void SetCustomSimulationSpaceAsParent()
        {
            var trailParticlesMain = trailingParticles.main;
            trailParticlesMain.customSimulationSpace = transform.parent;
        }

        private void CheckForEnterCamera()
        {
            bool previous = onCamera;
            onCamera = transform.position.x == Mathf.Clamp(transform.position.x, -cameraSize.x, cameraSize.x)
                && transform.position.z == Mathf.Clamp(transform.position.z, -cameraSize.y, cameraSize.y);
            if (!previous && onCamera)
            {
                Instantiate(enterScreenPrefab, transform.position, Quaternion.identity).transform.up = -(player.position - transform.position);
            }
        }

        private void CheckForCaughtPlayer()
        {
            Collider[] check = Physics.OverlapSphere(transform.position + Vector3.down * .5f, distanceToConsiderCaught);
            foreach (Collider overlap in check)
            {
                if (overlap.tag == "Player")
                {
                    OnCaughtPlayer?.Invoke();
                    break;
                }
            }
        }
    }
}