using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Sprites;

namespace Artemis.Example.Rituals
{
    [System.Serializable]
    public struct TierSettings
    {
        public Material material;
        public Vector2Int gridSize;
        public int numberOfRooms;

        [Space]
        public TileBase floorTile;
    }

    [System.Serializable]
    public struct CellData
    {
        public bool occupied;
        public int neighbors;
        public byte blocked;
        public int distanceFromStart;

        public void Init()
        {
            occupied = false;
            neighbors = 0;
            blocked = 0;
            distanceFromStart = int.MaxValue;
        }

        public void Init(Vector2Int size, int index)
        {
            Init();

            //Check for blocking
            if (index % size.x == 0) //Right
            {
                blocked |= 1 << 0;
            }
            if (index % size.x == size.x - 1) //Left
            {
                blocked |= 1 << 1;
            }
            if (index < size.x) //Down
            {
                blocked |= 1 << 2;
            }
            if (index > size.x * (size.y - 1)) //Up
            {
                blocked |= 1 << 3;
            }
        }

        public void UpdateAsNeighbor(ref CellData neighbor)
        {
            neighbor.neighbors += 1;

            if (neighbor.distanceFromStart > distanceFromStart)
            {
                neighbor.distanceFromStart = distanceFromStart + 1;
            }
            else if(neighbor.distanceFromStart < distanceFromStart)
            {
                distanceFromStart = neighbor.distanceFromStart + 1;
            }
        }
    }

    [System.Serializable]
    public struct GridData
    {
        public CellData[] grid;
        public Vector2Int offset;
        public Vector2Int size;
        public Vector2Int endRoom;
        public List<int> rooms;

