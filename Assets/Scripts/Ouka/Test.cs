using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public Image image;
    public Sprite sprite;
    // Start is called before the first frame update
    void Start()
    {
        string str = "aaaaaaaa,bbbb,cccc";
        var temp = str.Split(";".ToCharArray());
        foreach (var item in temp)
        {
            Debug.Log(item);
        }

        //image.sprite
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeImage()
    {
        //this.GetComponent<Image>().overrideSprite = sprite;
        this.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("icon_role_filter");
    }
}
