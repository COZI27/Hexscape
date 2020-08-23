using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class MapSpawner : MonoBehaviour
{

    public const int MAX_LEVEL_LAYERS = 5;
    public const int GRID_VERTICAL_OFFSET = 5;
    public const int MAX_LEVEL_RADIUS = 16;

    public const int MAP_LAYER_0 = 0;
    public const int MAP_LAYER_DIGIT = 101;
    public const int MAP_LAYER_UI = 111;




    private static MapSpawner instance;
    public static MapSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MapSpawner>();
                if (instance == null) Debug.LogError("No instance of MapSpawner was found.");
            }
            return instance;
        }
    }



    [SerializeField] public GameObject gridObjectToSpawn;

    private int currentLayer = 0;
    private HexagonGrid[] gridLayers = new HexagonGrid[MAX_LEVEL_LAYERS];

    Dictionary<int, Dictionary<Vector2Int, Hex>> mapLayers = new Dictionary<int, Dictionary<Vector2Int, Hex>>()
    {
        { 0, new Dictionary<Vector2Int, Hex>()  },
        { 1, new Dictionary<Vector2Int, Hex>()  },
        { 2, new Dictionary<Vector2Int, Hex>()  },
        { 3, new Dictionary<Vector2Int, Hex>()  },
        { 4, new Dictionary<Vector2Int, Hex>()  }
    };

    private bool[] hiddenMapLayers = new bool[5] {
        false,
        false,
        false,
        false,
        false
        };



    public bool GetLayerIsHidden(int layer)
    {
        return hiddenMapLayers[layer];
    }

    public void SetLayerIsHidden(bool isHidden, int layer)
    {
        hiddenMapLayers[layer] = isHidden;


        foreach (Hex h in mapLayers[layer].Values)
        {
            h.gameObject.SetActive(!isHidden);
        }

    }

    // Creates a dictionary for Hex to its position to be sent to the gridfinder
    //Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();

    //Dictionary<Vector2Int, Hex> mapUILayer = new Dictionary<Vector2Int, Hex>();







    public HexagonGrid GetCurrentGrid()
    {
        return gridLayers[currentLayer];
    }
    
    public HexagonGrid GetGridFromLayer(int layer)
    {
        if (layer >= 0 && layer < MAX_LEVEL_LAYERS)
            return gridLayers[layer];
        else return null;
    }


    public void DisableMapInteraction(bool onlyIfActive = true, int layer = MAP_LAYER_0) // doesn't work
    {
        foreach (var item in mapLayers[layer])
        {
            if (!onlyIfActive || item.Value.isActiveAndEnabled) item.Value.DisableHex();
        }
     //   ColourManager.instance.SetGrayPallet(true);
    }

    public void EnableMapInteraction(bool onlyIfActive = true, int layer = MAP_LAYER_0)  // doesn't work
    {
        foreach (var item in mapLayers[layer])
        {
            if (!onlyIfActive || item.Value.isActiveAndEnabled) item.Value.EnableHex();

        }

        ColourManager.instance.SetGrayPallet(false);
    }



    private GameObject currentMapHolder;
    public GameObject GetCurrentMapHolder()
    {
        return gridLayers[currentLayer].gameObject;

       // return grid.gameObject;
    }

    // public int currentLevel = 0;

    public float shortLength = 1;
    private float longLength;


    public float distanceBetweenMaps = 3;

    //[ContextMenu("Next Level")]
    //public void NextLevel()
    //{
    //    currentLevel++;
    //    SpawnHexs(currentLevel, currentLevel * distanceBetweenMaps);
    //}


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateHexBoards();
        CalculateLongLengthFromShort();
    }

    //[ContextMenu("Load Level")]
    //public void LoadLevel()
    //{
    //    foreach (Hex hex in GetCurrentGrid().GetComponentsInChildren<Hex>())
    //        if (hex != null) Destroy(hex);

    //    CalculateLongLengthFromShort();
    //}

    //[ContextMenu("Save Level")]

    //public void SaveLevel()
    //{
    //    List<MapElement> mapElements = new List<MapElement>();

    //    foreach (Hex hex in GetCurrentGrid().GetComponentsInChildren<Hex>())
    //    {

    //       // mapElements.Add(new MapElement(hex.GetTypeOfHex(), 
    //        //    new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y)));
    //    }

    //}

    private void OnEnable()
    {
        GenerateHexBoards();

    }


    public void SetGameobjectWidth(GameObject gameobjectInstance)
    {
        float targetColWidth = shortLength;
        float hexWidth = gameobjectInstance.GetComponent<Collider>().bounds.size.x;

        if (hexWidth == 0)
            hexWidth = 1.05f;

        float newHexWidth = gameobjectInstance.transform.localScale.x / hexWidth * targetColWidth;

        Vector3 tempScale = gameobjectInstance.transform.localScale;
        tempScale.x = tempScale.z = newHexWidth;

        gameobjectInstance.transform.localScale = tempScale;


    }


    private void GenerateHexBoards()
    {

        List<GameObject> foundHexGrids = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<HexagonGrid>())
            {
                foundHexGrids.Add(child.gameObject);
            }
        }

        for (int i = 0; i < gridLayers.Length; i++)
        {
            GameObject gridObjectToAdd;
            if (foundHexGrids.Count > 0) {
                gridObjectToAdd = foundHexGrids[0];
                foundHexGrids.RemoveAt(0);
            }
            else
            {
                gridObjectToAdd = Instantiate(gridObjectToSpawn);
            }

            
            if (gridObjectToAdd != null)
            {
                gridObjectToAdd.transform.parent = this.transform;

                Vector3 spawnPos = this.transform.position - new Vector3(0, GRID_VERTICAL_OFFSET * i, 0);
                gridObjectToAdd.transform.localPosition = spawnPos;
                gridLayers[i] = gridObjectToAdd.GetComponent<HexagonGrid>();
            }

        }

        if (foundHexGrids.Count > 0)
        {
            foreach (GameObject g in foundHexGrids)
            {
#if UNITY_EDITOR
                DestroyImmediate(g);
#endif

                if (g != null ) Destroy(g);
            }
        }


    }


    public void ClearCurrentGrid()
    {
        mapLayers[currentLayer].Clear();

        foreach (Hex hex in GetCurrentGrid().GetComponentsInChildren<Hex>())
        {
            if (hex.gameObject.activeInHierarchy)
            {
                hex.transform.parent = this.transform;
                hex.DigHex(false);
            }
        }



    }

    public void ClearAllGrids()
    {
        for (int i = 0; i < mapLayers.Count; i++)
        {
            mapLayers[i].Clear();
        }

        for (int i = 0; i < gridLayers.Length; i++)
        {
            foreach (Hex hex in gridLayers[i].GetComponentsInChildren<Hex>())
            {
                if (hex.gameObject.activeInHierarchy)
                {

#if UNITY_EDITOR
                    hex.gameObject.SetActive(false);
                    HexBank.Instance.AddDisabledHex(hex.gameObject);
#endif
                    hex.transform.parent = this.transform;
                    hex.DigHex(false);
                }
            }
        }

        //GetCurrentGrid().transform.rotation = Quaternion.Euler(Vector3.zero);
        //GetCurrentGrid().transform.position = Vector3.zero;
    }

    //public void ClearMapGrid()
    //{
    //    mapLayers[MAP_LAYER_0].Clear();


    //    foreach (Hex hex in GetCurrentGrid().GetComponentsInChildren<Hex>())
    //    {
    //        if (hex.gameObject.activeInHierarchy)
    //        {
    //            hex.transform.parent = this.transform;
    //            hex.DigHex(false);
    //        }
    //    }

    //    GetCurrentGrid().transform.rotation = Quaternion.Euler(Vector3.zero);
    //    GetCurrentGrid().transform.position = Vector3.zero;

    //}



    public void PositionMapGrid(Vector3 playerPos, bool randomRotate = true)
    {
        float yRot = randomRotate ? 30 * Random.Range(0, 12) : 0; // set rotation if random rotate is on

        PositionMapGrid(playerPos, playerPos.y - distanceBetweenMaps, yRot);
    } // overide that auto gets ypos
    public void PositionMapGrid(Vector3 playerPos, float yPos, int yRotState)
    {
        GetCurrentGrid().transform.position = new Vector3(playerPos.x, yPos, playerPos.z);
        GetCurrentGrid().transform.rotation = Quaternion.Euler(0, 30 * yRotState, 0);
    } // Int to float overload
    public void PositionMapGrid(Vector3 playerPos, float yPos, float yRot)
    {
        GetCurrentGrid().transform.position = new Vector3(playerPos.x, yPos, playerPos.z);
        GetCurrentGrid().transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    public void RemoveHexAtPoint(Vector2Int gridPos)
    {
        if (mapLayers[MAP_LAYER_0].ContainsKey(gridPos)) // need to replace / update to use correct map layer
        {
            Hex oldHex = mapLayers[MAP_LAYER_0][gridPos];
            if (oldHex != null) oldHex.FinishDestroy();
            mapLayers[MAP_LAYER_0].Remove(gridPos);

        }
    }

    public Hex SpawnAHex(MapElement hexInfo)
    {
        return SpawnAHex(hexInfo, currentLayer);
    }
        
    public Hex SpawnAHex(MapElement hexInfo, int layer)
    {

        GameObject foundHexObj = HexBank.Instance.GetDisabledHex(GetCurrentGrid().CellToWorld(new Vector2Int(hexInfo.gridPos.x, hexInfo.gridPos.y)).Value, GetCurrentGrid().transform);
        Hex hexInstance = null;
        if (foundHexObj != null)
        {
            hexInstance = foundHexObj.GetComponent<Hex>();
        }
        else return null;

        currentLayer = layer;



        Vector3 localPos = ((Vector3)GetCurrentGrid().CellToWorld(new Vector2Int(hexInfo.gridPos.x, hexInfo.gridPos.y)));
        hexInstance.transform.localPosition = localPos;
        hexInstance.transform.rotation = GetCurrentGrid().transform.rotation;

        HexMatComponent materialComp = hexInstance.GetComponent<HexMatComponent>();
        if (materialComp != null)
        {
            materialComp.SetAtlas(hexInfo.iconAtlasName);
            materialComp.SetMaterialIcon(hexInfo.iconIndex);
            materialComp.SetEmissionColour(hexInfo.baseColour, hexInfo.iconColour);
            materialComp.SetIconRotation(hexInfo.iconRotation);
        }


        foreach (ElementAttribute item in hexInfo.hexAttributes)
        {
            Debug.Log("AddAttributeToHex for " + item.GetDisplayName());
            if (item != null) item.AddAttributeToHex(hexInstance);
        }

        HexMatComponent hexMatComp = hexInstance.GetComponent<HexMatComponent>();
        if (hexMatComp != null)
        {
            hexMatComp.SetAtlas(hexInfo.iconAtlasName);
            hexMatComp.SetMaterialIcon(hexInfo.iconIndex);
        }

        //SetGameobjectWidth(hexInstance.gameObject);

        if (mapLayers[layer].ContainsKey(hexInfo.gridPos))
        {
            Hex oldHex = mapLayers[layer][hexInfo.gridPos];
            if (oldHex != null) oldHex.FinishDestroy();
            mapLayers[layer][hexInfo.gridPos] = hexInstance;
        }
        else
        {
            mapLayers[layer].Add(hexInfo.gridPos, hexInstance);
        }

        return null;

    }


    public void UpdateMapRefence()
    {
        if (GridFinder.instance != null) GridFinder.instance.SetMap(mapLayers[MAP_LAYER_0], GetCurrentGrid().transform.position, GetCurrentGrid().transform.rotation);
    }

    public void SpawnLevel(Level level, Vector3 playerPos, bool randomRotate = true)
    {
        ClearAllGrids();

        Vector2Int playerStartHexIndex;

        for (int i = 0; i < level.hexs.Length; i ++)
        {
            foreach (MapElement element in level.hexs[i])
                SpawnAHex(element, i);         
        }

        // Offset the map to move the player spawn to the centre
        if (level.playerStartIndex !=  null)
            this.transform.position = new Vector3(this.transform.position.x - level.playerStartIndex.x, this.transform.position.y, this.transform.position.z- level.playerStartIndex.y);   
        

        PositionMapGrid(playerPos, randomRotate);

        UpdateMapRefence();

    }


    public Hex SpawnHexAtLocation(Vector2Int hexLoc, bool replaceExisting, int layer = MAP_LAYER_0)
    {
        bool positionOccupied = mapLayers[layer].ContainsKey(hexLoc);

        // Position blocked
        if (positionOccupied && !replaceExisting)
        {
            Debug.LogWarning("Failed To Spawn Hex at " + hexLoc + ". Position Occupied.");
            return null;
        }
        else
        {
            if (positionOccupied) // Remove occupying tile
            {

                if (mapLayers[layer][hexLoc] != null) mapLayers[layer][hexLoc].DigHex(false);
                mapLayers[layer].Remove(hexLoc);
            }


            GetCurrentGrid().transform.rotation = Quaternion.Euler(Vector3.zero);



            Vector3? gridPos = GetCurrentGrid().CellToWorld(new Vector2Int(hexLoc.x, hexLoc.y)); // COULD BE THIS?
            //gridPos -= grid.transform.position; // THIS FIXED IT

            if (gridPos == null)
            {
                Debug.LogWarning("Failed To Spawn Hex at " + hexLoc + ". Out of Grid Bounds");
                return null;
            }
            else
            {

                //gridPos -= grid.gameObject.transform.position;
                Hex hexInstance = HexBank.Instance.GetDisabledHex(gridPos.Value, GetCurrentGrid().transform).GetComponent<Hex>();

                // Sets the local position of the Hex to match the level holder
                hexInstance.gameObject.transform.localPosition = new Vector3(hexInstance.gameObject.transform.position.x, 0, hexInstance.gameObject.transform.position.z);

                SetGameobjectWidth(hexInstance.gameObject);

                // adds the hex to the dictonary for the grid finder
                mapLayers[layer].Add(hexLoc, hexInstance);
                return hexInstance;
            }
        }
    }


    public int GetMapLayer()
    {
        return currentLayer;
    }

    public bool SetMapLayer(int newLayer)
    {
        if (newLayer >= 0 && newLayer < MAX_LEVEL_LAYERS)
        {
            //TODO: Move grid to layer position
            //      Spawn hexes
            //...

            currentLayer = newLayer;

            for (int i = 0; i < gridLayers.Length; i++)
            {
                if (currentLayer == i)
                    gridLayers[i].SetIsHidden(false);
                else gridLayers[i].SetIsHidden(true);
            }
            return true;
        } 
       else return false;
    
    }




    public void CalculateLongLengthFromShort()
    {
        longLength = (shortLength / Mathf.Sqrt(3)) * 2;
    }

    public void CalculateShortLengthFromLong()
    {
        shortLength = (longLength / 2) * Mathf.Sqrt(3);
    }


}