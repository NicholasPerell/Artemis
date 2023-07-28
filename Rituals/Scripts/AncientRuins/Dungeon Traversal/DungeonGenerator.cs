using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Sprites;
using UnityEngine.Events;
using System;

namespace Perell.Artemis.Example.Rituals
{

    [System.Serializable]
    public struct TierSettings
    {
        public Vector2Int gridSize;
        public int numberOfRooms;
        [Space]
        public RoomLayout[] layouts;
        public PlayerAbilityData[] abilities;
        [Space]
        [Header("Tiles")]
        public TileBase floorNorthWest;
        public TileBase floorNorth;
        public TileBase floorNorthEast;
        public TileBase floorWest;
        public TileBase floorCenter;
        public TileBase floorEast;
        public TileBase floorSouthWest;
        public TileBase floorSouth;
        public TileBase floorSouthEast;
        [Space]
        public TileBase wallNorthWest;
        public TileBase wallNorth;
        public TileBase wallNorthEast;
        public TileBase wallWest;
        public TileBase wallCenter;
        public TileBase wallEast;
        public TileBase wallSouthWest;
        public TileBase wallSouth;
        public TileBase wallSouthEast;
        [Space]
        public TileBase doorSouth;

        private List<PlayerAbilityData> unusedAbilities;
        public PlayerAbilityData GetAbility() => GetRandomFromList<PlayerAbilityData>(ref abilities, ref unusedAbilities);

        private List<RoomLayout> unusedLayouts;
        public RoomLayout GetLayout() => GetRandomFromList<RoomLayout>(ref layouts, ref unusedLayouts);

        private T GetRandomFromList<T>(ref T[] array, ref List<T> list)
        {
            T result = default(T);

            if (array != null && array.Length > 0)
            {
                int arrayIndex = UnityEngine.Random.Range(0, array.Length);
                result = array[arrayIndex];

                if (list == null)
                {
                    list = new List<T>();
                }

                if (list.Count > 0)
                {
                    int listIndex = UnityEngine.Random.Range(0, list.Count);
                    result = list[listIndex];
                    list.RemoveAt(listIndex);
                }
                else
                {
                    list.AddRange(array);
                    list.RemoveAt(arrayIndex);
                }
            }

            return result;
        }
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
        public bool visited;
        public RoomLayout layout;

        public void Init(int _tier, Vector2Int _coords)
        {
            tier = _tier;
            coords = _coords;
            doors = 0;
            visited = false;
        }

        public void MarkDoor(int index)
        {
            doors |= 1 << index;
        }

        public bool isDeadEnd => (doors & (doors - 1)) == 0;
    }

    public class DungeonGenerator : MonoBehaviour
    {
        public const int roomWidth = 17;
        public const int roomHeight = 9;

        //TODO: make ComparableIntArray into its own script over in Dev
        [System.Serializable]
        public struct ComparableIntArray : System.IComparable
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
        TierSettings[] tiers;

        [SerializeField]
        GridData[] tierGrids;

        [SerializeField]
        SortedStrictDictionary<ComparableIntArray,RoomData> fullDungeonRooms;

        [SerializeField]
        Tilemap visualTilemap;

        [SerializeField]
        RoomLayout defaultLayout;

        List<PlayerAbilityData> abilitiesAccountedFor;

        int attempts;

        [HideInInspector]
        public UnityEvent<SortedStrictDictionary<ComparableIntArray, RoomData>> generationComplete;

        void OnEnable()
        {
            visualTilemap.transform.localPosition = new Vector3(-roomWidth / 2.0f, -roomHeight / 2.0f, 0);
            GenerateDungeon();
        }

        void OnDisable()
        {
            visualTilemap.ClearAllTiles();
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
            GenerateRooms();
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
            if (tierIndex == 0 || tierIndex == tiers.Length - 1)
            {
                startingIndex = gridSize.x / 2;
            }
            else
            {
                startingIndex = tierGrids[tierIndex].PositionToIndex(gridSize / 2);

            }

            if (tierIndex != 0)
            { 
                tierGrids[tierIndex].DetermineOffset(tierGrids[tierIndex - 1]);
                for (int i = tierIndex - 1; i >= 0; i--)
                {
                    tierGrids[tierIndex].PreventOverlap(tierGrids[i], i == tierIndex - 1);
                }
            }

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
                            UnityEngine.Random.value < .5f) //50% of spawning
                        {
                            //New Room!
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
                GenerateTier(tierIndex);
            }
            else if (attempts == 1000)
            {
            }
            else
            {
                attempts = 0;
            }
        }

