using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*DEPRECATED*/ /* TODO: Make Tunnel base class from which other tunnels can be derived*/ 
public class HexTunnel : MonoBehaviour
{

    [System.Serializable]
    public class HexRing
    {
        string trailObjectPrefabPath = "Prefabs/Particles/HexTunnelPiece";

        public HexRing(Vector3 hexCentreWorldPos, int hexSize = 7)
        {
            this.hexCentreWorldPos = hexCentreWorldPos;
            this.hexSize = hexSize;

            trailObjects = new GameObject[numberOfTrailObjects];
            for (int i = 0; i < numberOfTrailObjects; i++) {
                trailObjects[i] = GameObject.Instantiate(Resources.Load(trailObjectPrefabPath) as GameObject);

                //trailObject.transform.position = this.worldPosition;
                //trailComponent = trailObject.transform.GetComponent<TrailRenderer>();

                trailObjects[i].GetComponent<TrailRenderer>().emitting = false;
            }

            trailObjects[currentTrailIndex].GetComponent<TrailRenderer>().emitting = true;
            
        }

        public Vector3 HexCentreWorldPos()
        {
            Vector3 returnPos = new Vector3(hexCentreWorldPos.x, 0, hexCentreWorldPos.y);
            return hexCentreWorldPos;
        }

        public void SetVerticalWorldPos(float newPos)
        {
            // NOTE: Could bool lerp here
            hexCentreWorldPos.y = newPos;
        }

        private Vector3 hexCentreWorldPos; // the world position of the centre of the cell.
        public int hexSize;

        public int numberOfTrailObjects = 2;

        //GameObject trailObject;
        //TrailRenderer trailComponent;

        GameObject[] trailObjects;
        int currentTrailIndex;

        //Vector3 startPos;
        //Vector3 previousNodePos;

        public void SetTrailObjectPos(Vector2 newPosition, bool isTerminalNode = false)
        {
            if (!isTerminalNode)
            {
                trailObjects[currentTrailIndex].transform.position = new Vector3(newPosition.x, hexCentreWorldPos.y, newPosition.y);
            }
            else
            {
                //previousNodePos = trailObjects[currentTrailIndex].transform.position;


                Vector3 nodeA = trailObjects[currentTrailIndex].transform.position;
                Vector3 nodeB = newPosition;
                Vector3 nodeAB = (nodeA - nodeB);
                float normalisedRemainder = 0.8f; //(lastEdgeRemainder - 0) / (edgeValue - 0);
                Vector3 normalisedPosition = (nodeA - (normalisedRemainder * nodeAB));
                Debug.Log("previousNodePos: " + nodeA + "| newPos: " + newPosition + "| normPos: " + normalisedPosition);

                trailObjects[currentTrailIndex].transform.position = new Vector3(normalisedPosition.x, this.hexCentreWorldPos.y, normalisedPosition.z);

                trailObjects[currentTrailIndex].transform.position = new Vector3(newPosition.x, hexCentreWorldPos.y, newPosition.y);
                GameManager.instance.StartCoroutine(ResetTrail());
                //GameManager.instance.StartCoroutine(ResetTrail());



                //cell.SetTrailObjectPos(cornerA - (normalisedRemainder * cornerAB), true);



                //trailObjects[currentTrailIndex].GetComponent<TrailRenderer>().AddPosition();

            }


            //trailObject.transform.position = new Vector3(Random.Range(-10, 10), trailObject.transform.position.y, Random.Range(-10, 10));
            //trailObject.transform.position = position;
            //GameManager.instance.StartCoroutine(CapTrail);
        }






        //private IEnumerator CapTrail()
        //{

        //    yield return new WaitForEndOfFrame();
        //}

        private IEnumerator ResetTrailRenderer()
        {
            TrailRenderer currentRenderer = trailObjects[currentTrailIndex].GetComponent<TrailRenderer>();
            //currentRenderer.AddPosition(trailObjects[currentTrailIndex].transform.position + (new Vector3(0, 0.1f, 0)));
            //currentRenderer.AddPosition(trailObjects[currentTrailIndex].transform.position + (new Vector3(1, 1f, 1)));
            currentRenderer.emitting = false;

            currentTrailIndex = currentTrailIndex >= trailObjects.Length - 1 ? 0 : currentTrailIndex + 1;

            // TODO: Reset position
            currentRenderer = trailObjects[currentTrailIndex].GetComponent<TrailRenderer>();

            yield return new WaitForEndOfFrame();
            currentRenderer.emitting = true;
            yield return null;

            //currentRenderer.AddPosition(trailObjects[currentTrailIndex].transform.position + (new Vector3(1, 1f, 1)));



            //trailComponent.AddPosition(trailObject.transform.position);
            //float trailTime = trailComponent.time;
            //trailComponent.AddPosition(trailObject.transform.position + (new Vector3(0, 0.1f, 0) ));

            //trailObjects[currentTrailIndex].GetComponent<TrailRenderer>().emitting = false;
            //trailComponent.time = 0;
            //yield return new WaitForFixedUpdate();
            //trailComponent.time = trailTime;
            //trailObjects[currentTrailIndex].GetComponent<TrailRenderer>().emitting = true;
            //trailComponent.Clear();
        }



