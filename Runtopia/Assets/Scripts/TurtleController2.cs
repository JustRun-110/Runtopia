using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController2 : MonoBehaviour
{
    public GameObject turtle;
    private TurtleMovement2 ds;
    private Transform tf;



    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Turtle") {
            turtle = other.gameObject;

            ds = turtle.GetComponent<TurtleMovement2>();
            tf = turtle.GetComponent<Transform>();


            if(ds.isRight){
                turtle.transform.Rotate(new Vector3(tf.rotation.x, -180, tf.rotation.z));
                ds.isRight = false;
            }
            else {
                turtle.transform.Rotate(new Vector3(tf.rotation.x, 180, tf.rotation.z));
                ds.isRight = true;
            }
        }
            
    }
}
