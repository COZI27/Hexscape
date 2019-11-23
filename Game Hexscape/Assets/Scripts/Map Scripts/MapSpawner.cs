using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MapSpawner : MonoBehaviour
{

    public const int MAP_LAYER_0 = 0;
    public const int MAP_LAYER_DIGIT = 101;
    public const int MAP_LAYER_UI = 111;

    string pathKillzonePrefab = "Assets/Resources/Prefabs/PlayerKillZone";
    private GameObject playerKillZonePrefab;


    // Giver her a level and she will spawn a map... Its a bit messy at the moment but she will do for now :)
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



    [SerializeField] public HexagonGrid grid;

    // Creates a dictionary for Hex to its position to be sent to the gridfinder
    //Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();

    //Dictionary<Vector2Int, Hex> mapUILayer = new Dictionary<Vector2Int, Hex>();

    Dictionary<int, Dictionary<Vector2Int, Hex>> mapLayers = new Dictionary<int, Dictionary<Vector2Int, Hex>>()
    {
        { MAP_LAYER_0, new Dictionary<Vector2Int, Hex>()  },
        { MAP_LAYER_DIGIT, new Dictionary<Vector2Int, Hex>()  }
    };





    private GameObject currentMapHolder;
    public GameObject GetCurrentMapHolder()
    {
        return grid.gameObject;
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

        CalculateLongLengthFromShort();
    }

    [ContextMenu("Load Level")]
    public void LoadLevel()
    {
        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {
            if (hex != null)
            {
                Destroy(hex);
            }

        }

        CalculateLongLengthFromShort();
        //grid.cellSize = new Vector3(shortLength, longLength, 0);



        //SpawnHexs(EndlessGameplayManager.instance.levelIndex, 0);
    }

    [ContextMenu("Save Level")]

    public void SaveLevel()
    {
        List<MapElement> mapElements = new List<MapElement>();

        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {

            mapElements.Add(new MapElement(hex.typeOfHex, new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y)));
        }

        //Level level = EndlessGameplayManager.instance.levels[EndlessGameplayManager.instance.levelIndex];
        //level.hexs = mapElements.ToArray();
        //EditorUtility.SetDirty(level);


    }


    public void SetGameobjectWidth(GameObject gameobjectInstance)
    {
        float targetColWidth = shortLength;
        float hexWidth = gameobjectInstance.GetComponent<Collider>().bounds.size.x;

        if (hexWidth == 0)
        {
            hexWidth = 1.05f;
        }

        float newHexWidth = gameobjectInstance.transform.localScale.x / hexWidth * targetColWidth;

        
        //Debug.Log(gameobjectInstance.name + ": " + hexWidth);

        Vector3 tempScale = gameobjectInstance.transform.localScale;
        tempScale.x = tempScale.z = newHexWidth;

        gameobjectInstance.transform.localScale = tempScale;


    }

    //public void SpawnHexs(int level)
    //{
    //    float yPos = level * distanceBetweenMaps;

    //    GameObject holder = new GameObject(level + ": " + EndlessGameplayManager.instance.levels[level].name);
    //    holder.transform.SetParent(grid.transform);




    //    foreach (MapElement element in EndlessGameplayManager.instance.levels[level].hexs)
    //    {
    //        Hex hexInstance = Instantiate(element.hexPrefab, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), Quaternion.Euler(-90, 0, 0), holder.transform).GetComponent<Hex>();


    //        SetGameobjectWidth(hexInstance.gameObject);
    //        hexInstance.prefab = element.hexPrefab;


    //    }

    //    holder.transform.position = holder.transform.position -= Vector3.up * yPos;

    //}

        // hexAttribute
        // destroy old hexes 
        //foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        //{          
        //    if (hex.gameObject.activeInHierarchy)
        //    {
              //  hex.gameObject.transform.parent = grid.transform;

    GameObject killZone;

    private void Start()
    {
        playerKillZonePrefab = Resources.Load(pathKillzonePrefab) as GameObject;
        if (playerKillZonePrefab != null)
            killZone = Instantiate(playerKillZonePrefab, grid.transform.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, transform);
    }

    public void ClearMapGrid()
    {
        mapLayers[MAP_LAYER_0].Clear();
       

        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {
            if (hex.gameObject.activeInHierarchy)
            {
                hex.transform.parent = this.transform;
                hex.DigHex();                 
            }
        }

        grid.transform.rotation = Quaternion.Euler(Vector3.zero);
        grid.transform.position = Vector3.zero;

    }

    

    public void PositionMapGrid(Vector3 playerPos, bool randomRotate = true)
    {
        float yRot = randomRotate ? 30 * Random.Range(0, 12) : 0; // set rotation if random rotate is on

        PositionMapGrid(playerPos, playerPos.y - distanceBetweenMaps, yRot);
    } // overide that auto gets ypos
    public void PositionMapGrid(Vector3 playerPos, float yPos, int yRotState)
    {
        grid.transform.position = new Vector3(playerPos.x, yPos, playerPos.z);
        grid.transform.rotation = Quaternion.Euler(0, 30 * yRotState, 0);
    } // Int to float overload
    public void PositionMapGrid(Vector3 playerPos, float yPos, float yRot)
    {
        grid.transform.position = new Vector3(playerPos.x, yPos, playerPos.z);
        grid.transform.rotation = Quaternion.Euler(0,yRot,0);
    }

    public void RemoveHexAtPoint(Vector2Int gridPos)
    {
        if (mapLayers[MAP_LAYER_0].ContainsKey(gridPos)) // need to replace
        {
            Hex oldHex = mapLayers[MAP_LAYER_0][gridPos];
            oldHex.FinishDestroy();
            mapLayers[MAP_LAYER_0].Remove(gridPos);

            if (GameManager.instance != null) GameManager.instance.ReplaceTilePassScores(oldHex.destroyPoints, 0);

        }
    }

    public Hex SpawnAHex(MapElement hexInfo)
    {
        Hex hexInstance = HexBank.Instance.GetDisabledHex(
            hexInfo.GetHex().typeOfHex, 
            grid.CellToWorld(new Vector2Int(hexInfo.gridPos.x, hexInfo.gridPos.y)).Value, grid.transform).GetComponent<Hex>();

        
        Vector3 localPos = ((Vector3) grid.CellToWorld(new Vector2Int(hexInfo.gridPos.x, hexInfo.gridPos.y)));
        hexInstance.transform.localPosition = localPos;

        hexInstance.transform.rotation = grid.transform.rotation;
        if (hexInfo.hexAttribute != null) hexInfo.hexAttribute.AddAttributeToHex(hexInstance);
        SetGameobjectWidth(hexInstance.gameObject);

       if (mapLayers[MAP_LAYER_0].ContainsKey(hexInfo.gridPos) )
        {
            Hex oldHex = mapLayers[MAP_LAYER_0][hexInfo.gridPos];
            oldHex.FinishDestroy();
            mapLayers[MAP_LAYER_0][hexInfo.gridPos] = hexInstance;

            if (GameManager.instance != null) GameManager.instance.ReplaceTilePassScores(oldHex.destroyPoints, hexInstance.destroyPoints);

        }
        else
        {
            mapLayers[MAP_LAYER_0].Add(hexInfo.gridPos, hexInstance);

            if (GameManager.instance != null) GameManager.instance.ReplaceTilePassScores(0, hexInstance.destroyPoints);
        }


        return hexInstance;
    }
    

    public void UpdateMapRefence ()
    {
        
        
        if (GridFinder.instance != null) GridFinder.instance.SetMap(mapLayers[MAP_LAYER_0], grid.transform.position, grid.transform.rotation);
    }

    public void SpawnHexs(Level level, Vector3 playerPos, bool randomRotate = true)
    {

        // ... null ref here...?
        ClearMapGrid();



        foreach (MapElement element in level.hexs)
        {
            SpawnAHex(element);
        }

        PositionMapGrid(playerPos, randomRotate);

        UpdateMapRefence();


        if (killZone != null)
            killZone.transform.position = grid.transform.position - Vector3.up * 2 * distanceBetweenMaps;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveLevel();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
                // temp
            }
            LoadLevel();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
                // temp
            }
        }
    }

    public Hex SpawnHexAtLocation(Vector2Int hexLoc, HexTypeEnum typeToSpawn, bool replaceExisting, int layer = MAP_LAYER_0)
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
                mapLayers[layer][hexLoc].DigHex();
                mapLayers[layer].Remove(hexLoc);
            }

            grid.transform.rotation = Quaternion.Euler(Vector3.zero);

            Vector3? gridPos = grid.CellToWorld(new Vector2Int(hexLoc.x, hexLoc.y));

            if (gridPos == null)
            {
                Debug.LogWarning("Failed To Spawn Hex at " + hexLoc + ". Out of Grid Bounds");
                return null;
            }
            else
            {
                //gridPos -= grid.gameObject.transform.position;
                Hex hexInstance = HexBank.Instance.GetDisabledHex(typeToSpawn, gridPos.Value, grid.transform).GetComponent<Hex>();

                // Sets the local position of the Hex to match the level holder
                hexInstance.gameObject.transform.localPosition = new Vector3(hexInstance.gameObject.transform.position.x, 0, hexInstance.gameObject.transform.position.z);

                SetGameobjectWidth(hexInstance.gameObject);

                // adds the hex to the dictonary for the grid finder
                mapLayers[layer].Add(hexLoc, hexInstance);
                return hexInstance;
            }
        }
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


    //public void SpawnHexs(Level level, Vector3 playerPos, bool randomRotate = true)
    //{
    //    // Creates a dictionary for Hex to its position to be sent to the gridfinder
    //    //Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();
    //    mapRefrence.Clear();

    //    // destroy old hexes 
    //    foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
    //    {
    //        if (hex.gameObject.activeInHierarchy)
    //        {
    //            //  hex.gameObject.transform.parent = grid.transform;

    //            // Debug.Log(hex.gameObject.transform.position);
    //            hex.transform.parent = this.transform;
    //            hex.DestroyHex();
    //            // might need to add a diffrent function for a 'map destroy'               
    //        }
    //    }

    //    foreach (Transform mapParent in grid.transform)
    //    {
    //        if (mapParent.childCount == 0)
    //        {
    //            mapParent.DetachChildren();
    //            Destroy(mapParent.gameObject);
    //        }
    //    }


    //    // makes it so the Y pos is just bellow the player if the level is below 1
    //    float yPos = playerPos.y - 1f;

    //    //if ( EndlessGameplayManager.instance.levelIndex > 1)
    //    //{
    //    yPos = playerPos.y - distanceBetweenMaps;
    //    //}


    //    // GameObject holder = new GameObject(level + ": " + level.levelName);
    //    GameObject holder = grid.gameObject;
    //    holder.transform.rotation = Quaternion.Euler(Vector3.zero); // reset rotation of map
    //    holder.transform.SetParent(grid.transform);


    //    currentMapHolder = holder;


    //    foreach (MapElement element in level.hexs)
    //    {
    //        // Hex hexInstance = Instantiate(element.hexPrefab, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), Quaternion.Euler(-90, 0, 0), holder.transform).GetComponent<Hex>();

    //        Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();
    //        if (element.hexAttribute != null) element.hexAttribute.AddAttributeToHex(hexInstance);
    //        SetGameobjectWidth(hexInstance.gameObject);
    //        //   hexInstance.prefab = element.hexPrefab;


    //        // adds the hex to the dictonary for the grid finder
    //        mapRefrence.Add(element.gridPos, hexInstance);

    //    }

    //    holder.transform.position = new Vector3(playerPos.x, yPos, playerPos.z);

    //    // random rotation:

    //    if (randomRotate)
    //    {
    //        holder.transform.rotation = Quaternion.Euler(0, 30 * Random.Range(0, 12), 0);
    //    }


    //    Instantiate(playerKillZonePrefab, holder.transform.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, grid.transform);

    //    // sends the maprefrence to the gridfinder
    //    GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);
    //}

    //public void SpawnHexs(int level, float yPos)
    //{
    //    Creates a dictionary for Hex to its position to be sent to the gridfinder
    //    /*Dictionary<Vector2Int, Hex>*/ //mapRefrence = new Dictionary<Vector2Int, Hex>();
                                          //   mapRefrence.Clear();

 //    GameObject holder = new GameObject(level + ": " + EndlessGameplayManager.instance.levels[level].levelName);
 //    holder.transform.SetParent(grid.transform);

 //    foreach (MapElement element in EndlessGameplayManager.instance.levels[level].hexs)
 //    {

 //        //  HexBank.instance.PullHex(element.GetHex());
 //        Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();


 //        SetGameobjectWidth(hexInstance.gameObject);

 //        // adds the hex to the dictonary for the grid finder
 //        mapRefrence.Add(element.gridPos, hexInstance);
 //    }

 //    holder.transform.position = holder.transform.position -= Vector3.up * yPos;

 //    sends the maprefrence to the gridfinder
 //    GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);

 //}

 //public void spawnhexs(int level, float ypos, vector3 playerpos)
 //{
 //    creates a dictionary for hex to its position to be sent to the gridfinder
 //    /*dictionary<vector2int, hex>*/ //maprefrence = new dictionary<vector2int, hex>();
 //   maprefrence.clear();

 //    gameobject holder = new gameobject(level + ": " + endlessgameplaymanager.instance.levels[level].levelname);
 //    holder.transform.setparent(grid.transform);




 //    foreach (mapelement element in endlessgameplaymanager.instance.levels[level].hexs)
 //    {
 //        hex hexinstance = hexbank.instance.getdisabledhex(element.gethex().typeofhex, grid.celltoworld(new vector3int(element.gridpos.x, element.gridpos.y, 0)), holder.transform).getcomponent<hex>();


 //        setgameobjectwidth(hexinstance.gameobject);
 //        // hexinstance.prefab = element.hexprefab;

 //        debug.log(hexinstance);

 //        // adds the hex to the dictonary for the grid finder
 //        maprefrence.add(element.gridpos, hexinstance);
 //    }

 //    holder.transform.position = new vector3(playerpos.x, -ypos, playerpos.z);
 //    //   holder.transform.position = holder.transform.position -= vector3.up * ypos;

 //    // sends the maprefrence to the gridfinder
 //    gridfinder.instance.setmap(maprefrence, holder.transform.position, holder.transform.rotation);
 //}