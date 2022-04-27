using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChange : MonoBehaviour
{
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat.SetColor("_Color",new Color(0f,1f,0,1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
