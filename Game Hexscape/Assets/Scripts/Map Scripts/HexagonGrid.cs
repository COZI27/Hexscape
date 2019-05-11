using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// This class is largely based on the explanations provided by Patel at https://www.redblobgames.com/grids/hexagons/

[ExecuteInEditMode]
public class HexagonGrid : MonoBehaviour
{

    public int gridRadius = 6;


    [SerializeField]
    public float hexSize = 1;
    public int gridWidth = 10;
    public int gridHeight = 10;

    [SerializeField]
    private float hexWidth;
    [SerializeField]
    private float hexHeight;

    [SerializeField]
    public Vector2 pointToFind = new Vector2(0, 0);

    private Vector2 foundHexPoint;

    [SerializeField]
    private Dictionary<Vector3Int, HexCell> hexCells;

    public GameObject testTrackerObj;


    [System.Serializable]
    public class HexCell
    {
        public HexCell(Vector2 worldPosition, Vector3Int cellIndex)
        {
            this.worldPosition = worldPosition;
            this.cellIndex = cellIndex;
        }

        public Vector2 worldPosition; // the world position of the centre of the cell. Should be relative to the grid, and also use the grid's y position
        public Vector3Int cellIndex;


        public bool isSelected = false;
    }


    Vector3 GetHexCornerVertical(Vector2 center, float size, int i)
    {
        float angle_deg = 60 * i - 30; //°; // For flat top - don't subjact 30 degrees
        float angle_rad = Mathf.PI / 180 * angle_deg;
        return new Vector3(
                      center.x + size * Mathf.Cos(angle_rad),
                      0,
                      center.y + size * Mathf.Sin(angle_rad)
                      );
    }



    [ExecuteInEditMode]
    void OnEnable()
    {
        CreateGrid();

        SceneView.onSceneGUIDelegate -= GridUpdate;
        SceneView.onSceneGUIDelegate += GridUpdate;

        hexWidth = Mathf.Sqrt(3) * hexSize;
        hexHeight = 2 * hexSize;

        LocatePointToFindTest();
    }

    private void Update()
    {
        if (testTrackerObj != null)
        {
            pointToFind = new Vector2(testTrackerObj.transform.position.x, testTrackerObj.transform.position.z);

            foundHexPoint = PointToHex(pointToFind);

            Debug.Log("Actual ptf =" + pointToFind + "hex found = " + foundHexPoint);
        }
    }



    [ContextMenu("Locate PointToFind")]
    public void LocatePointToFindTest()
    {
        //Debug.Log("PointToFind found in hex " + PointToHex(pointToFind) + "which is located at" + hexCells[PointToHex(pointToFind)].worldPosition + " and has a cell index of " + hexCells[PointToHex(pointToFind)].cellIndex + ".");
    }



    [ExecuteInEditMode]
    void CreateGrid()
    {
        hexCells = new Dictionary<Vector3Int, HexCell>();

        //CreateCell(new Vector3Int(0, 0, 0));
        //for (int r = 0; r > -gridRadius; r--)
        //    for (int q = -r - 1; q > -gridRadius - r; q--)
        //        //makeCell(q, r);
        //        CreateCell(new Vector3Int(q, 0, r));

        //for (int r = 1; r < gridRadius; r++)
        //    for (int q = 0; q > -gridRadius; q--)
        //        CreateCell(new Vector3Int(q, 0, r));

        //for (int q = 1; q < gridRadius; q++)
        //    for (int r = -q; r < gridRadius - q; r++)
        //        CreateCell(new Vector3Int(q, 0, r));

        List<Vector3Int> tileCoordinates = new List<Vector3Int>();
        tileCoordinates.Add(new Vector3Int(0, 0, 0));


        //int columnOffset = 0;
        int rowOffset = -gridRadius;
        int currentRowLength = gridRadius + 1;
        int worldRowOffset = 0;


        for (int c = -gridRadius; c <= gridRadius; c++) // iterate for the total number of rows - works upwards
        {

            worldRowOffset = c <= 0 ? worldRowOffset-- : worldRowOffset++; // TODO ENEABLE ME


            for (int r = rowOffset; r < rowOffset + currentRowLength; r++)
            {
                Vector3Int hexIndex = new Vector3Int(r, 0, c);
                //Vector3Int hexIndex = new Vector3Int(c, 0, r);

                // TODO: reverse z value top t bottom

                Vector2 worldPos = new Vector2();
                worldPos.x = ((hexIndex.x + (-hexIndex.z * +0.5f) + /* hexIndex.z*/ worldRowOffset / 2) * hexWidth); // + hexIndex.z * +0.5f - hexIndex.z / 2) * hexWidth);
                                                                                                                     //worldPos.x = ( (hexIndex.x + (-hexIndex.z * +0.5f) - hexIndex.z / 2 ) * hexWidth ); // + hexIndex.z * +0.5f - hexIndex.z / 2) * hexWidth);

                worldPos.y = hexIndex.z * hexHeight * 3 / 4;

                hexCells[hexIndex] = new HexCell(worldPos, hexIndex);//CreateCell(hexIndex);
            }
            if (c < 0)
            {
                ++currentRowLength;

            }
            if (c >= 0)
            {
                --currentRowLength;
                rowOffset++; // TODO ENEABLE ME

            }



        }



        //for (int y = -gridRadius; y <= gridRadius; y++)
        //{
        //    for (int z = -gridRadius; z <= gridRadius; z++)
        //    {
        //        for (int x = -gridRadius; x <= gridRadius; x++)
        //        {


        //            Vector3Int hexIndex = new Vector3Int(x, y, z);
        //            hexCells[hexIndex] = CreateCell(hexIndex);


        //        }

        //        //hexCells[CoordinatesToIndex(c, r)] = CreateCell(c, r, i++);
        //    }
        //}
    }

