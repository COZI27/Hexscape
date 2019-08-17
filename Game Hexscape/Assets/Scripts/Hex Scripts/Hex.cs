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


    //  [SerializeField] public GameObject prefab;
    [SerializeField] public int destroyPoints;

    public UnityEvent clickedEvent = new UnityEvent();


    public bool isClickable = true;

    public HexTypeEnum typeOfHex;


    public float destroyTime = 1f;

    public ElementAttribute hexAttribute;


    public bool isSleeping = true; // Dictates whether the tile is clickable
    public Material disabledMaterial;
    public Material enabledMaterial;

    [HeaderAttribute("Tile Spawn Effect")]
    public bool useSpawnEffect;
    public bool useSpawnDelay; // Delays the spawn time based upon the tiles position
    public GameObject spawnParticleEffectToSpawn;
    private GameObject spawnParticleEffect;
    private float spawnEffectTimer = 0;
    private float spawnDelay = 0.0f;


    public bool isAlive = false;

    public MeshRenderer mesh;

    public bool useFalling = false;

    private bool hasBeenTouched;
    private float destroyTimer = 0;

    // A delegate used to signal to listening objects the death of this Hex 
    //      (it would be possible to add other events here, such as when the player enters the tile)
    public delegate void OnHexDeath();
    public event OnHexDeath onHexDeath;

    public delegate void OnHexDig();
    public event OnHexDig onHexDig;

    private int fallRotIndex = 0;
    private Vector2[] fallRotations;

    private Collider col; // so we can disable col when the hex falls so it wont bump player

    private void Awake()
    {
        hasBeenTouched = false;
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

    float touchTime;

    private void OnEnable()
    {

        touchTime = 0;

        if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
        {
            mesh.materials[0].SetColor("_EmissionColor", Color.black);
        }

        hasBeenTouched = false;
        isAlive = true;
        col.enabled = true;

        if (useSpawnEffect && spawnParticleEffect != null)
        {
            if (useSpawnDelay)
            {

                spawnDelay = Mathf.Abs(this.transform.position.x + this.transform.position.z) / 3;
                spawnParticleEffect.SetActive(false);
            }
            else spawnParticleEffect.SetActive(true);
        }
        else if (mesh != null) mesh.enabled = true;

        DisableHex();

        if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
        {
            mesh.materials[1].SetColor("_EmissionColor", Color.green);
        }
    }



    
   
    public void EnableHex()
    {
        isSleeping = false;
        incraseTouchTime = false;
        // mesh.materials[2] = enabledMaterial;

        //Debug.Log("AWAKEN");

        if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
        {
            mesh.materials[0].SetColor("_EmissionColor", Color.white);
        }
    }

    public void DisableHex()
    {
        
        hasBeenTouched = false;
        isSleeping = true;
        //  mesh.materials[2] = disabledMaterial;


    }

    // Triggers the rest of the board to wake up.
    public void AwakenMap()
    {
        //EndlessGameplayManager.instance.colourLerper.NextColour();

        //GameManager.instance.colourLerper.NectColour();

        GameManager.instance.BallLandEvent();

        foreach (Hex hex in transform.parent.GetComponentsInChildren<Hex>())
        {
            hex.EnableHex();
        }


    }



    
    public void DigHex(bool isANeighbourDeath = false) // make a dig hex function that gives points before calling this one
    {
        //onHexDig.Invoke();
        if (onHexDig != null)
            onHexDig();

        if (isAlive)
        {
            if (col != null)
                col.enabled = false;
            hasBeenTouched = false;
            isAlive = false;

            clickedEvent.RemoveAllListeners(); // TDOD: COnsider whether this would be better suited to being called when returned to the object pool
            if (GameManager.instance !=  null) GameManager.instance.DigEvent(destroyPoints);
        }

        if (useFalling)
        {
            fallRotIndex = Random.Range(0, fallRotations.Length - 1);
        }

    }

    public void FinishDestroy()
    {
        destroyTimer = 0;



        gameObject.SetActive(false);
        HexBank.Instance.AddDisabledHex(gameObject); // puts the hex back into the bank (hex object pool)


    }

    private void Update() // was fixed update before for some reason
    {
        if (isAlive)
        {

            HandleSpawningEffect();

            if (incraseTouchTime)
            {
                touchTime += Time.deltaTime;

            }



            if (typeOfHex == HexTypeEnum.HexTile_ClickIndestructible)
            {
                mesh.materials[1].SetColor("_EmissionColor", Color.Lerp(Color.magenta, Color.black, touchTime/destroyTime));

                if (touchTime >= destroyTime) DigHex();

            }
        }
        else
        {
            HandleDestructionTimer();
            HandleFallingEffect();
        }

        

    }

    // Handles the timers controling the enabling of the spawnParticleEffect and mesh  
    void HandleSpawningEffect()
    {
        if (isAlive)
        {
            if (!mesh.enabled)
            {
                spawnDelay -= Time.deltaTime;

                if (spawnDelay <= 0)
                {
                    spawnParticleEffect.SetActive(true);
                }

                if (spawnParticleEffect.activeInHierarchy)
                {
                    spawnEffectTimer += Time.deltaTime;
                    if (spawnEffectTimer >= 0.8f) // TODO: Replace hardcoding of timer
                    {
                        spawnEffectTimer = 0;
                        mesh.enabled = true;
                    }
                }
            }
        }
    }

    // Handles the pseudo physics of the falling effect for the tile, if falling is enabled
    void HandleFallingEffect()
    {
        if (useFalling)
        {
            this.transform.position += (Physics.gravity / 25);
            this.transform.Rotate(Vector3.right * Time.deltaTime * fallRotations[fallRotIndex].x);
            this.transform.Rotate(Vector3.forward * Time.deltaTime * fallRotations[fallRotIndex].y);
        }
    }

    // Handles the destruction timer for the tile, which when concluded will finalise the tiles destruction
    // also handles the visual effect of destroying the mesh before the conclusion of the timer in order to complete other visual effects such as particles
    void HandleDestructionTimer()
    {
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyTime - 0.7f) mesh.enabled = false; // TODO: Replace hardcoding of timer
        if (destroyTimer >= destroyTime) FinishDestroy();
    }

    public void OnMouseClick()
    {
        if (isAlive && !isSleeping) {
            GameManager.instance.ClickEvent();
        }

        //if (clickedEvent.GetPersistentEventCount() > 0) // For whatever reason this method does not work. https://forum.unity.com/threads/get-number-of-runtime-listeners.292537/
        clickedEvent.Invoke(); // Added to allow level components to register their own methods with hexes.

        if (isSleeping == false)
        {
            if (typeOfHex == HexTypeEnum.HexTile_ClickDestroy)
            {
                DigHex();
            }
        }

    }

    public void OnPlayerExit()
    {
        incraseTouchTime = false;
        if (isSleeping == false)
        {
            
            if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
            {
                Debug.Log("OnPlayerExit HexTile_ExitDestroy");
                DigHex();
            }
        }
    }

    private bool incraseTouchTime;

    public void OnPlayerEnter()
    {
        incraseTouchTime = true;

        if (isSleeping)
        {
            AwakenMap();
        }

        if (isSleeping == false)
        {
            //if (hexType == HexTypeEnum.destroyOnEnter)
            //{
            //    DestroyHex();
            //}

            if (typeOfHex == HexTypeEnum.HexTile_ExitDestroy)
            {
                mesh.materials[0].SetColor("_EmissionColor", Color.black);
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player") OnPlayerExit();
    }

    public void PlayerTouchedNewHex(Hex newHex)
    {
        if (newHex != null)
        {
            if (newHex == this || gameObject.activeInHierarchy == false || isSleeping) return;

            if (hasBeenTouched)
            {
                OnPlayerExit();
            }
        }
    }


}
