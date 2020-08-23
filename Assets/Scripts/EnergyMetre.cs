using UnityEngine;
using UnityEngine.Events;

public class EnergyMetre
{

    public EnergyMetre(float initialEnergy = 90, float initalAcceleration = 0.5f, int initalEnergyTier = 1, float deteriorationRate = 0.05f)
    {
        initialEnergyLevel = initialEnergy;
        currentEnergyLevel = initialEnergy;
        currentAccelerationLevel = initalAcceleration;
        this.deteriorationRate = deteriorationRate;
        currentEnergyTier = initalEnergyTier;

        tierGraceValue = 0;
    }

    //public delegate void OnMaxEnergyReached();
    //public event OnMaxEnergyReached onMaxEnergyReached;

    public UnityEvent maxEnergyReached = new UnityEvent();

    public int currentEnergyTier { get; private set; }
    private float currentEnergyLevel;
    private float currentAccelerationLevel; 

    private float tierGraceValue;
    private const float tierUpGaceBufferAmount = 25f;
    private float initialEnergyLevel;

    private float deteriorationRate;

    private float peakDeteriorationExponent = 0.0f;

    private float energyPeak = 70;

    private int energyMax = 100;
    public int GetEnergyMax() { return energyMax; }


    public float GetCurrentEnergy() { return currentEnergyLevel; }


    public float GetCurrentEnergyNormalised()
    {
        return (currentEnergyLevel - 0) / (energyMax - 0);
    }

    public float GetBarFill()
    {
        float val = ((float)currentEnergyLevel) / ((float)energyMax);
        // Debug.Log("currentEnergyLevel: " + currentEnergyLevel + " / energyMax: " + energyMax + " = " + val );
        return (val);
    }




    // for accel value:  value needs to be more inert then energy value... increate the energy gained when heigh. 
    // rewards player for good continuous play... might add an "On Fire" state when at max greatly increasing the speed of progression.


    public void NextTier()
    {
        currentEnergyTier++;
        //tierGraceValue = tierUpGaceBufferAmount;
        //currentEnergyLevel %= energyMax;
        currentEnergyLevel = initialEnergyLevel;
        maxEnergyReached.Invoke();
        GameManager.instance.scoreUI.FlickerMultiplierArrow(true);
    }


    public void AddEnergy(float amountToAdd)
    {
        currentEnergyLevel += amountToAdd;
        if (currentEnergyLevel >= energyMax)
        {
            NextTier();
        }
    }

    public void DrainEmergy()
    {
        if (tierGraceValue >=0)
        {
            tierGraceValue -= deteriorationRate;
            return;
        }


        if (currentEnergyLevel > energyPeak)
        {
            currentEnergyLevel -= deteriorationRate + (Mathf.Pow(peakDeteriorationExponent, currentEnergyLevel - energyPeak) - peakDeteriorationExponent);
        }
        else currentEnergyLevel -= deteriorationRate;
    }

    //public void DrainEmergy()
    //{

    //    if (tierGraceValue >= 0)
    //    {
    //        tierGraceValue -= deteriorationRate;
    //    }
    //    else
    //    {
    //        currentEnergyLevel -= deteriorationRate;
    //    }

    //    if (tierGraceValue < 0)
    //    {
    //        currentEnergyLevel += tierGraceValue;
    //        tierGraceValue = 0;
    //    }
    //}

    private static float GetPow(float baseNum, float powNum)
    {
        float result = 1;

        for (int i = 0; i < powNum; i++)
            result = result * baseNum;

        return result;
    }



}



