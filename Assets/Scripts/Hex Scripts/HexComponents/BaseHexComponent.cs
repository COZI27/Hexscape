using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHexComponent : MonoBehaviour
{

    protected Hex owningHex;

    public abstract void CleanupComponent();


    private void Awake()
    {
        owningHex = this.GetComponent<Hex>();
    }

}
