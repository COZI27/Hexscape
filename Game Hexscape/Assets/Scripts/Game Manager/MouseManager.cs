using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    // The mouse manager sets up ripples, tells the player what direction to head in (these functions might be moved to the gameplay manager)


        // only destroys hex's once they are "awake" but will still move the player towards a hex, and will still play the ripple animation thingo


    [SerializeField] private LayerMask mouseMask;
    [SerializeField] private PlayerController player;
    [SerializeField] private RippleManager rippleManager;

   

    private float rayDistance = 100f;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance, mouseMask))
            {
                Hex hex = hit.collider.GetComponent<Hex>();

                if (hex != null)
                {
                    if (hex.isClickable)
                    {
                        player.SetDestination(hex.transform.position);
                        hex.OnMouseClick();

                        RippleManager.instance.CreateRippleClick(hex.transform.position, 5f, 100);
                        //rippleManager.CreateRippleClick(hex.transform.position, 5f, 100);
                    }
                   
                }
            }
        } else
        {
            // temp for test

            RaycastHit testHit;
            if (Physics.Raycast(ray, out testHit, rayDistance, mouseMask))
            {
                Hex hex = testHit.collider.GetComponent<Hex>();

                if (hex != null)
                {

                    //Debug.Log(GridFinder.instance.WorldToGridPoint(hex.transform.position));
                }
            }
        }
    }
}
