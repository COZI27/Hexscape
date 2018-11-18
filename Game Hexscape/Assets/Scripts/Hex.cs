using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Hex : MonoBehaviour
{

    // what you see is what you get, basically the script that sits on all hexes... 
    // Hexes spawn asleep (so they cannot be broken) and awaken once the player collides with one of the hexes for that particular level.
    
   

    [SerializeField] public GameObject prefab;
    [SerializeField] public int destroyPoints;




    public bool isClickable = true;

    public DestroyState destroyType;
    public float destroyTime = 1f;



    public bool isSleeping = true; // Dictates whether the tile is clickable
    public Material disabledMaterial;
    public Material enabledMaterial;

    public bool isAlive = true;


    public MeshRenderer mesh;


    private bool hasBeenTouched;

    // A delegate used to signal to listening objects the death of this Hex 
    //      (it would be possible to add other events here, such as when the player enters the tile)
    public delegate void OnHexDeath();
    public event OnHexDeath onHexDeath;

    private void Awake()
    {
        hasBeenTouched = false;

        // sub so we know when we do exit (an exit now occours when the player touches a diffrent hex tile)
        PlayerController.instance.newHextouched += PlayerTouchedNewHex;
    }

    private void OnEnable()
    {
        hasBeenTouched = false;
        isAlive = true;

         mesh = GetComponent<MeshRenderer>();
      //  mesh.materials[1].color = mesh.materials[1].color + new Color(0, 0, 0, 1);




        DisableHex();

        if (destroyType == DestroyState.destroyOnExit)
        {
            mesh.materials[1].SetColor("_EmissionColor", Color.green);
        }
    }


    public void EnableHex()
    {
        isSleeping = false;
        // mesh.materials[2] = enabledMaterial;

        //Debug.Log("AWAKEN");

    }

    public void DisableHex()
    {
        hasBeenTouched = false;
        isSleeping = true;
        //  mesh.materials[2] = disabledMaterial;


    }


    public void AwakenMap()
    {
        EndlessGameplayManager.instance.colourLerper.NextColour();

        foreach (Hex hex in transform.parent.GetComponentsInChildren<Hex>())
        {
            hex.EnableHex();
        }
    }




    public Hex(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public void DestroyHex()
    {
        hasBeenTouched = false;
        isAlive = false;

        // Broadcast delegate event
        if (onHexDeath != null) onHexDeath();

        //Destroy(gameObject, destroyTime);


        gameObject.SetActive(false);
        HexBank.instance.AddDisabledHex(gameObject);

        EndlessGameplayManager.instance.GainHexDigPoints(destroyPoints);
    }

    public void OnMouseClick()
    {
        if (isSleeping == false)
        {
            if (destroyType == DestroyState.destroyOnClick)
            {
                DestroyHex();
            }
        }

    }

    public void OnPlayerExit()
    {
        if (isSleeping == false)
        {
            if (destroyType == DestroyState.destroyOnExit)
            {
                DestroyHex();
            }
        }
    }

    public void OnPlayerEnter()
    {
       

        if (isSleeping)
        {

            AwakenMap();
            EndlessGameplayManager.instance.PlayGroundThud();

        }

        if (isSleeping == false)
        {
            if (destroyType == DestroyState.destroyOnEnter)
            {
                DestroyHex();
            }

            if (destroyType == DestroyState.destroyOnExit)
            {
                mesh.materials[1].SetColor("_EmissionColor", Color.red);
            }
        }
    }




    public void OnCollisionEnter(Collision collision)
    {

        PlayerController player = PlayerController.instance;
        
    
        if (player != null)
        {
            hasBeenTouched = true;
            OnPlayerEnter();
           
        }
    }

    public void PlayerTouchedNewHex (Hex newHex)
    {
        if (newHex == this || gameObject.activeInHierarchy == false || isSleeping) return;

        if (hasBeenTouched)
        {
            OnPlayerExit();
        }
     
    }

    public enum DestroyState
    {
        dontDestroy,
        destroyOnExit,
        destroyOnEnter,
        destroyOnClick
    }




}
