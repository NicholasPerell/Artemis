using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis.Example.Rituals
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
            foreach (SpawnIconPairing pair in spawnIconPairings)
            {
                if (pair.type == spawnType)
                {
                    GetComponent<SpriteRenderer>().sprite = pair.sprite;
                    break;
                }
            }
        }
    }
}
