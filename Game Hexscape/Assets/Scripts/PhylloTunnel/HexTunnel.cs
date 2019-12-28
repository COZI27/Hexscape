using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTunnel : MonoBehaviour
{

    [System.Serializable]
    public class HexRing
    {
        string trailObjectPrefabPath = "Prefabs/Particles/HexTunnelPiece";

        public HexRing(Vector3 worldPosition, int hexSize = 7)
        {
            this.worldPosition = worldPosition;
            this.hexSize = hexSize;

            trailObject = GameObject.Instantiate(Resources.Load(trailObjectPrefabPath) as GameObject);
            trailObject.transform.position = this.worldPosition;
            trailComponent = trailObject.transform.GetComponent<TrailRenderer>();
        }

        public Vector3 WorldPosition()
        {
            Vector3 returnPos = new Vector3(worldPosition.x, 0, worldPosition.y);
            return worldPosition;
        }

        public Vector3 worldPosition; // the world position of the centre of the cell.
        public int hexSize;

        GameObject trailObject;
        TrailRenderer trailComponent;

        public void SetTrailObjectPos(Vector2 position)
        {
            //TODO: Update Y pos

            trailObject.transform.position = new Vector3(position.x, 
                                                        trailObject.transform.position.y, 
                                                        position.y
                                                        );

            //trailObject.transform.position = new Vector3(Random.Range(-10, 10), trailObject.transform.position.y, Random.Range(-10, 10));
            //trailObject.transform.position = position;

        }

        public IEnumerator ResetTrailRenderer()
        {

            trailComponent.AddPosition(trailObject.transform.position);
            //float trailTime = trailComponent.time;
            trailComponent.AddPosition(trailObject.transform.position + (new Vector3(0, 0.1f, 0) ));

            trailComponent.emitting = false;
            //trailComponent.time = 0;
            yield return null;
            //trailComponent.time = trailTime;
            trailComponent.emitting = true;
            //trailComponent.Clear();
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






    public int numberOfObjects = 1;
    public float distanceBetweenObjects = 30; // The Y distance between each object. 

    //public GameObject objectToSpawn;

    private Queue<HexRing> tunnelPieces;
    private float currentTopObjectYpos;
    private float currentBottomObjectYpos;

    private int currentItteration = 0;

    [Range(1, 6)]
    public int lastItteration = 4; // temp - for debugging and testing
    private bool reverseDirection = false;




    // Start is called before the first frame update
    void Start()
    {
        // mathewfalvai@gmail.com
        SpawnTunnelPieces();
    }

    // Update is called once per frame
    void Update()
    {
        DrawHexes();
    }

    void SpawnTunnelPieces()
    {
        tunnelPieces = new Queue<HexRing>();

        float initialYPos = this.transform.position.y;
        currentTopObjectYpos = initialYPos;

        for (int i = 0; i < numberOfObjects; ++i)
        {


            tunnelPieces.Enqueue(new HexRing(new Vector3(0,initialYPos, 0)));
            //GameObject newObject = Instantiate(objectToSpawn, new Vector3(this.transform.position.x, initialYPos, this.transform.position.z), Quaternion.identity, GameManager.instance.transform) as GameObject;
            //if (newObject != null)
            //{
            //         script = newObject.GetComponent<PhylloTunnelPiece>();
            //    if (script != null)
            //    {
            //        tunnelPieces.Enqueue(script);
            //        script.targetYPos = initialYPos;
            //        currentBottomObjectYpos = initialYPos;
            //    }
            //    //else Destroy(newObject);
            //}

            initialYPos -= distanceBetweenObjects;
        }
    }

    private void DrawHexes()
    {
        // TODO: Draw here or in individual objects?
        // 

        foreach (HexRing cell in tunnelPieces)
        {
            cell.SetTrailObjectPos(GetHexCornerVertical(cell.worldPosition, cell.hexSize, currentItteration));
            if (currentItteration >= lastItteration || currentItteration == 0)
            {
                StartCoroutine(cell.ResetTrailRenderer());
            }
        }
        //if (currentItteration == lastItteration) currentItteration = 0;
        //currentItteration++;

        if (!reverseDirection)
        {
            if (currentItteration == lastItteration)
            {
                reverseDirection = !reverseDirection;
                currentItteration--;
            }
            else currentItteration++;
        }
        else if (reverseDirection)
        {
            if (currentItteration == 0)
            {
                reverseDirection = !reverseDirection;
                currentItteration++;
            }
            else currentItteration--;
        }

    }
}
