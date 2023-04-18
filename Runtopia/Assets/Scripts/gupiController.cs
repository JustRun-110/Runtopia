using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gupiController : MonoBehaviour
{
    public GameObject gupi;
    private gupiMovement ds;
    private Transform tf;



    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Gupi") {
            gupi = other.gameObject;

            ds = gupi.GetComponent<gupiMovement>();
            tf = gupi.GetComponent<Transform>();


            if(ds.isRight){
                gupi.transform.Rotate(new Vector3(tf.rotation.x, -180, tf.rotation.z));
                ds.isRight = false;
            }
            else {
                gupi.transform.Rotate(new Vector3(tf.rotation.x, 180, tf.rotation.z));
                ds.isRight = true;
            }
        }
            
    }
}
