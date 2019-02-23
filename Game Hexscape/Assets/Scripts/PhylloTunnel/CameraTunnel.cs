using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTunnel : MonoBehaviour {

    public int numberOfObjects = 4;
    public float distanceBetweenObjects = 30; // The Y distance between each object. 

    public GameObject objectToSpawn;

    private Queue<PhylloTunnelPiece>  tunnelPieces;

    bool isGoingDown = true;

    float bottomRingYPos, topRingYPos;

    void Start () {
        SpawnTunnelPieces();
    }

    // Spawns a series of tunnel piece objects defined by the 'objectToSpawn' variable. If the objects contain the PhylloTunnelPiece script, then a reference to them is stored in 'tunnelPieces', else the object is destroyed.
    void SpawnTunnelPieces()
    {
        tunnelPieces = new Queue<PhylloTunnelPiece>();

        float initialYPos = this.transform.position.y - 20;

        topRingYPos = initialYPos;

        for (int i = 0; i < numberOfObjects; ++i)
        {
            GameObject newObject = Instantiate(objectToSpawn, new Vector3(this.transform.position.x, initialYPos, this.transform.position.z), Quaternion.identity, GameManager.instance.transform) as GameObject;
            if (newObject != null)
            {
                PhylloTunnelPiece script = newObject.GetComponent<PhylloTunnelPiece>();
                if (script != null)
                {
                    if (i == 0) topRingYPos = newObject.transform.position.y;
                    tunnelPieces.Enqueue(script);
                    script.targetYPos = initialYPos;
                }
                //else Destroy(newObject);
            }

            initialYPos -= distanceBetweenObjects;
        }

        bottomRingYPos = initialYPos + distanceBetweenObjects;

    }
	
	void Update () {
        HandleMoveTunnelDown();
    }

    // Automatically handles the movement of tunnel pieces in the Y axis. Intended to be called in the Update method.
    void HandleMoveTunnelDown() {

        if (isGoingDown) // Going Down
        {
            if (this.transform.position.y < topRingYPos)
            {
                PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
                pieceToMove.targetYPos = bottomRingYPos - distanceBetweenObjects;
                tunnelPieces.Enqueue(pieceToMove);

                topRingYPos -= distanceBetweenObjects;
                bottomRingYPos -= distanceBetweenObjects;
            }
            if (this.transform.position.y > topRingYPos + distanceBetweenObjects)
            {
                ReverseQueue();
            }
        }
        else // Going Up
        {
            if (this.transform.position.y > topRingYPos)
            {
                PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
                pieceToMove.targetYPos = topRingYPos + distanceBetweenObjects;
                tunnelPieces.Enqueue(pieceToMove);

                topRingYPos += distanceBetweenObjects;
                bottomRingYPos += distanceBetweenObjects;
            }
            if (this.transform.position.y < topRingYPos - (distanceBetweenObjects * 2))
            {
                ReverseQueue();
            }
        }

        //if (currentFirstObjectYPos > this.transform.position.y) {
        //    PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
        //    pieceToMove.targetYPos = currentFirstObjectYPos - (distanceBetweenObjects * (numberOfObjects - 1));
        //    currentBottomObjectYpos = pieceToMove.targetYPos;
        //    currentFirstObjectYPos -= distanceBetweenObjects;
        //    tunnelPieces.Enqueue(pieceToMove);
        //}

    }

    void ReverseQueue()
    {
        #region oldreversecode
        /*
        if (!isReversing)
        {
            if (tunnelPieces.Count > 0) return;


            isReversing = true;
            PhylloTunnelPiece[] tempArray = tunnelPieces.ToArray();
            System.Array.Reverse(tempArray);
            //tunnelPieces.Clear();
            //currentFirstObjectYPos = tempArray[0].gameObject.transform.position.y;
            Debug.Log("tempArray.length = " + tempArray.Length);

            for (int i = 0; i < tempArray.Length - 1; i++)
            {
                tunnelPieces.Dequeue();
                tunnelPieces.Enqueue(tempArray[i]);
            }



            isGoingDown = !isGoingDown;

            Debug.Log("Reversing Queue. isGoingDown = " + isGoingDown);

            isReversing = false;
        }
        */
        #endregion
        tunnelPieces.Reverse();
        isGoingDown = !isGoingDown;
    }
}