        void GenerateRooms()
        {
            abilitiesAccountedFor = new List<PlayerAbilityData>();
            try
            {
                if (AncientRuinsManager.PlayerController != null)
                {
                    abilitiesAccountedFor.AddRange(AncientRuinsManager.PlayerController.CurrentAbilities);
                }
            }
            catch
            {
                Debug.LogError("Issue finding the AncientRuinsManager.PlayerController or AncientRuinsManager.PlayerController.CurrentAbilities right now");
            }

            fullDungeonRooms = new SortedStrictDictionary<ComparableIntArray, RoomData>();
            RoomData roomData;
            Vector2Int coords;
            for (int i = 0; i < tierGrids.Length; i++)
            {
                foreach (int room in tierGrids[i].rooms)
                {
                    coords = tierGrids[i].IndexToPosition(room) + tierGrids[i].offset;
                    roomData = new RoomData();
                    roomData.Init(i, coords);
                    fullDungeonRooms.Add(new ComparableIntArray(coords), roomData);
                }
            }

            visualTilemap.ClearAllTiles();
            Vector2Int[] traversalNodes = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
            ComparableIntArray comparableIntArray;
            RoomData neighborFound;
            List<int> startRoomIndexes = new List<int>();
            List<int> deadEndIndexes = new List<int>();
            bool isStartRoom;
            for (int i = 0; i < fullDungeonRooms.Count; i++)
            {

                //Check for Neighbors
                isStartRoom = false;
                roomData = fullDungeonRooms.GetTupleAtIndex(i).Value;
                coords = roomData.coords;
                for (int j = 0; j < traversalNodes.Length; j++)
                {
                    comparableIntArray = new ComparableIntArray(coords + traversalNodes[j]);
                    if (fullDungeonRooms.TryGetValue(comparableIntArray, out neighborFound))
                    {
                        roomData.MarkDoor(j);
                        if(neighborFound.tier < roomData.tier)
                        {
                            isStartRoom = true;
                        }
                    }
                }

                if(isStartRoom)
                {
                    startRoomIndexes.Add(i);

                }
                else if(roomData.isDeadEnd)
                {
                    deadEndIndexes.Add(i);
                }
                else
                {
                    roomData.layout = tiers[roomData.tier].GetLayout();
                    DrawRoom(roomData);
                }

                fullDungeonRooms[fullDungeonRooms.GetTupleAtIndex(i).Key] = roomData;

            }
            
            //Starting Rooms
            foreach(int index in startRoomIndexes)
            {
                roomData = fullDungeonRooms.GetTupleAtIndex(index).Value;
                roomData.layout = AttemptGenerateAbilityRoomLayout(roomData.tier);
                DrawRoom(roomData);
                fullDungeonRooms[fullDungeonRooms.GetTupleAtIndex(index).Key] = roomData;
            }

            //DeadEnd Rooms
            int randomIndex, temp;
            for(int i = deadEndIndexes.Count - 1; i > 0; i--) //Shuffle
            {
                randomIndex = UnityEngine.Random.Range(0, i + 1);
                temp = deadEndIndexes[randomIndex];
                deadEndIndexes[randomIndex] = deadEndIndexes[i];
                deadEndIndexes[i] = temp;
            }
            foreach (int index in deadEndIndexes)
            {
                roomData = fullDungeonRooms.GetTupleAtIndex(index).Value;
                roomData.layout = AttemptGenerateAbilityRoomLayout(roomData.tier);
                DrawRoom(roomData);
                fullDungeonRooms[fullDungeonRooms.GetTupleAtIndex(index).Key] = roomData;
            }

            visualTilemap.RefreshAllTiles();
            generationComplete.Invoke(fullDungeonRooms);
        }

        private RoomLayout AttemptGenerateAbilityRoomLayout(int tierIndex)
        {
            ref TierSettings tier = ref tiers[tierIndex];
            RoomLayout layout;
            RoomLayout nonAbilityLayout = tier.GetLayout();

            PlayerAbilityData abilityData = null;
            int getsAttempted = 0;
            while(abilityData == null && getsAttempted < tier.abilities.Length)
            {
                abilityData = tier.GetAbility();
                if(abilitiesAccountedFor.Contains(abilityData))
                {
                    abilityData = null;
                }
                getsAttempted++;
            }

            if(abilityData != null)
            {
                layout = RoomLayout.CreateInstance<RoomLayout>();
                layout.name = $"Tier {tierIndex} {abilityData.name} Ability Room";
                layout.tier = nonAbilityLayout.tier;
                layout.uniqueTiles = new List<RoomLayout.UniqueTile>(nonAbilityLayout.uniqueTiles);
                layout.spawnLocations = new List<RoomLayout.SpawnLocation>();
                layout.abilityData = abilityData;

                abilitiesAccountedFor.Add(abilityData);
            }
            else
            {
                layout = nonAbilityLayout;
            }

            return layout;
        }

        void DrawRoom(RoomData roomData)
        {
            Vector2Int coords = roomData.coords;
            Vector3Int bottomLeftCorner = new Vector3Int(coords.x * roomWidth, coords.y * roomHeight);
            Vector3Int bottomRightCorner = bottomLeftCorner + Vector3Int.right * (roomWidth - 1);
            Vector3Int topLeftCorner = bottomLeftCorner + Vector3Int.up * (roomHeight - 1);
            Vector3Int topRightCorner = bottomLeftCorner + new Vector3Int(roomWidth - 1, roomHeight - 1);

            TierSettings tierTiles = tiers[roomData.tier];

            //Set Tiles at corners (otherwise the drawing may be out of bounds)
            visualTilemap.SetTile(bottomLeftCorner, tierTiles.wallCenter);
            visualTilemap.SetTile(topRightCorner, tierTiles.wallCenter);

            //Wall Center
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(0, 1, 0), tiers[roomData.tier].wallCenter, bottomLeftCorner.x, bottomLeftCorner.y, topLeftCorner.x, topLeftCorner.y);
            visualTilemap.BoxFill(topRightCorner + new Vector3Int(0, -1, 0), tiers[roomData.tier].wallCenter, bottomRightCorner.x, bottomRightCorner.y, topRightCorner.x, topRightCorner.y);

