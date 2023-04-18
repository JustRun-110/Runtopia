using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatController : MonoBehaviour
{

    GameObject boat;
    BoatMovement bm;
    [SerializeField] bool come;

    private void Start()
    {
        boat = GameObject.Find("SmallBoat");
        bm = boat.GetComponent<BoatMovement>();
        come = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Boat")
        {
            if (come)
            {
                bm.moveSpeed =  1;
            }
            else
            {
                bm.moveSpeed = 1;
            }

            come = !come;
        }
    }
}
