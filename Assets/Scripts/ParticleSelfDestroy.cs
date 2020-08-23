using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour {

    public ParticleSystem emitter;
    float emitterDuration;

	// Use this for initialization
	void Start () {
        emitter = this.gameObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
    }

    private void Awake() {
        if (emitter != null) {
            emitterDuration = emitter.main.duration + emitter.main.startLifetime.constantMax;
        }
        else Debug.Log("emitter is null");
    }

    // Update is called once per frame
    void Update () {
        emitterDuration -= Time.deltaTime;

        if (emitterDuration <= 0) {
            Destroy(this.gameObject);
        }

	}
}
