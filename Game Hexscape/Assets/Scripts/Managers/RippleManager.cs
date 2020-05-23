using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleManager : MonoBehaviour
{

    // Ripplez for days dad... Just effects the matterials that are in the ripple mats array, it sets all of their origins and distances creating ripples where/when the mouse manager say so.

    public static RippleManager instance;
    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    [SerializeField] private Material[] rippleMatsClick;

    [SerializeField] private Material[] rippleMatsBallThud;

    public float maxDistance = 10f;
    public float rippleSpeed = 3f;

    public float currentClickRippleDistance;
    public float currentThudRippleDistance;


    private Vector3 clickOrigin;
    private Vector3 thudOrigin;

    // Update is called once per frame

    private void Awake()
    {
        MakeSingleton();
    }

    void Update()
    {


       if (currentClickRippleDistance < maxDistance)
        {
            currentClickRippleDistance += rippleSpeed * Time.deltaTime;
        }


        foreach (Material mat in rippleMatsClick)
        {
            mat.SetFloat("_RippleDistance", currentClickRippleDistance);
            mat.SetFloat("_RippleRadius", currentThudRippleDistance);
            mat.SetVector("_RippleOrigin", (Vector4)clickOrigin);
        }



        if (currentThudRippleDistance < maxDistance)
        {
            currentThudRippleDistance += rippleSpeed * Time.deltaTime;
        }

        foreach (Material mat in rippleMatsBallThud)
        {
            mat.SetFloat("_RippleDistance", currentThudRippleDistance);
            mat.SetFloat("_RippleRadius", currentThudRippleDistance);
            mat.SetVector("_RippleOrigin", (Vector4)thudOrigin);
        }

    }


    public void CreateRippleClick (Vector3 origin, float speed, float maxDistance)
    {
        //Debug.Log("CreateRippleClick: " + origin + ", " + maxDistance);
        this.maxDistance = maxDistance;
        this.rippleSpeed = speed;
        this.clickOrigin = origin;


        currentClickRippleDistance = 0;
    }

    public void CreateRippleThud(Vector3 origin, float speed, float maxDistance)
    {
        //Debug.Log("CreateRippleThud");
        this.maxDistance = maxDistance;
        this.rippleSpeed = speed;
        this.thudOrigin = origin;

        currentThudRippleDistance = 0;
    }
}

