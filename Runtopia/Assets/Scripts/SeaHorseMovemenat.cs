using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaHorseMovemenat : MonoBehaviour
{
    public int speed;
    private Transform tf;
    public bool isLeft;

    private void Start() {
        tf = gameObject.GetComponent<Transform>();
        isLeft = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(1, 0, 0).normalized * speed * Time.deltaTime;

        if(isLeft)
            tf.position += move;
        else 
            tf.position -= move;
    }
}
