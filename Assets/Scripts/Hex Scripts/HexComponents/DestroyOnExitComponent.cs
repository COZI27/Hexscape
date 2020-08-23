using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExitComponent : BaseHexComponent
{


    //int enterCount = 0;
    //int enterCountTilDig = 1;


    private void OnEnable()
    {
        if (owningHex != null)
        {
            owningHex.enterEvent.AddListener(() =>
            {

                OnEnterEventReceived();

            });

            owningHex.exitEvent.AddListener(() =>
            {
                OnExitEventReceived();
            });
        }
    }


    Vector3 foundPos;

    private void OnEnterEventReceived()
    {
        //++enterCount;

        Vector2Int pos = GridFinder.instance.WorldToGridPoint(this.transform.position);
      
         //foundPos = new Vector3(pos.x, MapSpawner.Instance.GetCurrentMapHolder().transform.position.y, pos.y);

        Vector3? foundPosVal = GridFinder.instance.GridPosToWorld(pos);
        if (foundPosVal.HasValue)
        {
            foundPos = foundPosVal.Value;
            foundPos.y = MapSpawner.Instance.GetCurrentMapHolder().transform.position.y;
        }

        //Debug.Log("EnteredHexPos = " + pos);

        //foreach (Hex hex in transform.parent.GetComponentsInChildren<Hex>())
        //{
        //    hex.ChangeEmissionColour(Color.blue);
        //}

        Hex[] hexNeighbours = GridFinder.instance.GetAllNeighbourHexs(pos, 1);

        //foreach (Hex h in HexNeighbours)
        //{
        //    h.ChangeEmissionColour(Color.red);
        //}

        if (hexNeighbours.Length <= 1) 
            StartCoroutine(RecheckNeighbourCondition());
    }

    private void OnExitEventReceived()
    {
        //if (enterCount >= enterCountTilDig)            
            owningHex.DigHex(true);      
    }


    // Performs a delay before checking condition - intended to bothdig the remaining two hexes in the correct order and to mitigate against incorrect condition check
    IEnumerator RecheckNeighbourCondition()
    {
        yield return new WaitForSeconds(0.5f);
        Vector2Int pos = GridFinder.instance.WorldToGridPoint(this.transform.position);
        Hex[] HexNeighbours = GridFinder.instance.GetAllNeighbourHexs(pos, 1);
        if (HexNeighbours.Length <= 1)
        {
            owningHex.DigHex(true);
        }
    }


    private void OnDrawGizmos()
    {
        //if (false)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(foundPos, 0.5f);


        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(this.transform.position, 0.3f);
        //}

        //Hex[] testNeighbours = GetAllNeighbourHexs(origin, rad, allowSkips);

        //    foreach (Hex hex in testNeighbours)
        //    {
        //        if (hex != null)
        //            Gizmos.DrawSphere(hex.transform.position, 0.3f);
        //    }
    }

    public override void CleanupComponent()
    {

    }
}
