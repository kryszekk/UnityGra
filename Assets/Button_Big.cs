﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Big : MonoBehaviour
{
    Animator anim;
    public List<GameObject> OnPress = new List<GameObject>();
    public List<StandardActions> OnPressFunctions = new List<StandardActions>();
    public AudioClip button_press;
    AudioSource aSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }
    void Update()
    {
    }
    public void Use()
    {
        for (int i = 0; i < OnPress.Count; i++)
        {
            OnPress[i].BroadcastMessage(OnPressFunctions[i].ToString());
        }
        anim.Play("button_big_press");
        aSource.clip = button_press;
        aSource.Play();
    }
        void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("block"))
        {
            anim.SetBool("isButtonPressed", true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("block"))
        {
            anim.SetBool("isButtonPressed", false);
        }
    }
}

