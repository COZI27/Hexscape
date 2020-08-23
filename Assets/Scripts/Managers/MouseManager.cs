using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    // The mouse manager sets up ripples, tells the player what direction to head in (these functions might be moved to the gameplay manager)


        // only destroys hex's once they are "awake" but will still move the player towards a hex, and will still play the ripple animation thingo


    [SerializeField] private LayerMask mouseMask = new LayerMask();
    [SerializeField] private PlayerController player = null;
    //[SerializeField] private RippleManager rippleManager = null;

   

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
                    //if (hex.isClickable && hex.readyState == Hex.HexReadyState.enabled || hex.readyState == Hex.HexReadyState.preThud) // now makes sure hex is sleepy - Why? Can't Hex handle that?
                   // {

                       //if (hex.isClickable) player.SetDestination(hex.transform.position);
                        hex.OnMouseClick();
                        //if (hex.hexAttributes.Count != 0 || hex.readyState != Hex.HexReadyState.preThud) hex.OnMouseClick(); // Just a temp little set up so you cant click regular hexes when set to prethud

                        RippleManager.instance.CreateRippleClick(hex.transform.position, 5f, 100);
                        //rippleManager.CreateRippleClick(hex.transform.position, 5f, 100);
                   // }
                   
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

                  //  Debug.Log(GridFinder.instance.WorldToGridPoint(hex.transform.position));
                    GridFinder.instance.origin = GridFinder.instance.WorldToGridPoint(hex.transform.position);
                }
            }
        }
    }
}
