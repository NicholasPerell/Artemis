using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Artemis.Example.Rituals
{
    public class RoomLayout : ScriptableObject
    {
        public enum SpawnType
        {
            TRICLOPS, //Green
            BLOB, //Purple
            CACODEMON, //Red with horns
            SPIKE
        }

        [System.Serializable]
        public struct SpawnLocation
        {
            public Vector3 position;
            public SpawnType spawnType;

            public SpawnLocation(Vector3 _position, SpawnType _spawnType)
            {
                position = _position;
                spawnType = _spawnType;
            }
        }

        [System.Serializable]
        public struct UniqueTile
        {
            public Vector2Int position;
            public TileBase tile;

            public UniqueTile(Vector2Int _position, TileBase _tile)
            {
                position = _position;
                tile = _tile;
            }
        }

        public int tier;
        public List<UniqueTile> uniqueTiles;
        public List<SpawnLocation> spawnLocations;
    }
}
