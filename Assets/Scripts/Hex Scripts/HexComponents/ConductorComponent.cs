using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hold a charge and updates neighbouring hexes to change in charge
/// </summary>
public class ConductorComponent : BaseHexComponent, IChargeable
{

    private EChargeType currentChargeType;
    public EChargeType chargeType => currentChargeType;

    public bool isSource => false;

    bool isCharged = false;
    public int chargeValue { get => Convert.ToInt32(isCharged); set { if (value >= 1) isCharged = true; else isCharged = false;  }   }

    private void OnEnable()
    {
        Debug.Log("ConductorComponent::OnEnable");
        if (owningHex != null)
        {
            owningHex.enterEvent.AddListener(() =>
            {
                Debug.Log("ConductorComponent::OnEnable AddListener");
                OnEnterEventReceived();

            });

            //owningHex.exitEvent.AddListener(() =>
            //{
            //    OnExitEventReceived();
            //});
        }
    }

    private void OnEnterEventReceived()
    {
        Debug.Log("OnEnterEventRecieved");
        if (!isCharged)
        {
            Debug.Log("!isCharged");
            // Temporary solution
            if (FindPowerSource())
            {
                Debug.Log("FoundSource");
                isCharged = true;

                   HexMatComponent matComp = this.GetComponent<HexMatComponent>();
                    if (matComp != null)
                    {
                        matComp.SetEmissionColour(Color.yellow * 2);
                    }              
            }
        }

        // Temp solution taken from StackOverflow: 
        //https://stackoverflow.com/questions/55519679/pathfinding-algorithm-to-find-if-each-piece-is-connected-to-a-specific-object

        //TODO: implement new neighbour code using Vector2Int and MapSpawner dictionary

        //List<IChargeable> foundChargeInterfaces = new List<IChargeable>();


        //TODO:
        //  Find Connected Neighbours
        // If not charged, then receive charge from found neighbour
        // X  Check if source exists  - if false then uncharge all neighbours
        // Use a Breadth First Search?


        //if (!isCharged)
        //{
        //    List<IChargeable> foundNeighbours = GetNeighbourChargeInterfaces();
        //    if (foundNeighbours.Count > 0)
        //    {
        //        foreach (IChargeable c in foundNeighbours)
        //        {
        //            //found charged hex
        //            if (c.chargeValue > 0)
        //            {
        //                //TODO: Work out how to identify which charge should be used
        //                currentChargeType = c.chargeType;
        //                HexMatComponent matComp = this.GetComponent<HexMatComponent>();
        //                if (matComp != null)
        //                {
        //                    matComp.SetEmissionColour(Color.yellow * 2);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //... Do Nothing - no charged neighbour found
        //    }


        //}
        //else
        //{
        //    //... Do Nothing - already charged
        //}

        //else { // Needed here...?
        //    bool sourceFound = false;
        //    //GetNeighbourChargeInterfaces();

        //    foreach (IChargeable i in foundChargeInterfaces)
        //    {
        //        if (i.isSource) sourceFound = true;
        //    }


        //    if (sourceFound == false)
        //    {
        //        foreach (IChargeable i in foundChargeInterfaces)
        //        {
        //            i.RemoveCharge();
        //        }
        //    }

        //}


    }


    public bool FindPowerSource()
    {
        //Prep collections with this object's connections
        var checkedSet = new HashSet<IChargeable>();
        var remainingStack = new Stack<IChargeable>(GetNeighbourChargeInterfaces());

        //Are there items left to check?
        while (remainingStack.Count > 0)
        {
            //Reference the next item and remove it from remaining
            var item = remainingStack.Pop();
            IChargeable targetObjectScript = item;

            //If it's the source, we're done
            if (item.isSource)
            {

                // NOTE: Could get charge info from source here


                if (item.RequestCharge() != EChargeType.NoCharge) // Checking if charge is remaining
                {
                    currentChargeType = item.chargeType;
                    return true;
                }
                else return false;
            }
            else
            {
                //Otherwise, check for new items to evaluate
                //(You'll have to publicly expose collidedList for this)
                foreach (var newItem in targetObjectScript.GetNeighbourChargeInterfaces())
                {
                    //HashSet.Add returns true if it's added and false if it's already in there
                    if (checkedSet.Add(newItem))
                    {
                        //If it's new, make sure it's going to be evaluated
                        remainingStack.Push(newItem);
                    }
                }
            }
        }
        
        //NOTE: Need to RemoceCharge from neighbours here if no source is found? 
        foreach(IChargeable c in checkedSet)
        {
            c.RemoveCharge();
        }

        return false;
    }





    public override void CleanupComponent()
    {
        //throw new System.NotImplementedException();
    }

    public void ReceieveCharge(EChargeType chargeType)
    {
        throw new NotImplementedException();
    }

    public void RemoveCharge()
    {
        throw new NotImplementedException();
    }

    public List<IChargeable> GetNeighbourChargeInterfaces()
    {
        List<IChargeable> foundChargeInterfaces = new List<IChargeable>();

        Vector2Int pos = GridFinder.instance.WorldToGridPoint(this.transform.position);
        Hex[] hexNeighbours = GridFinder.instance.GetAllNeighbourHexs(pos, 1);

        foreach (Hex h in hexNeighbours)
        {

            IChargeable foundInterface = h.gameObject.GetComponent<IChargeable>();
            if (foundInterface != null)
            {
                foundChargeInterfaces.Add(foundInterface);
            }
        }


        return foundChargeInterfaces;
    }

    public EChargeType RequestCharge()
    {
        throw new NotImplementedException();
    }
}