    int tempDebugCounter = 0;

    [ExecuteInEditMode]
    private HexCell CreateCell(Vector3Int index)
    {
        Vector2 worldPos;

        worldPos.x = (index.x + index.z * +0.5f - index.z / 2) * hexWidth;
        worldPos.y = index.z * hexHeight * 3 / 4;

        return new HexCell(worldPos, index);
    }


    Vector3[] GetHexCorners(Vector2 centre)
    {
        int cornerIndex = 0;
        Vector3[] hexCorners = new Vector3[]
        {
            GetHexCornerVertical(centre, hexSize, cornerIndex++),
            GetHexCornerVertical(centre, hexSize, cornerIndex++),
            GetHexCornerVertical(centre, hexSize, cornerIndex++),
            GetHexCornerVertical(centre, hexSize, cornerIndex++),
            GetHexCornerVertical(centre, hexSize, cornerIndex++),
            GetHexCornerVertical(centre, hexSize, cornerIndex++)
        };

        return hexCorners;
    }

    Vector2Int PointToHex(Vector2 point)
    {
        float c = ((Mathf.Sqrt(3) / 3 * point.x - 1.0f / 3  * point.y) / hexSize);

        float r = ((2.0f / 3 * point.y) / hexSize) ;


        Vector2 hex  = new Vector2(c, r);

        Debug.Log("Point to hex: " + hex +". point : " + point);

        //return new Vector2Int((int)hex.x, (int)hex.y);




        Vector2 roundedCoords = RoundHexCoords(new Vector2(c + r , r));

        int offset = (int)point.y > 1 ?
            1 * ((int)point.y - 1) :
            1 * ((int)point.y + 1);

        if (r > 1)
        {
            //offset = 
        }
        else if (r < -1)
        {

        }

        offset = (int)r;
        offset = 0;

        return new Vector2Int((int)roundedCoords.x + offset, (int)roundedCoords.y);
        // Vector2 coords = RoundHexCoords(new Vector2(c, r));
        // return CoordinatesToIndex(coords.x, coords.y);
    }


    int debugBuffer = 0;
    int debugBufferMax = 1000;
    bool CanrPintDebug()
    {
        if (debugBuffer >= debugBufferMax)
        {
            debugBuffer = 0;
            return true;
        }
        else
        {
            debugBuffer++;
            return false;
        }
    }
    

    Vector2 RoundHexCoords(Vector2 coord)
    {
        Vector2 returnCoord = ConvertCubeToAxial(RoundCubeCoords(ConvertAxielToCube(coord)));


        //if (CanrPintDebug())
        //Debug.Log("Int coord  =" + coord + ". Return coord = "+ returnCoord);


        //Debug.Log(coord + " rounded = " + returnCoord);

        return new Vector2(returnCoord.x, (int)returnCoord.y);
    }


