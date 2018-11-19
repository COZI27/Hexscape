using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    

    // allows hexs to be placed with the number keys (1,2,3 work at the moment, all other keys will make a hex vanish)

        //  while not functions called from the level editor... you can press up or down to move between levels...
        // Press 'E' to toggle between edit or play mode... btw, i used a player prefs remember what mode you where on last, so you might need to press 'E' at the start of the game if you start in edit mode.
        // You can press 'L' to load the level at that 'level index'.
        // you can press 'S' to save a whats on screen to the level at the current 'level index'.
        // also the 'delete' key will delete all of the hexes on screen.
        
        // The level index is the index inside of the gameplay manager.... that index is for an Array of scriptable objects allowing us to make changes to levels with ease...

    public bool editLevel;


    // public Hex.DestroyState[] hexTypes = new Hex.DestroyState[10];


    public Grid grid;

    public LayerMask hexMask;

    public Vector3 worldMousePos;

    

    [ExecuteInEditMode]
    public void Update()
    {


        if (editLevel == false) return;

        Vector3 mousePos;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        if (plane.Raycast(mouseRay, out distance))   // outs the required distance
        {
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * distance, Color.red);
            mousePos = mouseRay.GetPoint(distance);

            Vector3Int targetGridPos = grid.WorldToCell(mousePos);
            Vector3 worldPos = grid.CellToWorld(targetGridPos);

            Debug.DrawRay(worldPos - Vector3.forward / 2, Vector3.forward, Color.magenta);
            Debug.DrawRay(worldPos - Vector3.right / 2, Vector3.right, Color.magenta);

           // Debug.Log(worldPos);

            worldMousePos = worldPos;

            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(i + ""))
                {




                    // remove old hex at position
                    if (Physics.OverlapSphere(worldPos, 0.2f, hexMask).Length > 0)
                    {

                        Hex oldHex = Physics.OverlapSphere(worldPos, 0.1f, hexMask)[0].GetComponent<Hex>();
                        Debug.Log(oldHex);

                        HexBank.instance.AddDisabledHex(oldHex.gameObject);
                        oldHex.gameObject.SetActive(false);
                    }

                    if (i != 0) // if its not null, place a hex at that location
                    {

                        Hex hexInstance = HexBank.instance.GetDisabledHex(HexBank.instance.GetTypeAtIndex(i - 1), (worldPos), grid.transform).GetComponent<Hex>();
                        
                       
                    }



                }
            }

        } else
        {
         
        }

       




    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.color = Color.blue;

        Gizmos.DrawCube(worldMousePos, Vector3.one/ 3);
    }


}