            //Floor Center
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 2, 0), tiers[roomData.tier].floorCenter, bottomLeftCorner.x + 2, bottomLeftCorner.y + 2, topRightCorner.x - 2, topRightCorner.y - 2);

            //Left/West Side
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(1, 1, 0), tiers[roomData.tier].wallWest, bottomLeftCorner.x + 1, bottomLeftCorner.y + 1, topLeftCorner.x + 1, topLeftCorner.y - 1);
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 2, 0), tiers[roomData.tier].floorWest, bottomLeftCorner.x + 2, bottomLeftCorner.y + 2, topLeftCorner.x + 2, topLeftCorner.y - 2);

            //Right/East Side
            visualTilemap.BoxFill(bottomRightCorner + new Vector3Int(-1, 1, 0), tiers[roomData.tier].wallEast, bottomRightCorner.x - 1, bottomRightCorner.y + 1, topRightCorner.x - 1, topRightCorner.y - 1);
            visualTilemap.BoxFill(bottomRightCorner + new Vector3Int(-2, 2, 0), tiers[roomData.tier].floorEast, bottomRightCorner.x - 2, bottomRightCorner.y + 2, topRightCorner.x - 2, topRightCorner.y - 2);

            //Bottom/South Side
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 0, 0), tiers[roomData.tier].wallSouth, bottomLeftCorner.x + 2, bottomLeftCorner.y, bottomRightCorner.x - 2 , bottomRightCorner.y);
            visualTilemap.BoxFill(bottomLeftCorner + new Vector3Int(3, 1, 0), tiers[roomData.tier].floorSouth, bottomLeftCorner.x + 3, bottomLeftCorner.y + 1, bottomRightCorner.x - 3, bottomRightCorner.y + 1);

            //Top/North Side
            visualTilemap.BoxFill(topLeftCorner + new Vector3Int(2, 0, 0), tiers[roomData.tier].wallNorth, topLeftCorner.x + 2, topLeftCorner.y, topRightCorner.x - 2, topRightCorner.y);
            visualTilemap.BoxFill(topLeftCorner + new Vector3Int(3, -1, 0), tiers[roomData.tier].floorNorth, topLeftCorner.x + 3, topLeftCorner.y - 1, topRightCorner.x - 3, topRightCorner.y - 1);

            //Set Corners
            visualTilemap.SetTile(bottomLeftCorner + new Vector3Int(1, 0, 0), tierTiles.wallSouthWest);
            visualTilemap.SetTile(bottomLeftCorner + new Vector3Int(2, 1, 0), tierTiles.floorSouthWest);
            visualTilemap.SetTile(bottomRightCorner + new Vector3Int(-1, 0, 0), tierTiles.wallSouthEast);
            visualTilemap.SetTile(bottomRightCorner + new Vector3Int(-2, 1, 0), tierTiles.floorSouthEast);
            visualTilemap.SetTile(topLeftCorner + new Vector3Int(1, 0, 0), tierTiles.wallNorthWest);
            visualTilemap.SetTile(topLeftCorner + new Vector3Int(2, -1, 0), tierTiles.floorNorthWest);
            visualTilemap.SetTile(topRightCorner + new Vector3Int(-1, 0, 0), tierTiles.wallNorthEast);
            visualTilemap.SetTile(topRightCorner + new Vector3Int(-2, -1, 0), tierTiles.floorNorthEast);

            //Doors
            Vector3Int[] traversalNodes = new Vector3Int[4] { new Vector3Int(1,roomHeight/2), new Vector3Int(roomWidth - 2, roomHeight / 2), new Vector3Int(roomWidth / 2, 0), new Vector3Int(roomWidth / 2, roomHeight - 1) };
            float[] rotationNodes = new float[4] { -90, 90, 0, 180 };
            for (int i = 0; i < traversalNodes.Length; i++)
            {
                if ((roomData.doors & (1 << i)) != 0)
                {
                    visualTilemap.SetTile(new TileChangeData(bottomLeftCorner + traversalNodes[i], tierTiles.doorSouth, Color.white, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotationNodes[i]), Vector3.one)), true);
                }
            }

            //Flourishes
            foreach(RoomLayout.UniqueTile uniqueTile in roomData.layout.uniqueTiles)
            {
                visualTilemap.SetTile(bottomLeftCorner + new Vector3Int(uniqueTile.position.x, uniqueTile.position.y, 0), uniqueTile.tile);
            }
        }
    }
}   
