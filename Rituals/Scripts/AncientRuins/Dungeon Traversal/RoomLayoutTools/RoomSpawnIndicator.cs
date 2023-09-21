using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RoomSpawnIndicator : MonoBehaviour
    {
        [System.Serializable]
        private struct SpawnIconPairing
        {
            public RoomLayout.SpawnType type;
            public Sprite sprite;
        }

        public RoomLayout.SpawnType spawnType;

        [SerializeField]
        private SpawnIconPairing[] spawnIconPairings;

        private void OnValidate()
        {
            EnsureVisuals();
        }

        public void EnsureVisuals()
        {
            foreach (SpawnIconPairing pair in spawnIconPairings)
            {
                if (pair.type == spawnType)
                {
                    GetComponent<SpriteRenderer>().sprite = pair.sprite;
                    break;
                }
            }

            Clamp();
        }

        public void Clamp()
        {
            Vector3 corrected = transform.position;
            corrected.x = Mathf.Floor(corrected.x) + .5f;
            corrected.y = 0;
            corrected.z = Mathf.Floor(corrected.z) + .5f;
            transform.position = corrected;
        }
    }
}
