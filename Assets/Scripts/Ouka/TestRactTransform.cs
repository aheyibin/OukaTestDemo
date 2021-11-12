using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRactTransform : MonoBehaviour
{
    public float input;
    public RectTransform RectTransform;
    public Transform Transform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Transform
        RectTransform.sizeDelta = new Vector2( input,RectTransform.sizeDelta.y);
    }
}