        public void Init(Vector2Int _size)
        {
            size = _size;
            grid = new CellData[size.x * size.y]; //x goes by +- 1, y goes by +- x's length
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i].Init(size, i);
            }
            offset = Vector2Int.zero;
            endRoom = Vector2Int.zero;
            rooms = new List<int>();
        }

        public void DetermineOffset(GridData priorGrid)
        {
            offset = (priorGrid.endRoom + priorGrid.offset) - size/2; //The end room of the prior grid is the center of your grid
        }

        public void PreventOverlap(GridData otherGrid, bool isPriorGrid)
        {
            int endRoomIndex = otherGrid.PositionToIndex(otherGrid.endRoom);
            Vector2Int[] traversalNodes = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
            int tempNeighborPersonalIndex;
            foreach (int roomIndex in otherGrid.rooms)
            {
                if(!isPriorGrid || roomIndex != endRoomIndex)
                {
                    for (int i = 0; i < traversalNodes.Length; i++)
                    {
                        tempNeighborPersonalIndex = PositionToIndex(otherGrid.offset - offset + otherGrid.IndexToPosition(roomIndex) + traversalNodes[i]);
                        if (tempNeighborPersonalIndex > -1)
                        {
                            grid[tempNeighborPersonalIndex].occupied = true;
                        }
                    }
                }
            }
        }

        public int PositionToIndex(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x >= size.x || position.y >= size.y)
            {
                return -1;
            }
            else
            {
                return position.y * size.x + position.x;
            }
        }

        public Vector2Int IndexToPosition(int index)
        {
            if(index < 0 || index >= grid.Length)
            {
                return -Vector2Int.one;
            }
            else
            {
                return new Vector2Int(index % size.x, index / size.x);
            }
        }
    }

    [System.Serializable]
    public struct RoomData
    {
        public int tier;
        public int doors;
        public Vector2Int coords;
        public bool visted;

        public void Init(int _tier, Vector2Int _coords)
        {
            tier = _tier;
            coords = _coords;
            doors = 0;
            visted = false;
        }

        public void MarkDoor(int index)
        {
            doors |= 1 << index;
        }
    }

    public class DungeonGenerator : MonoBehaviour
    {
        //TODO: make ComparableIntArray into its own script over in Dev
        [System.Serializable]
        private struct ComparableIntArray : System.IComparable
        {
            [SerializeField]
            private int[] mArray;

            public ComparableIntArray(int[] array)
            {
                mArray = array;
            }

            public ComparableIntArray(Vector2Int array)
            {
                mArray = new int[2] { array.x, array.y };
            }

            private int CompareToSame(ComparableIntArray obj)
            {
                if (mArray.Length.CompareTo(obj.mArray.Length) != 0)
                {
                    return mArray.Length.CompareTo(obj.mArray.Length);
                }

                for (int i = 0; i < mArray.Length; i++)
                {
                    if (mArray[i].CompareTo(obj.mArray[i]) != 0)
                    {
                        return mArray[i].CompareTo(obj.mArray[i]);
                    }
                }

                return 0;
            }

            public int CompareTo(object obj)
            {
                return CompareToSame((ComparableIntArray)obj);
            }
        }

        [SerializeField]
        GameObject cube;

        [SerializeField]
        TierSettings[] tiers;

        [SerializeField]
        GridData[] tierGrids;

        [SerializeField]
        SortedStrictDictionary<ComparableIntArray,RoomData> fullDungeonRooms;

        [SerializeField]
        Tilemap tilemap;

        int attempts;

        void OnEnable()
        {
            GenerateDungeon();
            GenerateRooms();
        }

        void OnDisable()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        void GenerateDungeon()
        {
            bool success = false;
            while (!success)
            {
                success = true;
                attempts = 0;
                tierGrids = new GridData[tiers.Length];
                for (int i = 0; i < tiers.Length; i++)
                {
                    GenerateTier(i);
                    if(tierGrids[i].rooms.Count < tiers[i].numberOfRooms / 2)
                    {
                        success = false;
                    }
                }
            }
        }

        void GenerateTier(int tierIndex)
        {
            Vector2Int gridSize = tiers[tierIndex].gridSize;
            tierGrids[tierIndex] = new GridData();
            tierGrids[tierIndex].Init(gridSize);

            List<int> endRooms = new List<int>();
            Queue<int> queue = new Queue<int>();

            int[] traversalNodes = new int[] { -1, 1, -gridSize.x, gridSize.x };

            int startingIndex;
            GameObject tempSpawnedCube;
            if (tierIndex == 0)
            {
                startingIndex = gridSize.x / 2;
            }
            else
            {
                startingIndex = tierGrids[tierIndex].PositionToIndex(gridSize / 2);
                tierGrids[tierIndex].DetermineOffset(tierGrids[tierIndex - 1]);
                for (int i = tierIndex - 1; i >= 0; i--)
                {
                    tierGrids[tierIndex].PreventOverlap(tierGrids[i], i == tierIndex - 1);
                }
            }

            tempSpawnedCube = Instantiate(cube, new Vector2(startingIndex % gridSize.x, startingIndex / gridSize.x) + tierGrids[tierIndex].offset, Quaternion.identity, transform);
            tempSpawnedCube.GetComponent<MeshRenderer>().material = tiers[tierIndex].material;

            for (int i = 0; i < traversalNodes.Length; i++)
            {
                if ((tierGrids[tierIndex].grid[startingIndex].blocked & (1 << i)) == 0)
                {
                    tierGrids[tierIndex].grid[startingIndex + traversalNodes[i]].neighbors += 1;
                }
            }
            tierGrids[tierIndex].grid[startingIndex].occupied = true;
            tierGrids[tierIndex].grid[startingIndex].distanceFromStart = 0;
            queue.Enqueue(startingIndex);
            tierGrids[tierIndex].rooms.Add(startingIndex);

            int tempDequeuedIndex;
            int tempNeighborIndex;
            while (queue.Count > 0 && tierGrids[tierIndex].rooms.Count < tiers[tierIndex].numberOfRooms + 1)
            {
                tempDequeuedIndex = queue.Dequeue();
                for (int i = 0; i < traversalNodes.Length; i++)
                {
                    if ((tierGrids[tierIndex].grid[tempDequeuedIndex].blocked & (1 << i)) == 0) //Blocked off?
                    {
                        tempNeighborIndex = tempDequeuedIndex + traversalNodes[i];
                        if (tempNeighborIndex > -1 && tempNeighborIndex < tierGrids[tierIndex].grid.Length && //Neighbor index is in a valid range
                            !tierGrids[tierIndex].grid[tempNeighborIndex].occupied && //Not already a room there
                            tierGrids[tierIndex].grid[tempNeighborIndex].neighbors <= 1 && //Only the one neighbor (or less)
                            tierGrids[tierIndex].rooms.Count < tiers[tierIndex].numberOfRooms && //Still need more rooms
                            Random.value < .5f) //50% of spawning
                        {
                            //Spawn New Room!
                            tempSpawnedCube = Instantiate(cube, new Vector2(tempNeighborIndex % gridSize.x, tempNeighborIndex / gridSize.x) + tierGrids[tierIndex].offset, Quaternion.identity, transform);
                            tempSpawnedCube.GetComponent<MeshRenderer>().material = tiers[tierIndex].material;
                            for (int j = 0; j < traversalNodes.Length; j++)
                            {
                                if ((tierGrids[tierIndex].grid[tempNeighborIndex].blocked & (1 << j)) == 0 && tempNeighborIndex + traversalNodes[j] > -1 && tempNeighborIndex + traversalNodes[j] < tierGrids[tierIndex].grid.Length)
                                {
                                    tierGrids[tierIndex].grid[tempNeighborIndex].UpdateAsNeighbor(ref tierGrids[tierIndex].grid[tempNeighborIndex + traversalNodes[j]]);
                                    if (tierGrids[tierIndex].grid[tempNeighborIndex + traversalNodes[j]].occupied && tierGrids[tierIndex].grid[tempNeighborIndex + traversalNodes[j]].neighbors > 1)
                                    {
                                        endRooms.Remove(tempNeighborIndex + traversalNodes[j]);
                                    }
                                }
                            }
                            tierGrids[tierIndex].grid[tempNeighborIndex].occupied = true;
                            queue.Enqueue(tempNeighborIndex);
                            endRooms.Add(tempNeighborIndex);
                            tierGrids[tierIndex].rooms.Add(tempNeighborIndex);
                        }
                    }
                }
            }

            //TODO: If it turns out we don't like the paths generated, let's start adding rooms with >=2 neighbors with the intention to connect and cause some loops.

            int endingRoomIndex = tierGrids[tierIndex].rooms[0];
            foreach (int checkEndRoomIndex in endRooms)
            {
                if (tierGrids[tierIndex].grid[endingRoomIndex].distanceFromStart < tierGrids[tierIndex].grid[checkEndRoomIndex].distanceFromStart)
                {
                    endingRoomIndex = checkEndRoomIndex;
                }
            }
            tierGrids[tierIndex].endRoom = tierGrids[tierIndex].IndexToPosition(endingRoomIndex);

            if (tierGrids[tierIndex].rooms.Count < tiers[tierIndex].numberOfRooms && attempts < 1000)
            {
                attempts++;
                for (int i = transform.childCount - 1; i >= transform.childCount - tierGrids[tierIndex].rooms.Count; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                GenerateTier(tierIndex);
            }
            else if (attempts == 1000)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
            else
            {
                attempts = 0;
            }
        }

        void GenerateRooms()
        {
            fullDungeonRooms = new SortedStrictDictionary<ComparableIntArray,RoomData>();
            RoomData roomData;
            Vector2Int coords;
            for(int i = 0; i < tierGrids.Length; i++)
            {
                foreach (int room in tierGrids[i].rooms)
                {
                    coords = tierGrids[i].IndexToPosition(room) + tierGrids[i].offset;
                    roomData = new RoomData();
                    roomData.Init(i, coords);
                    fullDungeonRooms.Add(new ComparableIntArray(coords), roomData);
                }
            }

            tilemap.ClearAllTiles();
            Vector2Int[] traversalNodes = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
            ComparableIntArray comparableIntArray;
            for (int i = 0; i < fullDungeonRooms.Count; i++)
            {
                //Check for Neighbors
                roomData = fullDungeonRooms.GetTupleAtIndex(i).Value;
                coords = roomData.coords;
                for(int j = 0; j < traversalNodes.Length; j++)
                {
                    comparableIntArray = new ComparableIntArray(coords + traversalNodes[j]);
                    if(fullDungeonRooms.HasKey(comparableIntArray))
                    {
                        fullDungeonRooms[fullDungeonRooms.GetTupleAtIndex(i).Key].MarkDoor(j);
                    }
                }

                //Paint Tilemap
                Vector3Int center = new Vector3Int(coords.x * 17, coords.y * 9);

                tilemap.SetTile(center, tiers[roomData.tier].floorTile);
                tilemap.SetTile(center + new Vector3Int(16,8,0), tiers[roomData.tier].floorTile);
                tilemap.BoxFill(center + new Vector3Int(1, 1, 0), tiers[roomData.tier].floorTile, center.x, center.y, center.x + 16, center.y + 8);
            }
                tilemap.RefreshAllTiles();
        }
    }
}   
