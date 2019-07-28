using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* Attaching this component to a GameObject will create a grid of hexagons in a hexagonal layout with the 'gridRadius' and 'hexSize' properties dictating its proportions.
 * The grid will maintain its relevant psition to the object as well as rotation in the 'y' axis, though rotating in either of the other axis is not currently supported.
 * Currently, the grid cannot 'swizzle' that is to say that it is only generated on the 'x' and 'z' axis in a flat, top down plane.
 * 
 */

// This class is largely based on the explanations provided by Patel at https://www.redblobgames.com/grids/hexagons/

[ExecuteInEditMode]
public class HexagonGrid : MonoBehaviour
{

    public int gridRadius = 6;


    [SerializeField]
    public float hexSize = 0.5774f;
    public int gridWidth = 10;
    public int gridHeight = 10;

    [SerializeField]
    private float hexWidth; // used to calculate positions on the horzontal axis of the grid
    [SerializeField]
    private float hexHeight; // used to calculate positions on the vertical axis of the grid

    public float GetHexWidth()
    {
        return hexWidth;
    }

    private Vector2 pointToFind = new Vector2(0, 0);

    private Vector2 foundHexPoint;

    [SerializeField]
    private Dictionary<Vector2Int, HexCell> hexCells;

    //public GameObject testTrackerObj; // TEMP - used as a way to interact with the grid


    [System.Serializable]
    public class HexCell
    {
        public HexCell(Vector2 worldPosition, Vector2Int cellIndex)
        {
            this.worldPosition = worldPosition;
            this.cellIndex = cellIndex;
        }

        public Vector3 WorldPosition()
        {
            Vector3 returnPos = new Vector3(worldPosition.x, 0, worldPosition.y);

            return worldPosition;
        }

        public Vector2 worldPosition; // the world position of the centre of the cell. Should be relative to the grid, and also use the grid's y position
        public Vector2Int cellIndex;


        public bool isSelected = false;
    }


