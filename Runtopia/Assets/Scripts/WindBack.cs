using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBack : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        Vector3 destination = new Vector3(other.transform.position.x + 0.03f, other.transform.position.y, other.transform.position.z);

        other.transform.position =
        Vector3.MoveTowards(other.transform.position, destination, 2);
    }
}
