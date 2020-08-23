using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hex : MonoBehaviour
{

    // what you see is what you get, basically the script that sits on all hexes... 
    // Hexes spawn asleep (so they cannot be broken) and awaken once the player collides with one of the hexes for that particular level.

    public bool Flag = false;

    public void SetFlag(bool value)
    {
        Flag = value;
    }


    public UnityEvent clickedEvent = new UnityEvent();
    public UnityEvent enterEvent = new UnityEvent();
    public UnityEvent exitEvent = new UnityEvent();



    public delegate void OnHexDig();
    public event OnHexDig onHexDig;




    [HeaderAttribute("Tile Spawn Effect")]
    public bool useSpawnEffect;
    public bool useSpawnDelay; // Delays the spawn time based upon the tiles position
    public GameObject spawnParticleEffectToSpawn;
    private GameObject spawnParticleEffect;
    //private float spawnEffectTimer = 0;
    private float spawnDelay = 0.0f;

    private float destroyDelayTime = 1f;


    public MeshRenderer mesh;

    public bool useFalling = false;


    private int fallRotIndex = 0;
    private Vector2[] fallRotations;

    private Collider col; // so we can disable col when the hex falls so it wont bump player

  public  HexReadyState readyState = HexReadyState.preThud;

    public enum HexReadyState
    {
        preThud,
        disabled,
        enabled,
        destroyed,
    }



    private void Awake()
    {
        //hasBeenTouched = false;
        col = GetComponent<Collider>();

        // sub so we know when we do exit (an exit now occours when the player touches a diffrent hex tile)
        if (PlayerController.instance != null) PlayerController.instance.newHextouched += PlayerTouchedNewHex;

        mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        if (useSpawnEffect && spawnParticleEffectToSpawn != null)
        {
            spawnParticleEffect = Instantiate(spawnParticleEffectToSpawn);
            spawnParticleEffect.SetActive(false);
            spawnParticleEffect.transform.parent = this.gameObject.transform;
            spawnParticleEffect.transform.position = this.gameObject.transform.position;
            spawnParticleEffect.transform.rotation = this.gameObject.transform.rotation;
        }

        fallRotations = new Vector2[] { new Vector2(40, 60), new Vector2(60, 40), new Vector2(-40, -60), new Vector2(-60, -40) }; // Predefinign fall rotations to ensure that the tiles are side on when they despawn. May be replaced at a future point.
    }

    private void OnEnable()
    {     

        //hasBeenTouched = false;
       // isAlive = true;
        col.enabled = true;

        if (useSpawnEffect && spawnParticleEffect != null)
        {
            if (useSpawnDelay)
            {
                spawnDelay = Mathf.Abs(this.transform.position.x + this.transform.position.z) / 3;
                spawnParticleEffect.SetActive(false);
            }
            else spawnParticleEffect.SetActive(true);

            StartCoroutine(PlaySpawningEffect(spawnDelay));

        }
        else if (mesh != null) mesh.enabled = true;


        
        //hasBeenTouched = false;
        DisableHex();
        //isSleeping = false;
        readyState = HexReadyState.preThud;
    }

    private void Update() // was fixed update before for some reason
    {
        //if (readyState != HexReadyState.destroyed)
        //{
        //}
        //else
        // {
        //HandleDestructionTimer();
        //HandleFallingEffect();
        // }
    }


    public void EnableHex()
    {
        //isSleeping = false;
        if (readyState == HexReadyState.disabled || readyState == HexReadyState.preThud)
        {
            readyState = HexReadyState.enabled;
        }

    }

    public void DisableHex()
    {
        if (readyState == HexReadyState.enabled) readyState = HexReadyState.disabled;


    }

    // Triggers the rest of the board to wake up.
    public void AwakenMap() // TODO: Move to Map Spanwer..?
    {

        foreach (Hex hex in transform.parent.GetComponentsInChildren<Hex>())
        {
            hex.EnableHex();
        }

    }


    public void DigHex(bool awardPoints)
    {
        readyState = HexReadyState.destroyed;

        if (useFalling && isActiveAndEnabled) StartCoroutine(PlayFallingEffect());
        if (isActiveAndEnabled) StartCoroutine(StartDestroyTimer());


        if (onHexDig != null)
            onHexDig();

        if (col != null)
            col.enabled = false;
        RemoveListeners();
    }

    public void FinishDestroy()
    {
        gameObject.SetActive(false);
        HexBank.Instance.AddDisabledHex(gameObject); // puts the hex back into the bank (hex object pool)
    }

    private void RemoveListeners()
    {
        clickedEvent.RemoveAllListeners();
        enterEvent.RemoveAllListeners();
        exitEvent.RemoveAllListeners();
    }





    public void OnMouseClick()
    {
        if (readyState != HexReadyState.destroyed) {
            GameManager.instance.ClickEvent();

            if (readyState == HexReadyState.enabled || readyState == HexReadyState.preThud)
            {
              //  DigHex(true);
            }
        }
        clickedEvent.Invoke(); // Added to allow level components to register their own methods with hexes.
     
    }

    public void OnPlayerExit()
    {
        //incraseTouchTime = false;
        if (readyState == HexReadyState.enabled)
        {

            exitEvent.Invoke();
            
            //if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
            //{
            //    Debug.Log("OnPlayerExit HexTile_ExitDestroy");
            //    DigHex(true);
            //}
        }
    }

   
    
 
    public void OnCollisionEnter(Collision collision)
    {

        PlayerController player = PlayerController.instance;

        if (collision.gameObject.tag == "Player")
        {
            //hasBeenTouched = true;
            OnPlayerEnter();

        }
    }

    public void OnPlayerEnter()
    {
        //incraseTouchTime = true;

        if (readyState == HexReadyState.preThud)
        {
            readyState = HexReadyState.enabled;
            GameManager.instance.BallLandEvent();
            AwakenMap();
        }

        if (readyState == HexReadyState.enabled)
        {
            enterEvent.Invoke();
        }
    }




    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player") OnPlayerExit();
    }

    public void PlayerTouchedNewHex(Hex newHex) // Called when player touchs new hex
    {
        if (newHex != null)
        {
            if (newHex == this || gameObject.activeInHierarchy == false || readyState == HexReadyState.disabled) return;

            //if (hasBeenTouched)
                //OnPlayerExit();
        }
    }





    public void ChangeEmissionColour(Color newColour)
    {
        mesh.materials[1].SetColor("_EmissionColor", newColour);
    }



    #region Coroutines
    IEnumerator PlaySpawningEffect(float spawnDelay)
    {
        float startTime = Time.time;
        while (Time.time < startTime + spawnDelay)
        {
            yield return null;
        }
        spawnParticleEffect.SetActive(true);

        if (spawnParticleEffect.activeInHierarchy)
        {

            startTime = Time.time;
            while (Time.time < startTime + 0.8f)
            {
                yield return null;
            }
            mesh.enabled = true;
        }

        yield return null;

    }

    // Handles the timers controling the enabling of the spawnParticleEffect and mesh  
    //void HandleSpawningEffectOld()
    //{
    //    if (readyState != HexReadyState.destroyed)
    //    {
    //        if (!mesh.enabled)
    //        {
    //            spawnDelay -= Time.deltaTime;

    //            if (spawnDelay <= 0)
    //            {
    //                spawnParticleEffect.SetActive(true);
    //            }

    //            if (spawnParticleEffect.activeInHierarchy)
    //            {
    //                spawnEffectTimer += Time.deltaTime;
    //                if (spawnEffectTimer >= 0.8f) // TODO: Replace hardcoding of timer
    //                {
    //                    spawnEffectTimer = 0;
    //                    mesh.enabled = true;
    //                }
    //            }
    //        }
    //    }
    //}

    // Handles the falling effect for the tile if falling is enabled
    //void HandleFallingEffect()
    //{
    //    if (useFalling)
    //    {
    //        if (this.transform.position.y < GameManager.instance.GetPlayerBall().transform.position.y + 20) // +N here is used to prevent boards off screen falling
    //        {
    //            this.transform.position += (Physics.gravity / 15);
    //            this.transform.Rotate(Vector3.right * Time.deltaTime * fallRotations[fallRotIndex].x);
    //            this.transform.Rotate(Vector3.forward * Time.deltaTime * fallRotations[fallRotIndex].y);
    //        }
    //    }
    //}

    private IEnumerator PlayFallingEffect()
    {

        fallRotIndex = UnityEngine.Random.Range(0, fallRotations.Length - 1);

        float startTime = Time.time;
        while (Time.time < startTime + destroyDelayTime)
        {
            if (this.transform.position.y < GameManager.instance.GetPlayerBall().transform.position.y + 20) // +N here is used to prevent boards off screen falling
            {
                this.transform.position += (Physics.gravity / 15);
                this.transform.Rotate(Vector3.right * Time.deltaTime * fallRotations[fallRotIndex].x);
                this.transform.Rotate(Vector3.forward * Time.deltaTime * fallRotations[fallRotIndex].y);
            }
            yield return null;
        }

    }

    // Handles the destruction timer for the tile, which when concluded will finalise the tiles destruction
    // also handles the visual effect of destroying the mesh before the conclusion of the timer in order to complete other visual effects such as particles
    //void HandleDestructionTimer()
    //{
    //    destroyTimer += Time.deltaTime;
    //    if (destroyTimer >= 1 - 0.7f) mesh.enabled = false; // TODO: Replace hardcoding of timer
    //    if (destroyTimer >= 1) FinishDestroy();
    //}

    IEnumerator StartDestroyTimer()
    {
        float startTime = Time.time;
        while (Time.time < startTime + (destroyDelayTime / 2))
        {
            yield return null;
        }
        mesh.enabled = false;

        startTime = Time.time;
        while (Time.time < startTime + (destroyDelayTime / 2))
        {
            yield return null;
        }
        FinishDestroy();

        yield return null;

    }


    #endregion
}
