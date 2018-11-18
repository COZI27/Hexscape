using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateEffect : TileEffectBase {

    public float explosionRadius = 1f;


    // NOTE: Here is an example of how TriggerEffect could be bound to a different event on Hex, such as when the ball enters the hex
    //public override void Awake() {
    //    Debug.Log("PowerUpEffect Awake");
    //    Hex hexComp = this.gameObject.GetComponent<Hex>();
    //    if (hexComp) {
    //        hexComp.onPlayerEnter += TriggerEffect; 
    //    }
    //}

    public override void TriggerEffect() {
        base.TriggerEffect();
        Debug.Log("DetonateEffect Trigger Effect Called");

        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, explosionRadius);
       
        int i = 0;

        while (i < hitColliders.Length) {
            if (hitColliders[i].gameObject != this.gameObject) { // Do not run for this object
                if (hitColliders[i].gameObject.layer == LayerMask.NameToLayer("Hex")) {
                    Debug.Log("Col sphere found hex");
                    Hex hitHex = hitColliders[i].gameObject.GetComponent<Hex>();
                    if (hitHex != null && hitHex.isAlive) hitHex.DestroyHex();
                }
            }
            i++;
        }
    }

}
