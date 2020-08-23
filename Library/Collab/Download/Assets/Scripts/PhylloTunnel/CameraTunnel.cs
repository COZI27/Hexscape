using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTunnel : MonoBehaviour {

    public int numberOfObjects = 4;
    public float distanceBetweenObjects = 30; // The Y distance between each object. 

    public GameObject objectToSpawn;

    private Queue<PhylloTunnelPiece>  tunnelPieces;
    private float currentTopObjectYpos;
    private float currentBottomObjectYpos;

    float targetX = 0;
    float targetz = 0;
    int minmaxX = 4;
    public bool isGoingDown = true;

    void Start () {
        SpawnTunnelPieces();
    }

    // Spawns a series of tunnel piece objects defined by the 'objectToSpawn' variable. If the objects contain the PhylloTunnelPiece script, then a reference to them is stored in 'tunnelPieces', else the object is destroyed.
    void SpawnTunnelPieces() {
        tunnelPieces = new Queue<PhylloTunnelPiece>();

        float initialYPos = this.transform.position.y;
        currentTopObjectYpos = initialYPos;

        for (int i = 0; i < numberOfObjects; ++i) {

            GameObject newObject = Instantiate(objectToSpawn, new Vector3(this.transform.position.x, initialYPos, this.transform.position.z), Quaternion.identity, GameManager.instance.transform) as GameObject;
            if (newObject != null) {
                PhylloTunnelPiece script = newObject.GetComponent<PhylloTunnelPiece>();
                if (script != null) {
                    tunnelPieces.Enqueue(script);
                    script.targetYPos = initialYPos;
                    currentBottomObjectYpos = initialYPos;
                }
            }

            initialYPos -= distanceBetweenObjects;
        }
    }
	
	void Update () {
        HandleTunnelMove();
    }

    // Automatically handles the movement of tunnel pieces in the Y axis. Intended to be called in the Update metho
    void HandleTunnelMove()
    {
        //Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
        Vector3 ownerPos = this.transform.position;

        Vector3 targetPos;


        if (isGoingDown) // Going Down
        {
            if (currentTopObjectYpos < ownerPos.y /*- distanceBetweenObjects*/)
            {
                /*tunnelPieces = */
                ReverseQueueUtil<PhylloTunnelPiece>.ReverseQueue(ref tunnelPieces);
                isGoingDown = !isGoingDown;
                return;
            }

            if (currentTopObjectYpos > ownerPos.y)
            {

                PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
              //  Debug.Log("Tunnel Y Pos = " + pieceToMove.gameObject.transform.position);
                targetPos = new Vector3(ownerPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), ownerPos.z);
              //  Debug.Log("Target Y Pos = " + targetPos);

                pieceToMove.targetYPos = targetPos.y;
                currentBottomObjectYpos = targetPos.y;
                currentTopObjectYpos -= distanceBetweenObjects;
                tunnelPieces.Enqueue(pieceToMove);

            }
        }
        else // Going Up
        {
            if (currentTopObjectYpos > ownerPos.y + distanceBetweenObjects)
            {
                /*tunnelPieces = */
                ReverseQueueUtil<PhylloTunnelPiece>.ReverseQueue(ref tunnelPieces);
                isGoingDown = !isGoingDown;
                return;
            }

            if (currentTopObjectYpos < ownerPos.y - distanceBetweenObjects)
            {
                targetPos = new Vector3(ownerPos.x, currentTopObjectYpos + distanceBetweenObjects, ownerPos.z);
                PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
                pieceToMove.targetYPos = targetPos.y;
                currentBottomObjectYpos = pieceToMove.transform.position.y + distanceBetweenObjects;
                currentTopObjectYpos = targetPos.y;
                tunnelPieces.Enqueue(pieceToMove);

            }
        }



    }
}



//void HandleTunnelMove()
//{
//    //Vector3 mapPos = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
//    Vector3 ownerPos = this.transform.position;


//    Vector3 targetPos;


//    if (isGoingDown) // Going Down
//    {
//        if (currentTopObjectYpos < ownerPos.y /*- distanceBetweenObjects*/)
//        {
//            /*tunnelPieces = */
//            ReverseQueueUtil<PhylloTunnelPiece>.ReverseQueue(ref tunnelPieces);
//            isGoingDown = !isGoingDown;
//            return;
//        }

//        if (currentTopObjectYpos > ownerPos.y)
//        {
//            //PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
//            //pieceToMove.targetYPos = currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1));
//            //currentBottomObjectYpos = pieceToMove.targetYPos;
//            //currentTopObjectYpos -= distanceBetweenObjects;
//            //tunnelPieces.Enqueue(pieceToMove);

//            targetPos = new Vector3(ownerPos.x, currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1)), ownerPos.z);
//            PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
//            pieceToMove.targetYPos = targetPos.y;
//            currentBottomObjectYpos = targetPos.y;
//            currentTopObjectYpos -= distanceBetweenObjects;
//            tunnelPieces.Enqueue(pieceToMove);

//        }
//    }
//    else // Going Up
//    {
//        if (currentTopObjectYpos > ownerPos.y + distanceBetweenObjects)
//        {
//            /*tunnelPieces = */
//            ReverseQueueUtil<PhylloTunnelPiece>.ReverseQueue(ref tunnelPieces);
//            isGoingDown = !isGoingDown;
//            return;
//        }

//        if (currentTopObjectYpos < ownerPos.y - distanceBetweenObjects)
//        {
//            //PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
//            //pieceToMove.targetYPos = currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1));
//            //currentBottomObjectYpos = pieceToMove.targetYPos;
//            //currentTopObjectYpos -= distanceBetweenObjects;
//            //tunnelPieces.Enqueue(pieceToMove);

//            targetPos = new Vector3(ownerPos.x, currentTopObjectYpos + distanceBetweenObjects, ownerPos.z);
//            PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
//            pieceToMove.targetYPos = targetPos.y;
//            currentBottomObjectYpos = pieceToMove.transform.position.y + distanceBetweenObjects;
//            currentTopObjectYpos = targetPos.y;
//            tunnelPieces.Enqueue(pieceToMove);

//        }
//    }