using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBurst : MonoBehaviour
{
    public float fuseTime = 10f;

    void Start() {
        Invoke("Explode", fuseTime);
    }
    
    void Explode() {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.main.duration);
    }
    
    void OnCollisionEnter(Collision coll) {
        Explode();
    }
}
