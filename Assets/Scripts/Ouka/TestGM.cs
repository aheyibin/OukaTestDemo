using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGM : MonoBehaviour
{
    public GameObject gm;
    public Image img;
    public Sprite sp;
    public Animator animator;
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        TestFunc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestFunc()
    {
        var temp = img.GetComponent<Renderer>();
        Debug.Log(temp);
        animator.SetTrigger("left");
        btn.onClick.Invoke();
    }
}
