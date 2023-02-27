using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.Play("Shoot");
        }
        if (Input.GetKey(KeyCode.R))
        {
            anim.SetBool("Taunt", true);
        }
        else
        {
            anim.SetBool("Taunt", false);
        }
    }
}
