using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonFX : MonoBehaviour
{
    private AudioSource fx;

    private void Start() {
        fx = GetComponent<AudioSource>();
    }

    public void playPistonFX(){
        fx.Play();
    }
}
