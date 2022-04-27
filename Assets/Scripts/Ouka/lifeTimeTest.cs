using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifeTimeTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update");
    }
}
