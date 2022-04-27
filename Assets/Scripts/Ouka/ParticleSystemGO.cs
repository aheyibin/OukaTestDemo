using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemGO : MonoBehaviour
{
    public float intervaltime = 3f;

    public float currentTime;
    // Update is called once per frame
    public GameObject explosionPrefab;
    
    private void Start()
    {
        currentTime = 0;
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
            if (hit.collider.tag == "obstacle" && currentTime <= 0 )
            {
                Instantiate (explosionPrefab, hit.point, Quaternion.identity);
                currentTime = intervaltime;
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }
}
