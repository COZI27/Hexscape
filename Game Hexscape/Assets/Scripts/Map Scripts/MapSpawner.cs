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

    [SerializeField] private GameObject playerKillZonePrefab;



    private GameObject killZone;

    public GameObject GetCurrentMapHolder()
    {
        return grid.gameObject;
    }


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

        LevelGetter.instance.CreateLevel(level);


    } // come back to 




    public void SpawnHexs(Level level, Vector3 playerPos, bool randomRotate = true)
    {
        // Creates a dictionary for Hex to its position to be sent to the gridfinder
        Dictionary<Vector2Int, Hex> mapRefrence = new Dictionary<Vector2Int, Hex>();

        
      

        // destroy old hexes 
        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {


            if (hex.gameObject.activeInHierarchy)
            {
                hex.DestroyHex();
                hex.transform.parent = null;

                // might need to add a diffrent function for a 'map destroy'
            }



        }
        
        float yPos = playerPos.y - 1f;
        yPos = playerPos.y - distanceBetweenMaps;

        float rotation = 0;
        if (randomRotate && false)
        {
            rotation = Random.Range(0, 12) * 30;
        }

        foreach (MapElement element in level.hexs)
        {
       
            Vector3 position = GridFinder.instance.GridPosToWorld(element.gridPos);
            Hex hexInstance = HexBank.instance.GetDisabledHex(element.GetHex().typeOfHex, grid.CellToWorld(new Vector3Int(element.gridPos.x, element.gridPos.y, 0)), grid.transform).GetComponent<Hex>();
           


            SetGameobjectWidth(hexInstance.gameObject);
            hexInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log(hexInstance.transform.rotation.eulerAngles);

            mapRefrence.Add(element.gridPos, hexInstance);
           

        }

        hexGrid.position = new Vector3(playerPos.x, yPos, playerPos.z);
       
        if (killZone == null)
        {
            killZone = Instantiate(playerKillZonePrefab, hexGrid.position - Vector3.up * 2 * distanceBetweenMaps, Quaternion.identity, grid.transform);

        } else
        {
            killZone.transform.position = hexGrid.position - Vector3.up * 2 * distanceBetweenMaps;
        }

        // sends the maprefrence to the gridfinder

        GridFinder.instance.SetMap(mapRefrence, hexGrid.position, hexGrid.rotation);

        hexGrid.rotation = Quaternion.Euler(0, rotation, 0);
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


