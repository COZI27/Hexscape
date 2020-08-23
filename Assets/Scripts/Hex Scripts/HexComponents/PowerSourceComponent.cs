using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSourceComponent : BaseHexComponent, IChargeable
{

    private EChargeType defaultChargeType;
    public EChargeType chargeType => defaultChargeType;

    public bool isSource => true;

    private int powerSourceCharge = 5;
    public int chargeValue { get => powerSourceCharge; set { } }



    public override void CleanupComponent()
    {
       
    }

    public void ReceieveCharge(EChargeType chargeType)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveCharge()
    {
        throw new System.NotImplementedException();
    }

    public List<IChargeable> GetNeighbourChargeInterfaces()
    {
        throw new System.NotImplementedException();
    }

    public EChargeType RequestCharge()
    {
        if (chargeValue > 1)
        {
            powerSourceCharge--;
            return chargeType;
        }
        else return EChargeType.NoCharge;
    }
}
