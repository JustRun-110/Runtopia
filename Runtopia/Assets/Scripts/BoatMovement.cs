using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    float initPositionY;
    float initPositionZ;
    public float distance;
    public float turningPoint;
    public bool turnSwitch;
    public float moveSpeed;

    //public float rotateSpeed;

    void Awake()
    {
        if (gameObject.tag == "Boat")
        {
            initPositionZ = transform.position.z;
            turningPoint = initPositionZ - distance;
        }
    }


    void leftRight()
    {

        float currentPositionZ = transform.position.z;

        if (currentPositionZ >= initPositionZ + distance)
        {
            turnSwitch = false;
        }
        else if (currentPositionZ <= turningPoint)
        {
            turnSwitch = true;
        }

        if (turnSwitch)
        {
            transform.position = transform.position + new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = transform.position + new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime;
        }

    }


    void Update()
    {
        if (gameObject.tag == "Boat")
        {
            leftRight();
        }
    }
}
