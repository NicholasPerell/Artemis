using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Perell.Artemis.Example.Rituals
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
        EnemyManager enemyManager;
        [SerializeField]
        GameObject spikePrefab;
        [SerializeField]
        GameObject powerUpPrefab;

        SortedStrictDictionary<DungeonGenerator.ComparableIntArray, RoomData> rooms;
        [Space]
        [SerializeField]
        RoomData currentRoom;

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

            enemyManager.EnemiesClearedOut += RespondToEnemiesClearedOut;
        }

        private void OnDisable()
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doorEnterTriggers[i].doorEntered.RemoveListener(HandleDoorEntering);
            }

            dungeonGenerator.generationComplete.RemoveListener(HandleGenerationComplete);
            dungeonGenerator.enabled = false;

            enemyManager.EnemiesClearedOut -= RespondToEnemiesClearedOut;
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

            if (!currentRoom.visited)
            {
                foreach(RoomLayout.SpawnLocation spawns in currentRoom.layout.spawnLocations)
                {
                    switch (spawns.spawnType)
                    {
                        case RoomLayout.SpawnType.TRICLOPS:
                            enemyManager.SpawnEnemy(spawns.position + spawnOffset, EnemyType.TRICLOPS);
                            break;
                        case RoomLayout.SpawnType.BLOB:
                            enemyManager.SpawnEnemy(spawns.position + spawnOffset, EnemyType.BLOB);
                            break;
                        case RoomLayout.SpawnType.CACODEMON:
                            enemyManager.SpawnEnemy(spawns.position + spawnOffset, EnemyType.CACODEMON);
                            break;
                        case RoomLayout.SpawnType.SPIKE:
                            Instantiate(spikePrefab, spawns.position + spawnOffset, Quaternion.identity).transform.parent = tilemap.transform;
                            break;
                        default:
                            break;
                    }
                }

                if (currentRoom.layout.abilityData != null)
                {
                    GameObject powerUp = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity);
                    powerUp.transform.parent = tilemap.transform;
                    powerUp.GetComponent<PowerUp>().Initialize(currentRoom.layout.abilityData);
                }

                currentRoom.visited = true;
            }

            InitDoors();

            rooms[new DungeonGenerator.ComparableIntArray(coords)] = currentRoom;
        }

        private void InitDoors()
        {
            bool safe = !enemyManager.HasEnemies;
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

        private void RespondToEnemiesClearedOut()
        {
            OpenDoors();
        }

        public Vector2 FindDoorToNextRoom()
        {
            Vector2 result = Vector2.zero;

            Vector2Int[] roomTraversalNodes = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
            Vector2[] playerTraversalNodes = new Vector2[4] { new Vector2(-6.5f, 0), new Vector2(6.5f, 0), new Vector2(0, -3.5f), new Vector2(0, 3.5f) };

            List<RoomData> seen = new List<RoomData>();
            Queue<KeyValuePair<RoomData,int>> toSee = new Queue<KeyValuePair<RoomData, int>>();
            toSee.Enqueue(new KeyValuePair<RoomData, int>(currentRoom,-1));

            KeyValuePair<RoomData, int> currentSearched;
            RoomData currentNeighbor;
            int dir;

            StringBuilder searchStepsDebug = new StringBuilder("FindDoorToNextRoom()\nStarting At: " + currentRoom.coords);

            while (toSee.Count > 0 && result == Vector2.zero)
            {
                currentSearched = toSee.Dequeue();
                seen.Add(currentSearched.Key);
                searchStepsDebug.Append("\nDequeued: " + currentSearched.Key.coords + ", " + currentSearched.Value);

                dir = currentSearched.Value;
                for (int i = 0; i < 4; i++)
                {
                    if(currentSearched.Value == -1)
                    {
                        dir = i;
                    }

                    if (rooms.TryGetValue(new DungeonGenerator.ComparableIntArray(currentSearched.Key.coords + roomTraversalNodes[i]), out currentNeighbor))
                    {
                        if(!currentNeighbor.visited)
                        {
                            result = playerTraversalNodes[dir];
                            searchStepsDebug.Append("\nFOUND: " + currentNeighbor.coords +", "+ dir);
                            break;
                        }
                        else if(!seen.Contains(currentNeighbor))
                        {
                            searchStepsDebug.Append("\nEnqueued: " + currentNeighbor.coords +", "+ dir);

                            toSee.Enqueue(new KeyValuePair<RoomData, int>(currentNeighbor, dir));
                        }
                    }
                }
            }

            Debug.Log(searchStepsDebug.ToString());

            return result;
        }
    }
}
