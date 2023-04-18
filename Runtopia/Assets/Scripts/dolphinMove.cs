using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dolphinMove : MonoBehaviour
{

    public int speed;
    private Transform tf;

    private void Start() {
        tf = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(1, 0, 0).normalized * speed * Time.deltaTime;
        tf.position += move;
    }
}
