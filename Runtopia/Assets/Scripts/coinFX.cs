using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinFX : MonoBehaviour
{

    private GameObject obj;
    [SerializeField] new AudioSource audio;
    private bool check;

    private void Start() {
        obj = GameObject.Find("coinSound");
        audio = obj.GetComponent<AudioSource>();
        check = false;
    }

    private void OnCollisionEnter(Collision other) {
        check = true;
        if(other.gameObject.tag == "Player" && check) {
            check = false;
            audio.Play();
        }
    }
}
