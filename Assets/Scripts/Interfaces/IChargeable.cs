using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EChargeType
{
    NoCharge,
    ChargeNeutral, // Neutralcharge is unique in that it can be used in place of any charge type
    ChargeAlpha,
    ChargeBeta
}


/// <summary>
/// Components that inherrit from this interface will receive Charge from neighbouring hexes
/// </summary>
public interface IChargeable
{
    int chargeValue { get; set; }

    EChargeType chargeType { get; }

    bool isSource { get; }

    List<IChargeable> GetNeighbourChargeInterfaces(); 

    void ReceieveCharge(EChargeType chargeType);

    EChargeType RequestCharge();

    void RemoveCharge();


}
