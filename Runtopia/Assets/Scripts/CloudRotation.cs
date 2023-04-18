using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{

    //RT_Floor
    public float rotateSpeed;

    void Awake()
    {
    }
    void rotate()
    {
        transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);
    }
    
    void Update()
    {
        if (gameObject.tag == "Cloud")
        {
            rotate();
        }

    }
}
