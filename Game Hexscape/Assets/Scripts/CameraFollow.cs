using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    // simple, just follows the player ball with a bit of smothing on the x and z axis... bool if you want Y smoothing too.


    public bool smoothY;

    public Transform target;
    public Vector3 offset;

    public float smoothSpeed;

    [ContextMenu("SetOffset")]
    public void SetOffset ()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
       if (target == null)
        {
            return;
        }

        if (smoothY)
        {
            Vector3 endPos = target.position + offset;
            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);

            transform.position = currentPos;
        } else
        {
            Vector3 endPos = target.position + offset;
            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);
            currentPos.y = target.position.y + offset.y;

            transform.position = currentPos;
        }
       

    }

}
