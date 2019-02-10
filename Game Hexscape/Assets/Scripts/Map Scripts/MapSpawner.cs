using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MapSpawner : MonoBehaviour
{

    // Giver her a level and she will spawn a map... Its a bit messy at the moment but she will do for now :)


   //public GameObject mapHolder;


    public static MapSpawner instance;

    [SerializeField] public Grid grid;

    // Creates a dictionary for Hex to its position to be sent to the gridfinder
    Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();

    [SerializeField] private GameObject playerKillZonePrefab;

    private GameObject currentMapHolder;
    public GameObject GetCurrentMapHolder() { 
        return currentMapHolder;
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
        grid.cellSize = new Vector3(shortLength, longLength, 0);



        SpawnHexs(EndlessGameplayManager.instance.levelIndex, 0);
    }

    [ContextMenu("Save Level")]

    public void SaveLevel()
    {
        List<MapElement> mapElements = new List<MapElement>();



        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {

            mapElements.Add(new MapElement(hex.typeOfHex, new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y)));
        }

        Level level = EndlessGameplayManager.instance.levels[EndlessGameplayManager.instance.levelIndex];
       level.hexs = mapElements.ToArray();
        EditorUtility.SetDirty(level);


    }


    public void SetGameobjectWidth(GameObject gameobjectInstance)
    {
        float targetColWidth = shortLength;
        float hexWidth = gameobjectInstance.GetComponent<Collider>().bounds.size.x;

        float newHexWidth = gameobjectInstance.transform.localScale.x / hexWidth * targetColWidth;

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
    public void SpawnHexs(Level level, Vector3 playerPos, bool randomRotate = true)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        //Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();
        mapRefrence.Clear();


        // destroy old hexes 
        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {          
            if (hex.gameObject.activeInHierarchy)
            {
              //  hex.gameObject.transform.parent = grid.transform;

               // Debug.Log(hex.gameObject.transform.position);
                hex.DestroyHex();               
                // might need to add a diffrent function for a 'map destroy'               
            }   
        }

        foreach (Transform mapParent in grid.transform)
        {
            if (mapParent.childCount == 0)
            {
                mapParent.DetachChildren();
                Destroy(mapParent.gameObject);
            }
        }


        // makes it so the Y pos is just bellow the player if the level is below 1
        float yPos = playerPos.y - 1f;

        //if ( EndlessGameplayManager.instance.levelIndex > 1)
        //{
            yPos = playerPos.y - distanceBetweenMaps;
        //}


        GameObject holder = new GameObject(level + ": " + level.name);
        holder.transform.SetParent(grid.transform);

        currentMapHolder = holder;


        foreach (MapElement element in level.hexs)
        {
            // Hex hexInstance = Instantiate(element.hexPrefab, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), Quaternion.Euler(-90, 0, 0), holder.transform).GetComponent<Hex>();

            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();

            SetGameobjectWidth(hexInstance.gameObject);
            //   hexInstance.prefab = element.hexPrefab;


            // adds the hex to the dictonary for the grid finder
            mapRefrence.Add(element.gridPos, hexInstance);

        }

        holder.transform.position = new Vector3(playerPos.x, yPos, playerPos.z);

        // random rotation:
       
        if (randomRotate)
        {
            holder.transform.rotation = Quaternion.Euler(0, 30 * Random.Range(0, 12), 0);
        }
        

        Instantiate(playerKillZonePrefab, holder.transform.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, grid.transform);

        // sends the maprefrence to the gridfinder
        GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);
    }
    public void SpawnHexs(int level, float yPos)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        /*Dictionary<Vector2Int, Hex>*/ //mapRefrence = new Dictionary<Vector2Int, Hex>();
        mapRefrence.Clear();

        GameObject holder = new GameObject(level + ": " + EndlessGameplayManager.instance.levels[level].name);
        holder.transform.SetParent(grid.transform);

        foreach (MapElement element in EndlessGameplayManager.instance.levels[level].hexs)
        {
          
          //  HexBank.instance.PullHex(element.GetHex());
            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();


            SetGameobjectWidth(hexInstance.gameObject);

            // adds the hex to the dictonary for the grid finder
            mapRefrence.Add(element.gridPos, hexInstance);
        }

        holder.transform.position = holder.transform.position -= Vector3.up * yPos;

        // sends the maprefrence to the gridfinder
        GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);

    }

    public void SpawnHexs(int level, float yPos, Vector3 playerPos)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        /*Dictionary<Vector2Int, Hex>*/ //mapRefrence = new Dictionary<Vector2Int, Hex>();
        mapRefrence.Clear();

        GameObject holder = new GameObject(level + ": " + EndlessGameplayManager.instance.levels[level].name);
        holder.transform.SetParent(grid.transform);




        foreach (MapElement element in EndlessGameplayManager.instance.levels[level].hexs)
        {
            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();


            SetGameobjectWidth(hexInstance.gameObject);
           // hexInstance.prefab = element.hexPrefab;

            Debug.Log(hexInstance);

            // adds the hex to the dictonary for the grid finder
            mapRefrence.Add(element.gridPos, hexInstance);
        }

        holder.transform.position = new Vector3(playerPos.x, -yPos, playerPos.z);
        //   holder.transform.position = holder.transform.position -= Vector3.up * yPos;

        // sends the maprefrence to the gridfinder
        GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);
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

    public bool SpawnHexAtLocation(Vector2Int hexLoc, HexTypeEnum typeToSpawn, bool replaceExisting)
    {
        bool positionOccupied = mapRefrence.ContainsKey(hexLoc);

        // Position blocked
        if (positionOccupied && !replaceExisting) return false;
        else
        {
            if (positionOccupied) // Remove occupying tile
            {
                mapRefrence[hexLoc].DestroyHex();
                mapRefrence.Remove(hexLoc);
            }

            Hex hexInstance = HexBank.instance.GetDisabledHex(typeToSpawn, grid.CellToWorld(new Vector3Int(hexLoc.x, hexLoc.y, 0)), currentMapHolder.transform).GetComponent<Hex>();

            // Sets the local position of the Hex to match the level holder
            hexInstance.gameObject.transform.localPosition = new Vector3(hexInstance.gameObject.transform.position.x, 0, hexInstance.gameObject.transform.position.z);

            SetGameobjectWidth(hexInstance.gameObject);

            // adds the hex to the dictonary for the grid finder
            mapRefrence.Add(hexLoc, hexInstance);
            return true;
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


