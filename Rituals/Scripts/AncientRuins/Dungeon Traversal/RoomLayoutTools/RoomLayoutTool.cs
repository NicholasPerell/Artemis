using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace Perell.Artemis.Example.Rituals
{
    public class RoomLayoutTool : MonoBehaviour
    {
        [SerializeField]
        string roomName;

        [SerializeField]
        TierSettings[] tiers;

        [SerializeField]
        [Range(0,4)]
        int tierPreview;

        [SerializeField]
        Tilemap backdropTilemap;
        [SerializeField]
        Tilemap customizedTilemap;
        [SerializeField]
        GameObject spawnIndicatorPrefab;
        [Space]
        [SerializeField]
        RoomLayout toLoad;

        [ContextMenu("Initialize")]
        private void Init()
        {
            backdropTilemap.ClearAllTiles();
            customizedTilemap.ClearAllTiles();
            DrawRoom();
            RoomSpawnIndicator[] spawners = GameObject.FindObjectsOfType<RoomSpawnIndicator>();
            for (int i = spawners.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(spawners[i].gameObject);
            }
        }

        private void DrawRoom()
        {
            Vector3Int bottomLeftCorner = new Vector3Int(0, 0);
            Vector3Int bottomRightCorner = bottomLeftCorner + Vector3Int.right * (DungeonGenerator.roomWidth - 1);
            Vector3Int topLeftCorner = bottomLeftCorner + Vector3Int.up * (DungeonGenerator.roomHeight - 1);
            Vector3Int topRightCorner = bottomLeftCorner + new Vector3Int(DungeonGenerator.roomWidth - 1, DungeonGenerator.roomHeight - 1);

            TierSettings tierTiles = tiers[tierPreview];

            //Set Tiles at corners (otherwise the drawing may be out of bounds)
            backdropTilemap.SetTile(bottomLeftCorner, tierTiles.wallCenter);
            backdropTilemap.SetTile(topRightCorner, tierTiles.wallCenter);

            //Wall Center
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(0, 1, 0), tiers[tierPreview].wallCenter, bottomLeftCorner.x, bottomLeftCorner.y, topLeftCorner.x, topLeftCorner.y);
            backdropTilemap.BoxFill(topRightCorner + new Vector3Int(0, -1, 0), tiers[tierPreview].wallCenter, bottomRightCorner.x, bottomRightCorner.y, topRightCorner.x, topRightCorner.y);

            //Floor Center
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 2, 0), tiers[tierPreview].floorCenter, bottomLeftCorner.x + 2, bottomLeftCorner.y + 2, topRightCorner.x - 2, topRightCorner.y - 2);

            //Left/West Side
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(1, 1, 0), tiers[tierPreview].wallWest, bottomLeftCorner.x + 1, bottomLeftCorner.y + 1, topLeftCorner.x + 1, topLeftCorner.y - 1);
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 2, 0), tiers[tierPreview].floorWest, bottomLeftCorner.x + 2, bottomLeftCorner.y + 2, topLeftCorner.x + 2, topLeftCorner.y - 2);

            //Right/East Side
            backdropTilemap.BoxFill(bottomRightCorner + new Vector3Int(-1, 1, 0), tiers[tierPreview].wallEast, bottomRightCorner.x - 1, bottomRightCorner.y + 1, topRightCorner.x - 1, topRightCorner.y - 1);
            backdropTilemap.BoxFill(bottomRightCorner + new Vector3Int(-2, 2, 0), tiers[tierPreview].floorEast, bottomRightCorner.x - 2, bottomRightCorner.y + 2, topRightCorner.x - 2, topRightCorner.y - 2);

            //Bottom/South Side
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(2, 0, 0), tiers[tierPreview].wallSouth, bottomLeftCorner.x + 2, bottomLeftCorner.y, bottomRightCorner.x - 2, bottomRightCorner.y);
            backdropTilemap.BoxFill(bottomLeftCorner + new Vector3Int(3, 1, 0), tiers[tierPreview].floorSouth, bottomLeftCorner.x + 3, bottomLeftCorner.y + 1, bottomRightCorner.x - 3, bottomRightCorner.y + 1);

            //Top/North Side
            backdropTilemap.BoxFill(topLeftCorner + new Vector3Int(2, 0, 0), tiers[tierPreview].wallNorth, topLeftCorner.x + 2, topLeftCorner.y, topRightCorner.x - 2, topRightCorner.y);
            backdropTilemap.BoxFill(topLeftCorner + new Vector3Int(3, -1, 0), tiers[tierPreview].floorNorth, topLeftCorner.x + 3, topLeftCorner.y - 1, topRightCorner.x - 3, topRightCorner.y - 1);

            //Set Corners
            backdropTilemap.SetTile(bottomLeftCorner + new Vector3Int(1, 0, 0), tierTiles.wallSouthWest);
            backdropTilemap.SetTile(bottomLeftCorner + new Vector3Int(2, 1, 0), tierTiles.floorSouthWest);
            backdropTilemap.SetTile(bottomRightCorner + new Vector3Int(-1, 0, 0), tierTiles.wallSouthEast);
            backdropTilemap.SetTile(bottomRightCorner + new Vector3Int(-2, 1, 0), tierTiles.floorSouthEast);
            backdropTilemap.SetTile(topLeftCorner + new Vector3Int(1, 0, 0), tierTiles.wallNorthWest);
            backdropTilemap.SetTile(topLeftCorner + new Vector3Int(2, -1, 0), tierTiles.floorNorthWest);
            backdropTilemap.SetTile(topRightCorner + new Vector3Int(-1, 0, 0), tierTiles.wallNorthEast);
            backdropTilemap.SetTile(topRightCorner + new Vector3Int(-2, -1, 0), tierTiles.floorNorthEast);

            //Doors
            Vector3Int[] traversalNodes = new Vector3Int[4] { new Vector3Int(1, DungeonGenerator.roomHeight / 2), new Vector3Int(DungeonGenerator.roomWidth - 2, DungeonGenerator.roomHeight / 2), new Vector3Int(DungeonGenerator.roomWidth / 2, 0), new Vector3Int(DungeonGenerator.roomWidth / 2, DungeonGenerator.roomHeight - 1) };
            float[] rotationNodes = new float[4] { -90, 90, 0, 180 };
            for (int i = 0; i < traversalNodes.Length; i++)
            {
                backdropTilemap.SetTile(new TileChangeData(bottomLeftCorner + traversalNodes[i], tierTiles.doorSouth, Color.white, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotationNodes[i]), Vector3.one)), true);
            }
        }

        [ContextMenu("Save")]
        private void Save()
        {
            if(roomName == null)
            {
                return;
            }

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            string folderPath = scriptPath.Substring(0,scriptPath.LastIndexOf('/')) + "/../../../../ScriptableObjects/RoomLayouts";
            string filePath = folderPath + $"/Tier{tierPreview}_" + roomName.Trim().Replace(' ','_') + ".asset";

            RoomLayout newLayout = RoomLayout.CreateInstance<RoomLayout>();
            newLayout.uniqueTiles = new List<RoomLayout.UniqueTile>();
            newLayout.spawnLocations = new List<RoomLayout.SpawnLocation>();

            newLayout.tier = tierPreview;

            TileBase tempTile;
            for(int x = 0; x < DungeonGenerator.roomWidth; x++)
            {
                for (int y = 0; y < DungeonGenerator.roomHeight; y++)
                {
                    tempTile = customizedTilemap.GetTile(new Vector3Int(x, y));
                    if(tempTile != null)
                    {
                        newLayout.uniqueTiles.Add(new RoomLayout.UniqueTile(new Vector2Int(x, y), tempTile));
                    }
                }
            }

            RoomSpawnIndicator[] spawners = GameObject.FindObjectsOfType<RoomSpawnIndicator>();
            Vector3 temp;
            foreach(RoomSpawnIndicator spawner in spawners)
            {
                spawner.Clamp();
                temp = new Vector3(spawner.transform.position.x, 0, spawner.transform.position.z);
                newLayout.spawnLocations.Add(new RoomLayout.SpawnLocation(spawner.transform.position, spawner.spawnType));
            }
            
            if(!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(scriptPath.Substring(0, scriptPath.LastIndexOf('/')), "RoomLayouts");
            }

            AssetDatabase.CreateAsset(newLayout,filePath);
            AssetDatabase.ImportAsset(filePath);
            
            Init();
        }


        [ContextMenu("Load")]
        private void Load()
        {
            if(toLoad == null)
            {
                return;
            }

            tierPreview = toLoad.tier;

            roomName = toLoad.name.Substring(5+toLoad.tier.ToString().Length);

            Init();

            foreach(RoomLayout.UniqueTile uniqueTile in toLoad.uniqueTiles)
            {
                customizedTilemap.SetTile(new Vector3Int(uniqueTile.position.x,uniqueTile.position.y), uniqueTile.tile);
            }

            foreach(RoomLayout.SpawnLocation spawnLocation in toLoad.spawnLocations)
            {
                RoomSpawnIndicator indicator = Instantiate(spawnIndicatorPrefab, spawnLocation.position, Quaternion.Euler(90,0,0)).GetComponent<RoomSpawnIndicator>();
                indicator.spawnType = spawnLocation.spawnType;
                indicator.EnsureVisuals();
            }
        }
    }
}
