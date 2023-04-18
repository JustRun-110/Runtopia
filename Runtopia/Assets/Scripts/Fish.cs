using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public GameObject targetPosition;

    void Update()
    {
        transform.position = Vector3.Slerp(gameObject.transform.position, targetPosition.transform.position, 0.05f);
    }
}