        public void NextTrail(Vector2 startPos)
        {
            // "reset" + disable old trail, increase index, enable new trail ("reset"?)

          
            GameManager.instance.StartCoroutine(ResetTrail());

        }

        private IEnumerator ResetTrail()
        {
            GameObject oldTrailObject = trailObjects[currentTrailIndex];
            TrailRenderer oldTrail = oldTrailObject.GetComponent<TrailRenderer>();
            //oldTrail.AddPosition(trailObjects[currentTrailIndex].transform.position + (new Vector3(0, 0.1f, 0)));

            currentTrailIndex = currentTrailIndex >= trailObjects.Length - 1 ? 0 : currentTrailIndex + 1;


            oldTrail.emitting = false;
            //yield return new WaitForEndOfFrame();
            yield return null;
            TrailRenderer newTrail = trailObjects[currentTrailIndex].GetComponent<TrailRenderer>();
            newTrail.emitting = true;

            Vector2 resetPos = GetHexCornerVertical(hexCentreWorldPos, hexSize, 0);
            oldTrailObject.transform.position = new Vector3(resetPos.x, hexCentreWorldPos.y, resetPos.y);

        }


        Vector2 GetHexCornerVertical(Vector3 centre, float size, int i)
        {
            float angle_deg = 60 * i - 30; // For 'flat top' - don't subtract 30 degrees   
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector2 cornerPoint = new Vector2(
                          centre.x + size * Mathf.Cos(angle_rad), // + this.transform.position.x,
                          centre.z + size * Mathf.Sin(angle_rad)
                          //centre.y  // + this.transform.position.z
                          );

            return cornerPoint;
            //return RotatePointAroundPivot(cornerPoint,
            //                                new Vector3(centre.x, this.transform.position.y, centre.y),
            //                                new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z)
            //                                );
        }
    }



    Vector2 GetHexCornerVertical(Vector3 centre, float size, int i)
    {
        float angle_deg = 60 * i - 30; // For 'flat top' - don't subtract 30 degrees   
        float angle_rad = Mathf.PI / 180 * angle_deg;
        Vector2 cornerPoint = new Vector2(
                      centre.x + size * Mathf.Cos(angle_rad), // + this.transform.position.x,
                      centre.z + size * Mathf.Sin(angle_rad)
                      //centre.y  // + this.transform.position.z
                      );

        return cornerPoint;
        //return RotatePointAroundPivot(cornerPoint,
        //                                new Vector3(centre.x, this.transform.position.y, centre.y),
        //                                new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z)
        //                                );
    }



    public int edgeValue = 20; // the value assigned to each edge length
    [Range(0, 120)]
    public float valueToDisplay = 60; // the sum value for edges to display
    private float lastEdgeRemainder;
    private int currentItteration;
    public int lastWholeItteration = 6; // temp - for debugging and testing
    //private bool reverseDirection = false;

    /* Variables for handling vertical tunnel movement*/
    float targetX = 0;
    float targetz = 0;
    int minmaxX = 4;
    bool isGoingDown;


    public int numberOfObjects = 4;
    public float distanceBetweenObjects = 30; // The Y distance between each object. 

    //public GameObject objectToSpawn;

    private Queue<HexRing> tunnelPieces;
    private float currentTopObjectYpos;
    private float currentBottomObjectYpos;






    // Start is called before the first frame update
    void Start()
    {
        // mathewfalvai@gmail.com
        SpawnTunnelPieces();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateIndex();
        DrawHexes();
    }

    public void SetEnergyValue(float value)
    {
        valueToDisplay = value;
    }

    void SpawnTunnelPieces()
    {
        tunnelPieces = new Queue<HexRing>();

        float initialYPos = this.transform.position.y;
        currentTopObjectYpos = initialYPos;

        for (int i = 0; i < numberOfObjects; ++i)
        {
            tunnelPieces.Enqueue(new HexRing(new Vector3(0,initialYPos, 0)));
            initialYPos -= distanceBetweenObjects;
        }
    }

    private void CalculateIndex()
    {
        lastWholeItteration = Mathf.FloorToInt(valueToDisplay / edgeValue);
        lastEdgeRemainder = valueToDisplay % edgeValue;
    }

