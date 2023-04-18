using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnding : MonoBehaviour
{

    public GameObject END_UI;


    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player") {
            END_UI.SetActive(true);
        }
    }
}
