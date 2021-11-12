using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class LoadAssetFile : MonoBehaviour
{
    GameObject go;
    Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        //toggle.onValueChanged.AddListener(Vhange)


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Vhange(bool b)
    {
        
    }
    public void Load()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Data/TestAssets.asset");

        var temp = GameObject.Instantiate(go);
        //temp.gameObject.transform.SetParent()
        //Debug.Log($"{loadasset.ints[0]} -- {loadasset.ints[4]} -- {loadasset.ints[9]}");
    }
}
