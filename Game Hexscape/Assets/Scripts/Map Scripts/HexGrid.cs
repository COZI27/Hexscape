using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class HexMetrics
{
    public const float outerToInner = 0.866025404f;

    public const float outerRadius = 0.5774f;

    public const float innerRadius = outerRadius * outerToInner;


    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };



}


//https://www.redblobgames.com/grids/hexagons/
//https://www.redblobgames.com/grids/hexagons/#pixel-to-hex



[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HexCoordinates coordinates = new HexCoordinates(
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("z").intValue
        );

        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, coordinates.ToString());
    }
}

[System.Serializable]
public struct HexCoordinates
{

    [SerializeField]
    private int x, z;

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }


    public override string ToString()
    {
        return "(" +
            X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
}

[System.Serializable]
public class HexCell
{
    public HexCell(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
    }

    public HexCoordinates coordinates;


    [SerializeField]
    public Vector3 worldPosition;


    public bool isHighlighted = false;
}


[ExecuteInEditMode]
public class HexGrid : MonoBehaviour
{

    
    //private Vector2 PointToHexCoord(Vector2 point)
    //{
    //    var q = (Mathf.Sqrt(3) / 3 * point.x - 1.0f / 3 * point.y) / size;

    //}


    private int highlightedCell = 4;

    //GameObject trailRenderPrefab;

    public int width = 6;
    public int height = 6;

    [SerializeField]
    public HexCell[] cells;



    [ExecuteInEditMode]
    void OnEnable()
    {
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }

        SceneView.onSceneGUIDelegate -= GridUpdate;
        SceneView.onSceneGUIDelegate += GridUpdate;

        cursorObject = Instantiate(cursorObject) as GameObject;
    }


    Vector3 cursorPos;
    public GameObject cursorObject;

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
        Vector3 spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

        cursorPos = new Vector3(spawnPosition.x, 0, spawnPosition.z) ;
        if (cursorObject != null) cursorObject.transform.position = cursorPos;
        //Debug.Log(spawnPosition);
    }

    [ExecuteInEditMode]
    void CreateCell(int x, int z, int i)
    {

        Vector3 position;
        position.x = (x + z * +0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = new HexCell(position);
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
    }



    // private Vector2 GetHoveredHexagon(int x, int y)
    // {

        // float halfWidth

        // // Find the row and column of the box that the point falls in.
        // int row = (int)(y / height);
        // int column;

        // bool rowIsOdd = row % 2 == 1;

        // // Is the row an odd number?
        // if (rowIsOdd)// Yes: Offset x to match the indent of the row
            // column = (int)((x - halfWidth) / width);
        // else// No: Calculate normally
            // column = (int)(x / width);


        // //

        // // Work out the position of the point relative to the box it is in
        // double relY = y - (row * height);
        // double relX;

        // if (rowIsOdd)
            // relX = (x - (column * width)) - halfWidth;
        // else
            // relX = x - (column * width);

        // // Work out if the point is above either of the hexagon's top edges
        // if (relY < (-m * relX) + c) // LEFT edge
        // {
            // row--;
            // if (!rowIsOdd)
                // column--;
        // }
        // else if (relY < (m * relX) - c) // RIGHT edge
        // {
            // row--;
            // if (rowIsOdd)
                // column++;
        // }

        // return hexagons[column][row];
    // }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        foreach (HexCell cell in cells)
        {
            if (cell.isHighlighted)
            {
                Gizmos.color = Color.green;
            }
            else Gizmos.color = Color.white;

            for (int i = 0; i < 6; ++i)
            {
                Vector3 center = cell.worldPosition;

                Gizmos.DrawLine(
                    center + HexMetrics.corners[i],
                    center + HexMetrics.corners[i == 5 ? 0 : i + 1]
                    );
            }
        }

        //Gizmos.DrawSphere(cursorPos, 1.0f);
#endif
    }

void OnDrawGizmosSelected()
    {
//#if UNITY_EDITOR
//        foreach (HexCell cell in cells)
//        {
//            if (cell.isHighlighted) Gizmos.color = Color.green;
//            else Gizmos.color = Color.gray;

//            for (int i = 0; i < 6; ++i)
//            {
//                Vector3 center = cell.worldPosition;

//                Gizmos.DrawLine(
//                    center + HexMetrics.corners[i],
//                    center + HexMetrics.corners[ i == 5 ? 0 : i +1 ]
//                    );
//            }
//        }
//#endif
 }




}