    private void DrawHexes()
    {
        
        if (currentItteration == 0) HandleMoveTunnelDown(); //Performed on first itteration so as to start from first corner


        foreach (HexRing cell in tunnelPieces)
        {
            if (currentItteration < lastWholeItteration) // Draw full line
            {
                cell.SetTrailObjectPos(GetHexCornerVertical(cell.HexCentreWorldPos(), cell.hexSize, currentItteration));
            }
            else if (currentItteration >= lastWholeItteration && lastWholeItteration > 0) // Draw remainder line at end
            {
                Vector2 cornerA = GetHexCornerVertical(cell.HexCentreWorldPos(), cell.hexSize, currentItteration - 1);
                Vector2 cornerB = GetHexCornerVertical(cell.HexCentreWorldPos(), cell.hexSize, currentItteration);
                Vector2 cornerAB = (cornerA - cornerB);
                float normalisedRemainder = (lastEdgeRemainder - 0) / (edgeValue - 0);
                cell.SetTrailObjectPos(cornerA - (normalisedRemainder * cornerAB), true);
            }
            else if (lastWholeItteration == 0) // Draw remainder line at start
            {
                Vector2 cornerA = GetHexCornerVertical(cell.HexCentreWorldPos(), cell.hexSize, 0);
                Vector2 cornerB = GetHexCornerVertical(cell.HexCentreWorldPos(), cell.hexSize, 1);
                Vector2 cornerAB = (cornerA - cornerB);
                float normalisedRemainder = (lastEdgeRemainder - 0) / (edgeValue - 0);
                cell.SetTrailObjectPos(cornerA - (normalisedRemainder * cornerAB), true);
            }
        }
        currentItteration = currentItteration < lastWholeItteration ? currentItteration + 1 : 0;
    }

    void HandleMoveTunnelDown()
    {
        // Note: isReversing is currently NOT in use, but the intention is to automatically handle tunnel movement upwards too
        if (!isGoingDown)
        {
            targetX += Time.deltaTime;
            targetz += Time.deltaTime;
        }
        else
        {
            targetX -= Time.deltaTime;
            targetz -= Time.deltaTime;
        }
        if (targetX > minmaxX || targetX < -minmaxX) isGoingDown = !isGoingDown;


        if (currentTopObjectYpos > MapSpawner.Instance.GetCurrentMapHolder().transform.position.y)
        {
            // if (!isGoingDown) ReverseQueue();

            HexRing pieceToMove = tunnelPieces.Dequeue();
            //StartCoroutine(pieceToMove.ResetTrailRenderer());
            pieceToMove.SetVerticalWorldPos(currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)));
            //StartCoroutine(pieceToMove.ResetTrailRenderer()); // TODO: start new trail
            currentBottomObjectYpos = pieceToMove.HexCentreWorldPos().y;
            currentTopObjectYpos -= distanceBetweenObjects;
            tunnelPieces.Enqueue(pieceToMove);
        }
    }

    //void HandleTunnelMove()
    //{
    //    Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;

    //    Vector3 targetPos;


    //    if (isGoingDown) // Going Down
    //    {
    //        if (currentTopObjectYpos < mapPos.y /*- distanceBetweenObjects*/)
    //        {
    //            ReverseQueueUtil<HexRing>.ReverseQueue(ref tunnelPieces);
    //            isGoingDown = !isGoingDown;
    //            return;
    //        }

    //        if (currentTopObjectYpos > mapPos.y)
    //        {

    //            HexRing pieceToMove = tunnelPieces.Dequeue();
    //            //StartCoroutine(pieceToMove.ResetTrailRenderer());
    //            pieceToMove.SetVerticalWorldPos(currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)));
    //            //StartCoroutine(pieceToMove.ResetTrailRenderer()); // TODO: start new trail
    //            currentBottomObjectYpos = pieceToMove.HexCentreWorldPos().y;
    //            currentTopObjectYpos -= distanceBetweenObjects;
    //            tunnelPieces.Enqueue(pieceToMove);

    //            //GameObject pieceToMove = tunnelPieces.Dequeue();
    //            //targetPos = new Vector3(mapPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), mapPos.z);
    //            //IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
    //            //StartCoroutine(coroutine);
    //            //currentBottomObjectYpos = targetPos.y;
    //            //currentTopObjectYpos -= distanceBetweenObjects;
    //            //tunnelPieces.Enqueue(pieceToMove);

    //        }
    //    }
    //    else // Going Up
    //    {
    //        if (currentTopObjectYpos > mapPos.y + distanceBetweenObjects)
    //        {
    //            ReverseQueueUtil<HexRing>.ReverseQueue(ref tunnelPieces);
    //            isGoingDown = !isGoingDown;
    //            return;
    //        }

    //        if (currentTopObjectYpos < mapPos.y - distanceBetweenObjects)
    //        {
    //            //GameObject pieceToMove = tunnelPieces.Dequeue();
    //            //targetPos = new Vector3(mapPos.x, currentTopObjectYpos + distanceBetweenObjects, mapPos.z);
    //            //IEnumerator coroutine = MoveTo(pieceToMove, targetPos, 2f);
    //            //StartCoroutine(coroutine);
    //            //currentBottomObjectYpos = pieceToMove.transform.position.y + distanceBetweenObjects;
    //            //currentTopObjectYpos = targetPos.y;
    //            //tunnelPieces.Enqueue(pieceToMove);

    //        }
    //    }
    //}
}
