using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffectBase : MonoBehaviour {

    delegate void TriggerDelegate();

    TriggerDelegate triggerDelegate;

    public GameObject particleToSpawn;

    public virtual void Awake() {
        Debug.Log("TileEffectBase Awake");
        Hex hexComp = this.gameObject.GetComponent<Hex>();
        if (hexComp) {
            hexComp.onHexDeath += TriggerEffect;
        }
    }

    public virtual void TriggerEffect() {
        Debug.Log("TileEffectBase Trigger Effect Called");
        if (particleToSpawn != null) {
            GameObject childObject = Instantiate(particleToSpawn) as GameObject;
            childObject.transform.position = this.gameObject.transform.position;
            childObject.transform.rotation = this.gameObject.transform.rotation;
            //childObject.transform.parent = this.gameObject.transform;
        }
        //Hex hexComp = this.gameObject.GetComponent<Hex>();
        //if (hexComp) {
        //    hexComp.onHexDeath -= TriggerEffect;
        //}
    }
}
