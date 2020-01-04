using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTunnelEnergy : MonoBehaviour
{


    private Queue<GameObject> tunnelPieces;
    private float currentTopObjectYpos;
    private float currentBottomObjectYpos;
    private Material energyFillMat;
    private Material energyBackgroundMat;
    private float valueToDisplay;
    private float maxValue { get; set; }

    public int numberOfObjects;
    public float distanceBetweenObjects;
    public float planeMeshSize;



    public void Initialise(int noOfRings = 4, float ringDist = 30, float ringSize = 7, float maxVal = 120)
    {
        numberOfObjects = noOfRings;
        distanceBetweenObjects = ringDist;
        planeMeshSize = ringSize;
        maxValue = maxVal;
        LoadMaterials();
        SpawnTunnelPieces();
    }


    // Update is called once per frame
    void Update()
    {
        HandleTunnelMove();
        if (energyFillMat != null) energyFillMat.SetFloat("_Arc1", CalcFillValue());
    }


    float CalcFillValue()
    {
        float returnVal;

        float inValAsPercentage = (valueToDisplay * 100) / maxValue;
        //Debug.Log(valueToDisplay);
        //Debug.Log(maxValue);
        //Debug.Log("inValAsPercentage = " +inValAsPercentage);


        returnVal = 360 - ((360 / 100) * inValAsPercentage);

        return returnVal;
    }

    void LoadMaterials()
    {
        energyFillMat = Resources.Load("Materials/EnergyMetre/EnergyMetreFillMat", typeof(Material)) as Material;
        energyBackgroundMat = Resources.Load("Materials/EnergyMetre/EnergyMetreBackMat", typeof(Material)) as Material;
        if (energyFillMat == null) throw new System.Exception("energyFillMat not found. Failed to Load.");
        if (energyFillMat == null) throw new System.Exception("energyBackgroundMat not found. Failed to Load");
    }

    void SpawnTunnelPieces()
    {
        tunnelPieces = new Queue<GameObject>();

        float initialYPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position.y + distanceBetweenObjects;

        currentTopObjectYpos = initialYPos - distanceBetweenObjects;

        for (int i = 0; i < numberOfObjects; ++i)
        {
            GameObject newHex = SpawnHex();
            if (newHex != null)
            {
                newHex.transform.position = MapSpawner.Instance.GetCurrentMapHolder().transform.position 
                                            + new Vector3(0, initialYPos -= distanceBetweenObjects, 0);
                tunnelPieces.Enqueue(newHex);
            }
        }
    }

    GameObject SpawnHex()
    {
        GameObject newHex = new GameObject("EnergyHexRing");
        GameObject fillRing = new GameObject("FillRing");
        fillRing.AddComponent<MeshFilter>().mesh = CreateMesh(planeMeshSize, planeMeshSize);
        fillRing.AddComponent<MeshRenderer>().material = energyFillMat;
        fillRing.GetComponent<MeshRenderer>().material = energyFillMat;
        fillRing.transform.SetParent(newHex.transform);

        GameObject backRing = new GameObject("BackRing");
        backRing.AddComponent<MeshFilter>().mesh = CreateMesh(planeMeshSize, planeMeshSize);
        backRing.AddComponent<MeshRenderer>().material = energyBackgroundMat;
        backRing.transform.SetParent(newHex.transform);
        backRing.transform.localPosition += new Vector3(0, -0.1f, 0);

        return newHex;
    }

    Mesh CreateMesh(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        m.vertices = new Vector3[] {
            new Vector3(height,  0.01f, width),
            new Vector3(height, 0.01f,  -width),
            new Vector3(-height, 0.01f, -width),
            new Vector3(-height,  0.01f, width)
        };
        m.uv = new Vector2[] {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2(1, 1),
            new Vector2 (1, 0)
        };

        //m.triangles = new int[] { 0, 2, 3, 0, 1, 2 };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        m.RecalculateNormals();

        return m;
    }

    public void SetEnergyValue(float value)
    {
        valueToDisplay = value;
    }

    void HandleTunnelMove()
    {
        Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;

        if (currentTopObjectYpos > mapPos.y)
        {
            // if (!isGoingDown) ReverseQueue();


            //NOTE: Multiply Lerp speed by y distance from ap holder to make a curveWhat 

            GameObject pieceToMove = tunnelPieces.Dequeue();
            //pieceToMove.targetYPos = currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1));

            Vector3 targetPos = new Vector3(mapPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), mapPos.z);
            IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
            StartCoroutine(coroutine);

            //pieceToMove.transform.position = new Vector3(mapPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), mapPos.z);
            currentBottomObjectYpos = pieceToMove.transform.position.y;
            currentTopObjectYpos -= distanceBetweenObjects;
            tunnelPieces.Enqueue(pieceToMove);
        }
    }

    IEnumerator MoveTo(GameObject obj, Vector3 position, float time)
    {
        Vector3 start = obj.transform.position;
        Vector3 end = position;
        float t = 0;

        while (t < 1)
        {
            yield return null;
            t += Time.deltaTime / time;
            obj.transform.position = Vector3.Lerp(start, end, t);
        }
        obj.transform.position = end;

    }

}
