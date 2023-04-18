using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaHorseController : MonoBehaviour
{
    public GameObject seaHorse;
    private SeaHorseMovemenat ds;
    private Transform tf;



    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "SeaHorse" || other.gameObject.tag == "Squid"){
            
            seaHorse = other.gameObject;

            ds = seaHorse.GetComponent<SeaHorseMovemenat>();
            tf = seaHorse.GetComponent<Transform>();


            if(ds.isLeft){
                seaHorse.transform.Rotate(new Vector3(tf.rotation.x, -180, tf.rotation.z));
                ds.isLeft = false;
            }
            else {
                seaHorse.transform.Rotate(new Vector3(tf.rotation.x, 180, tf.rotation.z));
                ds.isLeft = true;
            }
        } 
    }
}
