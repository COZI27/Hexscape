using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSpawDebugger : MonoBehaviour
{
    //HexagonGrid grid;

    //Hex spawnedHex;

    //int timer = 60;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    grid = MapSpawner.Instance.grid;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    if (--timer <= 0)
    //    {
    //        if (this.spawnedHex != null) MapSpawner.Instance.RemoveHexAtPoint(grid.WorldToCell(this.spawnedHex.gameObject.transform.position));

    //        Vector2Int hexCoord = grid.WorldToCell(this.transform.position);
    //        Vector3? worldPos = grid.CellToWorld(hexCoord).Value;


    //        Hex spawnedHex = MapSpawner.Instance.SpawnHexAtLocation(
    //            hexCoord,
                
    //            true
    //            //MapSpawner.MAP_LAYER_DIGIT // CHECK THIS
    //            );

    //        this.spawnedHex = spawnedHex;
    //        this.spawnedHex.GetComponent<MeshRenderer>().enabled = true;
    //        timer = 60;
    //    }
    
    //}
}
