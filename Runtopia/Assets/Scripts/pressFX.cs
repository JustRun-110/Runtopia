using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressFX : MonoBehaviour
{

    private AudioSource fx;

    private void Start() {
        // fx = GameObject.Find("Pressure PlatformFX").GetComponent<AudioSource>();
        fx = GetComponent<AudioSource>();
    }

    public void playPressFX(){
        fx.Play();
    }
}
