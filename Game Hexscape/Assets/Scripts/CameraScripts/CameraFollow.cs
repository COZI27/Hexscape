using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    // simple, just follows the player ball with a bit of smothing on the x and z axis... bool if you want Y smoothing too.


    public bool smoothY;

    public Transform target;
    public Vector3 offset;

    public float smoothSpeed;

    float mapFitOffset;

    [ContextMenu("SetOffset")]
    public void SetOffset ()
    {
        offset = transform.position - target.position;
    }


   

    void Update()
    {
        Bounds mapBounds = new Bounds();
        mapBounds.size = Vector3.zero; // reset
        Hex[] HexObjects = MapSpawner.instance.grid.GetComponentsInChildren<Hex>();
        // Debug.Log(HexObjects.Length);
        foreach (Hex hex in HexObjects)
        {
            mapBounds.Encapsulate(hex.GetComponent<Collider>().transform.position);
        }

        float aspectRatio = (float) Screen.width / (float) Screen.height;
        Debug.Log(aspectRatio);
        float tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);

         mapFitOffset = (mapBounds.extents.x / 2.0f / aspectRatio / tanFov);
        Debug.Log(mapFitOffset);
        
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Bounds mapBounds = new Bounds();



        mapBounds.size = Vector3.zero; // reset
        Hex[] HexObjects = MapSpawner.instance.grid.GetComponentsInChildren<Hex>();
        // Debug.Log(HexObjects.Length);
        foreach (Hex hex in HexObjects)
        {

            mapBounds.Encapsulate(hex.GetComponent<Collider>().transform.position);
        }





        if (smoothY)
        {
            Vector3 endPos = target.position + offset + Vector3.up* 2 * mapFitOffset;

            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);



            transform.position = currentPos;
        }
        else
        {
            Vector3 endPos = target.position + offset;
            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);
            currentPos.y = target.position.y + offset.y;

            transform.position = currentPos;
        }


    }

}
