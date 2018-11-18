using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffectBase : MonoBehaviour {

    delegate void TriggerDelegate();

    TriggerDelegate triggerDelegate;

    public virtual void Awake() {
        Debug.Log("TileEffectBase Awake");
        Hex hexComp = this.gameObject.GetComponent<Hex>();
        if (hexComp) {
            hexComp.onHexDeath += TriggerEffect;
        }
    }

    public virtual void TriggerEffect() {
        Debug.Log("TileEffectBase Trigger Effect Called");
        //Hex hexComp = this.gameObject.GetComponent<Hex>();
        //if (hexComp) {
        //    hexComp.onHexDeath -= TriggerEffect;
        //}
    }
}
