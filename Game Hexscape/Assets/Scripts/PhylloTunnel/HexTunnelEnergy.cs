using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTunnelEnergy : MonoBehaviour
{


    private Queue<GameObject> tunnelPieces;
    private float currentTopObjectYpos;
    private float currentBottomObjectYpos;
    private Material energyFillMat;
    private Material energyAccelerationFillMat;
    private Material energyBackgroundMat;
    private float energyFill, accelerationFill;
    private float targetEnergyFill;

    public float fillRate = 2;

    public int numberOfObjects;
    public float distanceBetweenObjects;
    public float planeMeshSize;

    public bool isGoingDown = true;

    private bool isPlayingEffect = false; // while true - new inputs will be ignored until effect is ended

    public void Initialise(int noOfRings = 4, float ringDist = 30, float ringSize = 7)
    {
        numberOfObjects = noOfRings;
        distanceBetweenObjects = ringDist;
        planeMeshSize = ringSize;
        // maxValue = maxVal;
        LoadMaterials();
        SpawnTunnelPieces();
        GameManager.instance.StartCoroutine(StandardBehaviour());
    }


    void Update()
    {
        HandleTunnelMove();
    }

    void UpdateFillValues(float arc1Val, float arc2Val, float accelerationFill)
    {
        //Debug.Log("UpdateFillValues " + arc1Val + ", " + arc2Val);

        //float energyFillVal;
        float energyArc1Val, energyArc2Val;

        float acceFilllVal;

        energyArc1Val = CalcFillDegreesValue(arc1Val);
        energyArc2Val = CalcFillDegreesValue(arc2Val);
        acceFilllVal = CalcFillDegreesValue(accelerationFill);

        if (energyFillMat != null)
        {
            energyFillMat.SetFloat("_Arc1", energyArc1Val);
            energyFillMat.SetFloat("_Arc2", energyArc2Val);
        }
        if (energyFillMat != null) energyAccelerationFillMat.SetFloat("_Arc1", acceFilllVal);
    }

    float CalcFillDegreesValue(float fill)
    {
        float val = (1 - fill) * 360;
        val = Mathf.Clamp(val, 0, 360);
        return val;
    }


    void LoadMaterials()
    {
        energyFillMat = Resources.Load("Materials/EnergyMetre/EnergyMetreFillMat", typeof(Material)) as Material;
        energyBackgroundMat = Resources.Load("Materials/EnergyMetre/EnergyMetreBackMat", typeof(Material)) as Material;
        energyAccelerationFillMat = Resources.Load("Materials/EnergyMetre/AccelerationEnergyMetreFillMat", typeof(Material)) as Material;

        if (energyFillMat == null) throw new System.Exception("energyFillMat not found. Failed to Load.");
        if (energyBackgroundMat == null) throw new System.Exception("energyBackgroundMat not found. Failed to Load");
        if (energyAccelerationFillMat == null) throw new System.Exception("energyAccelerationFillMat not found. Failed to Load");
    }

    void SpawnTunnelPieces()
    {
        tunnelPieces = new Queue<GameObject>();

        Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;

        for (int i = 0; i < numberOfObjects; ++i)
        {
            GameObject newHex = SpawnHex();
            if (newHex != null)
            {
                if (i == 0)
                {
                    newHex.transform.position = mapPosition - new Vector3(0, distanceBetweenObjects / 2, 0);
                    currentTopObjectYpos = newHex.transform.position.y;
                }
                else
                {
                    newHex.transform.position = new Vector3(mapPosition.x, currentTopObjectYpos - (distanceBetweenObjects * i), mapPosition.z);
                }
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
        fillRing.transform.SetParent(newHex.transform);

        GameObject backRing = new GameObject("BackRing");
        backRing.AddComponent<MeshFilter>().mesh = CreateMesh(planeMeshSize, planeMeshSize);
        backRing.AddComponent<MeshRenderer>().material = energyBackgroundMat;
        backRing.transform.SetParent(newHex.transform);
        backRing.transform.localPosition += new Vector3(0, -0.1f, 0);

        GameObject accelerationRing = new GameObject("accelerationRing");
        accelerationRing.AddComponent<MeshFilter>().mesh = CreateMesh(planeMeshSize, planeMeshSize);
        accelerationRing.AddComponent<MeshRenderer>().material = energyAccelerationFillMat;
        accelerationRing.transform.SetParent(newHex.transform);
        accelerationRing.transform.localPosition += new Vector3(0, -0.2f, 0);
        accelerationRing.transform.localScale += new Vector3(0.06f, 0.06f, 0.06f);

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

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        m.RecalculateNormals();

        return m;
    }

    /// <summary>
    /// Set the display values for both the life ring and acceleration ring
    /// </summary>
    /// <param name="energyFill">
    ///  The fraction of energy the player has between death and their next tier
    /// </param>
    /// <param name="accelerationFill">
    ///  The fraction of acceleration the player has for gaining life
    /// </param>
    /// 
    public void SetEnergyFill(float energyFill, float accelerationFill = 0, bool lerpFill = true)
    {
        if (lerpFill)
        {
            this.targetEnergyFill = energyFill;
            this.accelerationFill = accelerationFill;
        }
        else
        {
            this.energyFill = energyFill;
            this.accelerationFill = accelerationFill;
        }
    }


    //void HandleTunnelMove()
    //{    
    //    Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;

    //    if (currentTopObjectYpos > mapPos.y)
    //    {
    //        // if (!isGoingDown) ReverseQueue();

    //        GameObject pieceToMove = tunnelPieces.Dequeue();

    //        Vector3 targetPos = new Vector3(mapPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), mapPos.z);
    //        IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
    //        StartCoroutine(coroutine);

    //        currentBottomObjectYpos = pieceToMove.transform.position.y;
    //        currentTopObjectYpos -= distanceBetweenObjects;
    //        tunnelPieces.Enqueue(pieceToMove);
    //    }
    //}

    void HandleTunnelMove()
    {
        Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;

        Vector3 targetPos;


        if (isGoingDown) // Going Down
        {
            if (currentTopObjectYpos < mapPos.y /*- distanceBetweenObjects*/)
            {
                /*tunnelPieces = */
                ReverseQueueUtil<GameObject>.ReverseQueue(ref tunnelPieces);
                isGoingDown = !isGoingDown;
                return;
            }

            if (currentTopObjectYpos > mapPos.y)
            {
                GameObject pieceToMove = tunnelPieces.Dequeue();
                targetPos = new Vector3(mapPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), mapPos.z);
                IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
                StartCoroutine(coroutine);
                currentBottomObjectYpos = targetPos.y;
                currentTopObjectYpos -= distanceBetweenObjects;
                tunnelPieces.Enqueue(pieceToMove);

            }
        }
        else // Going Up
        {
            if (currentTopObjectYpos > mapPos.y + distanceBetweenObjects)
            {
                /*tunnelPieces = */
                ReverseQueueUtil<GameObject>.ReverseQueue(ref tunnelPieces);
                isGoingDown = !isGoingDown;
                return;
            }

            if (currentTopObjectYpos < mapPos.y - distanceBetweenObjects)
            {
                GameObject pieceToMove = tunnelPieces.Dequeue();
                targetPos = new Vector3(mapPos.x, currentTopObjectYpos + distanceBetweenObjects, mapPos.z);
                IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
                StartCoroutine(coroutine);
                currentBottomObjectYpos = pieceToMove.transform.position.y + distanceBetweenObjects;
                currentTopObjectYpos = targetPos.y;
                tunnelPieces.Enqueue(pieceToMove);

            }
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

    public void FallDestroy()
    {
        while (tunnelPieces.Count > 0)
        {
            GameObject piece = tunnelPieces.Dequeue();
            GameObject.Destroy(piece);
        }
        GameObject.Destroy(this);
    }

    public void PlayTierUpEffect() {       
        GameManager.instance.StartCoroutine(TierUpEffect());
    }


    IEnumerator StandardBehaviour()
    {
        while (!isPlayingEffect)
        { 
            energyFill = Mathf.Lerp(energyFill, targetEnergyFill, Time.deltaTime * fillRate);

            UpdateFillValues(energyFill, 360, accelerationFill);

            yield return null;
        }
        yield break;
    }

    IEnumerator TierUpEffect(float rate = 2.0f)
    {


        isPlayingEffect = true;

        float arc1Val = energyFill;
        float arc2Val = 1;

        int effectInc = 0;

        float accelerationFill = 0;

        int defaultFlickerDilay = 3;
        int flickerFrameDelay = defaultFlickerDilay;

        while (isPlayingEffect)
        {
            if (flickerFrameDelay == 0)
            {
                accelerationFill = accelerationFill == 0 ? 1 : 0;
                flickerFrameDelay = defaultFlickerDilay;
            }
            else flickerFrameDelay--;



            switch (effectInc)
            {
                case 0: //time
                    arc1Val += Time.deltaTime * rate;
                    if (arc1Val >= 1)
                    {
                        arc1Val = 1;
                        effectInc++;
                    }
                    break;

                case 1: //time
                    arc2Val -= Time.deltaTime * rate;
                    if (arc2Val <= 0)
                    {
                        arc2Val = 0;
                        effectInc++;
                    }
                    break;

                case 2://instant
                    arc1Val = 1;
                    arc2Val = 0;
                    energyFill = 0;
                    effectInc++;
                    break;
                case 3:
                    isPlayingEffect = false;
                    break;
            }
            UpdateFillValues(arc1Val, arc2Val, accelerationFill);
            yield return null;
        }
        //Debug.Log("Tier Up Effect inc val = " + effectInc + "arc1Vals = " + arc1Val + ", " + arc2Val);


        yield return StartCoroutine(StandardBehaviour());

        

    }

}