    Vector3 RoundCubeCoords(Vector3 coord)
    {
        float rx = Mathf.Round(coord.x);
        float ry = Mathf.Round(coord.y);
        float rz = Mathf.Round(coord.z);

        var x_diff = Mathf.Abs(rx - coord.x);
        var y_diff = Mathf.Abs(ry - coord.y);
        var z_diff = Mathf.Abs(rz - coord.z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;


        return new Vector3(rx, ry, rz);
    }


    Vector2 ConvertCubeToAxial(Vector3 coord)
    {
        var q = coord.x;
        var r = coord.z;
        return new Vector2(q, r);
    }

    Vector3 ConvertAxielToCube(Vector2 coord)
    {
        var x = coord.x;
        var z = coord.y;
        var y = -x - z; // NOTE: these may require sqaping y and z
        return new Vector3(x, y, z);
    }

    //int CoordinatesToIndex(float column, float row)
    //{
    //    return (int)(gridWidth * column + row);
    //}






    [ExecuteInEditMode]
    void GridUpdate(SceneView sceneView)
    {
        //Event e = Event.current;

        //Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        //Vector3 mousePos = r.origin;

        //Vector3 aligned = new Vector3(Mathf.Floor(mousePos.x / width) * width + width / 2.0f,
        //                          Mathf.Floor(mousePos.y / height) * height + height / 2.0f, 0.0f);

        ////Vector2Int hoveredHex = GetHoveredHexagon(new Vector2(mousePos.x, mousePos.y));
        //Debug.Log("aligned  = " + aligned);

        //Vector3 mousePosition = Event.current.mousePosition;
        //mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
        //mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
        //mousePosition.y = -mousePosition.y;

        //Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        //Vector3 spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

        //cursorPos = new Vector3(spawnPosition.x, 0, spawnPosition.z);
        //if (cursorObject != null) cursorObject.transform.position = cursorPos;
        //Debug.Log(spawnPosition);
    }







    private void OnDrawGizmos()
    {
#if UNITY_EDITOR


        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(   new Vector3(foundHexPoint.x, 0, foundHexPoint.y), 0.6f);
        if (hexCells.ContainsKey((new Vector3Int((int)foundHexPoint.x, 0, (int)foundHexPoint.y)))) {
            HexCell foundCell = hexCells[(new Vector3Int((int)foundHexPoint.x, 0, (int)foundHexPoint.y))];
            hexCells.ContainsKey((new Vector3Int((int)foundHexPoint.x, 0, (int)foundHexPoint.y)));
            Gizmos.DrawWireSphere(new Vector3(foundCell.worldPosition.x, 0, foundCell.worldPosition.y), 0.6f); //      Gizmos.DrawWireSphere(   new Vector3(foundHexPoint.x, 0, foundHexPoint.y), 0.6f);)].x, 0, foundHexPoint.y), 0.6f);
        }
        Gizmos.color = Color.white;


        if (hexCells.Count > 0)
        {
            foreach (KeyValuePair<Vector3Int, HexCell> cell in hexCells)
            {

                Vector3[] corners = GetHexCorners(cell.Value.worldPosition);



                //if (PointToHex(new Vector2(cell.Value.cellIndex.x, cell.Value.cellIndex.z)) == PointToHex(pointToFind))
                //{
                //    Debug.Log("pointToFind = " + pointToFind + " rounded = " + PointToHex(pointToFind));



                //    Gizmos.color = Color.green;
                //    Gizmos.DrawWireSphere(new Vector3(cell.Value.worldPosition.x, 0, cell.Value.worldPosition.y), 0.5f);

                //    Gizmos.color = Color.white;
                //}


                Handles.Label(new Vector3(cell.Value.worldPosition.x - 0.5f, 0, cell.Value.worldPosition.y), (cell.Value.cellIndex.x   + " | " + cell.Value.cellIndex.z /*+ "\n     " + cell.Value.cellIndex.y*/).ToString());

                for (int i = 0; i < 6; ++i)
                {

                    Gizmos.DrawLine(
                        corners[i],
                        corners[i == 5 ? 0 : i + 1]
                        );
                }
            }
        }



        //foreach (HexCell cell in cells)
        //{
        //    if (cell.isHighlighted)
        //    {
        //        Gizmos.color = Color.green;
        //    }
        //    else Gizmos.color = Color.white;

        //    for (int i = 0; i < 6; ++i)
        //    {
        //        Vector3 center = cell.worldPosition;

        //        Gizmos.DrawLine(
        //            center + HexMetrics.corners[i],
        //            center + HexMetrics.corners[i == 5 ? 0 : i + 1]
        //            );
        //    }
        //}

        //Gizmos.DrawSphere(cursorPos, 1.0f);
#endif
    }

}
