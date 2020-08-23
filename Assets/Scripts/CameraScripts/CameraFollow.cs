using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    // simple, just follows the player ball with a bit of smothing on the x and z axis... bool if you want Y smoothing too.

    public static CameraFollow GetInstance()
    {
        if (instance == null) instance = FindObjectOfType<CameraFollow>();
        return instance;
   }

    private static CameraFollow instance = null;
    

    public bool smoothY;
    public bool snapToMapCentre = true;


    public Transform target;
    public Vector3 offset;

    Bounds mapBounds;

    public float smoothSpeed;

    float mapFitOffset;

    [ContextMenu("SetOffset")]
    public void SetOffset ()
    {
        offset = transform.position - target.position;
    }

    private void Start()
    {

        mapBounds = new Bounds();
    }
    private void Awake()
    {
        /*if (instance != null) Destroy(this.gameObject)*/;
       
    }



    void Update()
    {
        UpdateCameraBounds();
        
    }

    private void UpdateCameraBounds()
    {
        mapBounds.size = Vector3.zero; // reset
        Hex[] HexObjects = MapSpawner.Instance.GetCurrentGrid().GetComponentsInChildren<Hex>();
        // Debug.Log(HexObjects.Length);
        foreach (Hex hex in HexObjects)
        {
            mapBounds.Encapsulate(hex.GetComponent<Collider>().transform.position);
        }

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        //Debug.Log(aspectRatio);
        float tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);

        mapFitOffset = (mapBounds.extents.x / 2.0f / aspectRatio / tanFov);
        //Debug.Log(mapFitOffset);
    }

    private void LateUpdate()
    {
        Vector3 targetPos;
        if (snapToMapCentre == true) 
        {
            targetPos = MapSpawner.Instance.GetCurrentGrid().transform.position;

            if (target!= null)
            {
                targetPos.y = target.position.y;
            }

        } else
        {
            if (target == null)
            {
                return;
            }

            targetPos = target.position;
        }


       

        if (smoothY)
        {
            Vector3 endPos = targetPos + offset + Vector3.up* 2 * mapFitOffset;

            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);

            transform.position = currentPos;
        }
        else
        {
            Vector3 endPos = targetPos + offset;
            Vector3 currentPos = Vector3.Lerp(transform.position, endPos, smoothSpeed * Time.deltaTime);
            currentPos.y = targetPos.y + offset.y;

            transform.position = currentPos;
        }


    }

    public void TiltCamera(float angle, float duration)
    {
        StartCoroutine(TiltOverTime(angle, duration));
    }

    IEnumerator TiltOverTime(float targetAngle, float duration)
    {
        //float startRotation = transform.eulerAngles.x;
        //float endRotation = targetAngle;

        Vector3 startRot = transform.eulerAngles;
        Vector3 targetRot = new Vector3(targetAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            //float xRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            //transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y,
            //transform.eulerAngles.z);

            transform.eulerAngles = Vector3.Slerp(startRot, targetRot, t / duration);

            yield return null;
        }
    }

}
