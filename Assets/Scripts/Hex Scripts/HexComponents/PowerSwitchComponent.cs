using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitchComponent : BaseHexComponent
{

    bool isPoweredState;

    Color poweredColour = new Color(1, 0.92f, 0.016f, 1);
    Color unpoweredColour = new Color(0.5f, 0.7f, 0.2f, 1);

    public PowerSwitchComponent()
    {

    }

    private void Awake()
    {
        ToggleHexColour();
    }


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


    private void OnEnterEventReceived()
    {
        isPoweredState = !isPoweredState;
        ToggleHexColour();
    }

    private void OnExitEventReceived()
    {

    }

    public override void CleanupComponent()
    {
        
    }


    private void ToggleHexColour()
    {
        if (owningHex != null)
        {       
            owningHex.ChangeEmissionColour((isPoweredState ? poweredColour : unpoweredColour));
        }
    }

}
