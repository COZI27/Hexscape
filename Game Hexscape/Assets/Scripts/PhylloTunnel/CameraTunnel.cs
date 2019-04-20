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

    bool isGoingDown = true;

    float targetX = 0;
    float targetz = 0;
    int minmaxX = 4;
    bool isReversing;

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
                //else Destroy(newObject);
            }

            initialYPos -= distanceBetweenObjects;
        }
    }
	
	void Update () {
        HandleMoveTunnelDown();
    }

    // Automatically handles the movement of tunnel pieces in the Y axis. Intended to be called in the Update method.
    void HandleMoveTunnelDown() {

        if (!isReversing) {
            targetX += Time.deltaTime;
            targetz += Time.deltaTime;
        }
        else {
            targetX -= Time.deltaTime;
            targetz -= Time.deltaTime;
        }
        if (targetX > minmaxX || targetX < -minmaxX) isReversing = !isReversing;


        if (currentTopObjectYpos > this.transform.position.y) {

            PhylloTunnelPiece pieceToMove = tunnelPieces.Dequeue();
            pieceToMove.targetYPos = currentTopObjectYpos - (distanceBetweenObjects * (numberOfObjects - 1));
            pieceToMove.horizontalOffset = new Vector2(this.transform.position.x, this.transform.position.z);
            currentBottomObjectYpos = pieceToMove.targetYPos;
            currentTopObjectYpos -= distanceBetweenObjects;
            tunnelPieces.Enqueue(pieceToMove);
        }

    }
}
