using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // moves the player to the darget destination... currently set up so once the player moves to the target location they will stop
    public static PlayerController instance;

    // created a delegate that the player calles when they collide with a tile
    public delegate void CollideWithHexDeligate(Hex newHex);
    public CollideWithHexDeligate newHextouched;


    private float timeSinceLastTouchedHex;
        


    private Vector3 targetDestination;
    public float moveSpeed = 10;

    private Rigidbody rbody;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }  else
        {
            Destroy(gameObject);
        }
    }

    public void SetDestination(Vector3 destination)
    {
        targetDestination = destination;
    }

    private void MoveTo(Vector3 target)
    {
       // target.y = transform.position.y;

        
        Vector3 direction = ( new Vector3(target.x, target.y, target.z) - new Vector3( rbody.position.x, rbody.position.y, rbody.position.z)  ).normalized;
        Vector3 velocity = direction * moveSpeed * Time.deltaTime;

        rbody.velocity = (new Vector3(velocity.x, rbody.velocity.y, velocity.z));
    }

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();

        
    }
    private void Update()
    {
        timeSinceLastTouchedHex += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        
        MoveTo(targetDestination);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Hex hex = collision.gameObject.GetComponent<Hex>();
        if (hex != null)
        {
            timeSinceLastTouchedHex = 0;
            newHextouched.Invoke(hex);
        }
    
    }

    private void OnCollisionExit(Collision collision)
    {
        
        if (timeSinceLastTouchedHex > 0.1f) // to make sure we didn't quicky glitch untouch the hex i have added this timer
        {
            newHextouched.Invoke(null);
        }
            
       
    }
}
