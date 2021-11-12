using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationChange : MonoBehaviour
{
    public float speed = 1f;
    public int stateFlag = 1;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.Play("testPlay", 0, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stateFlag<0)
            {
                stateFlag = 1;
            }
            else
            {
                stateFlag = -2;
            }
            Debug.Log($" stateFlag[{stateFlag}]");
        }
        //animator.speed = stateFlag * speed;
        animator.SetFloat("Speed", stateFlag * speed);
        //animator.SetTrigger("dsada");
    }
}
