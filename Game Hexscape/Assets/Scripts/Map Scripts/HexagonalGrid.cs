using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// This class is largely based on the explanations provided by Patel at https://www.redblobgames.com/grids/hexagons/

[ExecuteInEditMode]
public class HexagonalGrid : MonoBehaviour
{

    public static float hexSize = 1;
    public int gridWidth = 10;
    public int gridHeight = 10;

    private float hexWidth = Mathf.Sqrt(3) * hexSize;
    private float hexHeight = 2 * hexSize;

    [SerializeField]
    public Vector2 pointToFind = new Vector2(0, 0);

    [SerializeField]
    private HexCell[] hexCells; // index = y * gridWidth + x

    public GameObject testTrackerObj;


    [System.Serializable]
    public class HexCell
    {
        public HexCell(Vector2 worldPosition, Vector2Int cellIndex)
        {
            this.worldPosition = worldPosition;
            this.cellIndex = cellIndex;
        }

        public Vector2 worldPosition;
        public Vector2Int cellIndex;


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

        LocatePointToFindTest();
    }

    private void Update()
    {
        if (testTrackerObj != null)
        {
            pointToFind = new Vector2(testTrackerObj.transform.position.x, testTrackerObj.transform.position.z);
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
        hexCells = new HexCell[gridWidth * gridHeight];




        for (int r = 0, i = 0; r < gridHeight; r++)
        {
            for (int c = 0; c < gridWidth; c++)
            {
                hexCells[CoordinatesToIndex(c, r)] = CreateCell(c, r, i++);
            }
        }

    }

    int tempDebugCounter = 0;

    [ExecuteInEditMode]
    private HexCell CreateCell(int x, int z, int i)
    {
        Vector2 worldPos;

        worldPos.x = (x + z * +0.5f - z / 2) * hexWidth;
        worldPos.y = z * hexHeight * 3 / 4;

        return new HexCell(worldPos, new Vector2Int(x, z));
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

    int PointToHex(Vector2 point)
    {
        float c = (Mathf.Sqrt(3) / 3 * point.x - 1.0f / 3 * point.y) / hexSize;
        float r = (2.0f / 3 * point.y) / hexSize;

        tempDebugCounter++;
        if (tempDebugCounter > 1000)
        {
            Debug.Log("Point = " + point + ". column = " + c + " + Row = " + r);
            tempDebugCounter = 0;
        }
        Vector2 coords = RoundHexCoords(new Vector2(c, r));
        return CoordinatesToIndex(coords.x, coords.y);
    }


    Vector3 AxielToCube(Vector2 hexCoords)
    {
        var x = hexCoords.x;
        var z = hexCoords.y;
        var y = -x - z;
        return new Vector3(x, y, z);
    }

    Vector2 CubeToAxiel(Vector3 coords)
    {
        var c = coords.x;
        var r = coords.y;
        return new Vector2(c, r);
    }


    Vector2 RoundHexCoords(Vector2 hexCoords)
    {
        return CubeToAxiel(RoundCubeCoords(AxielToCube(hexCoords)));
    }

    Vector3 RoundCubeCoords(Vector3 coords)
    {
        float rx = Mathf.Round(coords.x);
        float ry = Mathf.Round(coords.y);
        float rz = Mathf.Round(coords.z);

        float x_diff = Mathf.Abs(rx - coords.x);
        float y_diff = Mathf.Abs(ry - coords.y);
        float z_diff = Mathf.Abs(rz - coords.z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new Vector3(rx, ry, rz);
    }




    int CoordinatesToIndex(float column, float row)
    {
        return (int)(gridWidth * column + row);
    }






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

        Gizmos.color = Color.white;

        //Debug.Log("PTF : " + pointToFind);

        if (hexCells.Length > 0)
        {
            foreach (HexCell cell in hexCells)
            {

                Vector3[] corners = GetHexCorners(cell.worldPosition);

                //if (PointToHex(cell.cellIndex) == PointToHex(pointToFind)) {
                //    //Debug.Log(cell.cellIndex + " == " + PointToHex(cell.cellIndex));
                //    //Debug.Log(" PointToHex(pointToFind) = " + PointToHex(pointToFind));
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawWireSphere(new Vector3 (cell.worldPosition.x, 0, cell.worldPosition.y), 0.5f);

                //    Gizmos.color = Color.white;
                //}


                Handles.Label(new Vector3(cell.worldPosition.x - 0.5f, 0, cell.worldPosition.y), (cell.cellIndex.x + " | " + cell.cellIndex.y).ToString());

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
