using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MapSpawner : MonoBehaviour
{

    // Giver her a level and she will spawn a map... Its a bit messy at the moment but she will do for now :)



    public static MapSpawner instance;

    [SerializeField] public Grid grid;
    private Transform hexGrid; // parent of the grid

    // Creates a dictionary for Hex to its position to be sent to the gridfinder
    Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();

    [SerializeField] private GameObject playerKillZonePrefab;


<<<<<<< HEAD
=======

    private GameObject killZone;

    public GameObject GetCurrentMapHolder()
    {
        return grid.gameObject;
    }


>>>>>>> NewLevelSystem
    public float shortLength = 1;
    private float longLength;

    public void SetGameobjectWidth(GameObject gameobjectInstance)
    {
        float targetColWidth = shortLength;
        float hexWidth = gameobjectInstance.GetComponent<Collider>().bounds.size.x;

        float newHexWidth = gameobjectInstance.transform.localScale.x / hexWidth * targetColWidth;

        Vector3 tempScale = gameobjectInstance.transform.localScale;
        tempScale.x = tempScale.z = newHexWidth;

        gameobjectInstance.transform.localScale = tempScale;


    }

    public float distanceBetweenMaps = 3;


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

        hexGrid = grid.transform.parent;
    }



    //[ContextMenu("Save Level")]
    //public void SaveLevel()
    //{
    //    List<MapElement> mapElements = new List<MapElement>();

    //    foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
    //    {

    //        mapElements.Add(new MapElement(hex.typeOfHex, new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y)));
    //    }

    //    Level level = GameStateBase.GameSessionData ;
    //    level.hexs = mapElements.ToArray();

    //    LevelGetter.instance.CreateLevel(level, true);


    //} // come back to 



  



    public void SpawnHexs(Level level, Vector3 playerPos, bool randomRotate = true)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        //Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();
        mapRefrence.Clear();



        // destroy old hexes 
        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
<<<<<<< HEAD
        {          
            if (hex.gameObject.activeInHierarchy)
            {
              //  hex.gameObject.transform.parent = grid.transform;

               // Debug.Log(hex.gameObject.transform.position);
                hex.DestroyHex();               
                // might need to add a diffrent function for a 'map destroy'               
            }   
        }
=======
        {


            if (hex.gameObject.activeInHierarchy)
            {
                hex.DestroyHex();
                hex.transform.parent = null;

                // might need to add a diffrent function for a 'map destroy'
            }


>>>>>>> NewLevelSystem

        }

        float yPos = playerPos.y - 1f;
        yPos = playerPos.y - distanceBetweenMaps;

        float rotation = 0;
        if (randomRotate && false)
        {
            rotation = Random.Range(0, 12) * 30;
        }

        List<Hex> hexTiles = new List<Hex>();

        foreach (MapElement element in level.hexs)
        {

<<<<<<< HEAD
            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), holder.transform).GetComponent<Hex>();
=======
            Vector3 position = GridFinder.instance.GridPosToWorld(element.gridPos);
            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), grid.transform).GetComponent<Hex>();

>>>>>>> NewLevelSystem


            SetGameobjectWidth(hexInstance.gameObject);
            hexInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
          //  Debug.Log(hexInstance.transform.rotation.eulerAngles);

            mapRefrence.Add(element.gridPos, hexInstance);

            hexTiles.Add(hexInstance);

        }
<<<<<<< HEAD
        

        Instantiate(playerKillZonePrefab, holder.transform.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, grid.transform);

        // sends the maprefrence to the gridfinder
        GridFinder.instance.SetMap(mapRefrence, holder.transform.position, holder.transform.rotation);
    }

    public void SpawnHexs(int level, float yPos)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        /*Dictionary<Vector2Int, Hex>*/ //mapRefrence = new Dictionary<Vector2Int, Hex>();
        mapRefrence.Clear();
=======
>>>>>>> NewLevelSystem

        hexGrid.position = new Vector3(playerPos.x, yPos, playerPos.z);

        if (killZone == null)
        {
<<<<<<< HEAD
          
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
=======
            killZone = Instantiate(playerKillZonePrefab, hexGrid.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, grid.transform);

        }
        else
>>>>>>> NewLevelSystem
        {
            killZone.transform.position = hexGrid.position - Vector3.up * 2 * distanceBetweenMaps;
        }

<<<<<<< HEAD
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
=======


        GridFinder.instance.SetMap(mapRefrence, hexGrid.position, hexGrid.rotation);


        // just makes sure all hexes have 0 rotation 
        foreach (Hex h in hexTiles)
>>>>>>> NewLevelSystem
        {
            h.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

<<<<<<< HEAD
    public Hex SpawnHexAtLocation(Vector2Int hexLoc, HexTypeEnum typeToSpawn, bool replaceExisting)
    {
        bool positionOccupied = mapRefrence.ContainsKey(hexLoc);

        // Position blocked
        if (positionOccupied && !replaceExisting) return null;
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
=======
        hexGrid.rotation = Quaternion.Euler(0, rotation, 0);
    }
>>>>>>> NewLevelSystem

            // adds the hex to the dictonary for the grid finder
            mapRefrence.Add(hexLoc, hexInstance);
            return hexInstance;
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