    Vector3 GetHexCornerVertical(Vector2 centre, float size, int i)
    {
        float angle_deg = 60 * i - 30; // For 'flat top' - don't subtract 30 degrees
        float angle_rad = Mathf.PI / 180 * angle_deg;
        Vector3 cornerPoint = new Vector3(
                      centre.x + size * Mathf.Cos(angle_rad), // + this.transform.position.x,
                      this.transform.position.y,
                      centre.y + size * Mathf.Sin(angle_rad) // + this.transform.position.z
                      );

        return RotatePointAroundPivot(cornerPoint,
                                        new Vector3(centre.x, this.transform.position.y, centre.y),
                                        new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z)
                                        );
    }

    public Vector3? GetWorldPosFromHex(Vector2Int hexIndex)
    {
        if (hexCells.ContainsKey(hexIndex))
        {
            HexCell cell = hexCells[hexIndex];

            Vector3 cellPos = new Vector3(cell.worldPosition.x, 0, cell.worldPosition.y);

            return cellPos; // + this.transform.position;


        }
        else return null;
    }

    public Vector3? GetWorldPosFromHexOffset(Vector2Int hexIndex)
    {
        if (hexCells.ContainsKey(hexIndex))
        {
            HexCell cell = hexCells[hexIndex];

            Vector3 cellPos = new Vector3(cell.worldPosition.x, 0, cell.worldPosition.y);

            return cellPos + this.transform.position;


        }
        else return null;
    }



    public Quaternion GetGridRotation()
    {
        return this.transform.rotation;
    }


    [ExecuteInEditMode]
    void OnEnable()
    {
        CreateGrid();

        SceneView.onSceneGUIDelegate -= GridUpdate;
        SceneView.onSceneGUIDelegate += GridUpdate;

        hexWidth = Mathf.Sqrt(3) * hexSize;
        hexHeight = 2 * hexSize;

    }

    private void Update()
    {
        //if (testTrackerObj != null)
        //{
        //    pointToFind = new Vector2(testTrackerObj.transform.position.x, testTrackerObj.transform.position.z);
        //    foundHexPoint = PointToHex(pointToFind);
        //}
    }


    [ExecuteInEditMode]
    void CreateGrid()
    {
        hexCells = new Dictionary<Vector2Int, HexCell>();

        int rowStartIndex = -gridRadius;
        int currentRowLength = gridRadius + 1;
        int worldRowOffset = 0;

        // Iterate for the total number of rows - begins on the bottom row and works upwards
        for (int c = -gridRadius; c <= gridRadius; c++)
        {

            worldRowOffset = c <= 0 ? worldRowOffset-- : worldRowOffset++;

            // Iterate along the row for the length of the row, begining with the current start index
            for (int r = rowStartIndex; r < rowStartIndex + currentRowLength; r++)
            {
                // Generate a world position (the actual location of the centre of the hex in world space) 
                // and a hex index - the hex cell's position relative to the grid axis
                Vector2Int hexIndex = new Vector2Int(r, c);
                Vector2 worldPos = new Vector2(
                    worldPos.x = ((hexIndex.x + // the hex index as a position
                                  (-hexIndex.y * +0.5f) + // gives the offest of the row
                                  worldRowOffset / 2) *
                                  hexWidth),
                    worldPos.y = hexIndex.y * hexHeight * 3 / 4 // Multiplied by 3/4 in order to take into account of the upper and lower corners.
                );

                hexCells[hexIndex] = new HexCell(worldPos, hexIndex);
            }
            if (c < 0)
                ++currentRowLength;
            else if (c >= 0)
            {
                --currentRowLength;
                rowStartIndex++; // increase the start index as the top of the hex grid tapers inwards. Also keeps the column values aligned on their axis
            }
        }
    }

    int tempDebugCounter = 0;

    [ExecuteInEditMode]
    private HexCell CreateCell(Vector2Int index)
    {
        Vector2 worldPos;

        worldPos.x = (index.x + index.y * +0.5f - index.y / 2) * hexWidth;
        worldPos.y = index.y * hexHeight * 3 / 4;

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

    Vector3[] GetHexCorners(Vector3 centre)
    {
        return GetHexCorners(new Vector2(centre.x, centre.z));
    }


    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        return PointToHex(new Vector2(worldPosition.x, worldPosition.z));
    }

    public Vector3? CellToWorld(Vector2Int cellIndex)
    {
        return GetWorldPosFromHex(cellIndex);
    }



    private Vector2Int PointToHex(Vector2 point)
    {
        // Account for the grid's offset relative to the point
        point.x -= this.transform.position.x;
        point.y -= this.transform.position.z;


        float c; // = ((Mathf.Sqrt(3) / 3 * point.x - 1.0f / 3 * point.y) / hexSize);

        float r; // = ((2.0f / 3 * point.y) / hexSize);


        bool useRotated = true;
        if (useRotated)
        {

            Vector3 rotatedPoint = RotatePointAroundPivot(new Vector3(point.x, 0, point.y), this.transform.position, -this.transform.rotation.eulerAngles);


            c = ((Mathf.Sqrt(3) / 3 * rotatedPoint.x - 1.0f / 3 * rotatedPoint.z) / hexSize);

            r = ((2.0f / 3 * rotatedPoint.z) / hexSize);
        }
        else
        {
            c = ((Mathf.Sqrt(3) / 3 * point.x - 1.0f / 3 * point.y) / hexSize);

            r = ((2.0f / 3 * point.y) / hexSize);
        }



        Vector2 roundedCoords = RoundHexCoords(new Vector2(c + r, r));



        //int offset = (int)point.y > 1 ?
        //    1 * ((int)point.y - 1) :
        //    1 * ((int)point.y + 1);


        //offset = (int)r;
        //offset = 0;

        Vector2 offset = new Vector2(this.transform.position.x, this.transform.position.z);

        //return new Vector2Int((int)roundedCoords.x - (int)offset.x, (int)roundedCoords.y - (int)offset.y);

        return new Vector2Int((int)roundedCoords.x, (int)roundedCoords.y);
    }


    private Vector2 RoundHexCoords(Vector2 coord)
    {
        Vector2 returnCoord = ConvertCubeToAxial(RoundCubeCoords(ConvertAxielToCube(coord)));
        return new Vector2(returnCoord.x, (int)returnCoord.y);
    }


    private Vector3 RoundCubeCoords(Vector3 coord)
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


    private Vector2 ConvertCubeToAxial(Vector3 coord)
    {
        var q = coord.x;
        var r = coord.z;
        return new Vector2(q, r);
    }

    private Vector3 ConvertAxielToCube(Vector2 coord)
    {
        var x = coord.x;
        var z = coord.y;
        var y = -x - z;
        return new Vector3(x, y, z);
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





    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point;
    }



    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (!enabled) return;

        #region Highlight Found Hex
        Gizmos.color = Color.magenta;

        Vector2Int foundPoint = PointToHex(pointToFind);

        if (hexCells.ContainsKey(foundPoint))
        {
            HexCell foundCell = hexCells[(new Vector2Int(foundPoint.x, foundPoint.y))];
            Vector3[] corners;
            //bool useRotated = true;
            //if (useRotated)
            //{

            Vector3? hexWorldPos = GetWorldPosFromHexOffset(foundCell.cellIndex);

            if (hexWorldPos != null)
            {
                Vector3 rotatedPoint = RotatePointAroundPivot(
                                                hexWorldPos.Value,
                                                this.gameObject.transform.position,
                                                new Vector3(0, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z)
                                                );

                corners = GetHexCorners(new Vector2(rotatedPoint.x, rotatedPoint.z));


                for (int i = 0; i < 6; ++i)
                    corners[i] += new Vector3(0, 0.1f, 0);

                for (int i = 0; i < 6; ++i)
                {
                    Gizmos.DrawLine(
                        corners[i],
                        corners[i == 5 ? 0 : i + 1]
                        );
                }
            }
            //}
            //else
            //{
            //    corners = GetHexCorners(new Vector2(foundCell.worldPosition.x, foundCell.worldPosition.y));
            //}


        }
        #endregion

        #region Draw Hex Board

        Gizmos.color = Color.white;

        if (hexCells.Count > 0)
        {
            foreach (KeyValuePair<Vector2Int, HexCell> cell in hexCells)
            {
                Vector3 rotatedPoint = RotatePointAroundPivot(
                    GetWorldPosFromHexOffset(cell.Value.cellIndex).Value,
                    this.gameObject.transform.position,
                    new Vector3(0, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z)
                    );

                Vector3[] corners = GetHexCorners(new Vector2(rotatedPoint.x, rotatedPoint.z));


                bool drawLabels = false;
                if (drawLabels)
                {
                    Handles.Label(new Vector3(
                        rotatedPoint.x - (hexWidth / 3),
                        this.transform.position.y,
                        rotatedPoint.z),
                        cell.Value.cellIndex.x + " | " + cell.Value.cellIndex.y
                        );
                }

                for (int i = 0; i < 6; ++i)
                {

                    Gizmos.DrawLine(
                        corners[i],
                        corners[i == 5 ? 0 : i + 1]
                        );
                }
            }
        }
        #endregion

#endif
    }

}
