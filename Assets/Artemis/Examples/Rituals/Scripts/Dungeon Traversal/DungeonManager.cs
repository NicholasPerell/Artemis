using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Artemis.Example.Rituals
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField]
        DungeonGenerator dungeonGenerator;
        [SerializeField]
        DoorController[] doors;
        DoorEnterTrigger[] doorEnterTriggers;
        [SerializeField]
        Tilemap tilemap;
        [SerializeField]
        Transform player;

        [Space]
        [SerializeField]
        GameObject spikePrefab;
        [SerializeField]
        GameObject greenPrefab;
        [SerializeField]
        GameObject purplePrefab;
        [SerializeField]
        GameObject redPrefab;

        SortedStrictDictionary<DungeonGenerator.ComparableIntArray, RoomData> rooms;
        int enemiesAlive = 0;
        [SerializeField]
        RoomData currentRoom;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            doorEnterTriggers = new DoorEnterTrigger[doors.Length];
            for (int i = 0; i < doors.Length; i++)
            {
                doorEnterTriggers[i] = doors[i].GetComponentInChildren<DoorEnterTrigger>();
                doorEnterTriggers[i].doorEntered.AddListener(HandleDoorEntering);
            }

            dungeonGenerator.generationComplete.AddListener(HandleGenerationComplete);
            dungeonGenerator.enabled = true;
        }

        private void OnDisable()
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doorEnterTriggers[i].doorEntered.RemoveListener(HandleDoorEntering);
            }

            dungeonGenerator.generationComplete.RemoveListener(HandleGenerationComplete);
            dungeonGenerator.enabled = false;
        }

        private void HandleGenerationComplete(SortedStrictDictionary<DungeonGenerator.ComparableIntArray, RoomData> _rooms)
        {
            rooms = _rooms;

            player.position = Vector3.zero;

            LoadRoom(Vector2Int.zero);
        }

        private void LoadRoom(Vector2Int coords)
        {
            currentRoom = rooms[new DungeonGenerator.ComparableIntArray(coords)];
            tilemap.transform.localPosition = new Vector3((-currentRoom.coords.x - 0.5f) * DungeonGenerator.roomWidth,(-currentRoom.coords.y - 0.5f) * DungeonGenerator.roomHeight);

            Vector3 spawnOffset = new Vector3(-0.5f * DungeonGenerator.roomWidth,0,-0.5f * DungeonGenerator.roomHeight);

            enemiesAlive = 0;
            if (!currentRoom.visited)
            {
                foreach(RoomLayout.SpawnLocation spawns in currentRoom.layout.spawnLocations)
                {
                    switch (spawns.spawnType)
                    {
                        case RoomLayout.SpawnType.TRICLOPS:
                            Instantiate(greenPrefab, spawns.position + spawnOffset, Quaternion.identity).transform.parent = tilemap.transform;
                            //enemiesAlive++;
                            break;
                        case RoomLayout.SpawnType.BLOB:
                            //enemiesAlive++;
                            Instantiate(purplePrefab, spawns.position + spawnOffset, Quaternion.identity).transform.parent = tilemap.transform;
                            break;
                        case RoomLayout.SpawnType.CACODEMON:
                            //enemiesAlive++;
                            Instantiate(redPrefab, spawns.position + spawnOffset, Quaternion.identity).transform.parent = tilemap.transform;
                            break;
                        case RoomLayout.SpawnType.SPIKE:
                            Instantiate(spikePrefab, spawns.position + spawnOffset, Quaternion.identity).transform.parent = tilemap.transform;
                            break;
                        default:
                            break;
                    }
                }
                currentRoom.visited = true;
            }

            InitDoors();

            rooms[new DungeonGenerator.ComparableIntArray(coords)] = currentRoom;
        }

        private void InitDoors()
        {
            bool safe = enemiesAlive == 0;
            bool doorHere;
            for(int i = 0; i < 4; i++)
            {
                doorHere = (currentRoom.doors & (1 << i)) != 0;
                if (!doorHere || !safe)
                {
                    doors[i].Close(doorHere);
                }
                else
                {
                    doors[i].Open(false);
                }
            }
        }
        
        private void OpenDoors()
        {
            bool doorHere;
            for (int i = 0; i < 4; i++)
            {
                doorHere = (currentRoom.doors & (1 << i)) != 0;
                if (doorHere)
                {
                    doors[i].Open(true);
                }
            }
        }

        private void HandleDoorEntering(int direction)
        {
            Vector2Int[] roomTraversalNodes = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
            Vector2Int[] playerTraversalNodes = new Vector2Int[4] { new Vector2Int(6,0), new Vector2Int(-6, 0), new Vector2Int(0,3), new Vector2Int(0,-3) };

            player.position = new Vector3(playerTraversalNodes[direction].x, 0, playerTraversalNodes[direction].y);
            LoadRoom(currentRoom.coords + roomTraversalNodes[direction]);
        }
    }
}
