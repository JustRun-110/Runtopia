using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindLeft : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
            Vector3 destination = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z-0.1f);

            other.transform.position =
            Vector3.MoveTowards(other.transform.position, destination, 2);
        }
}
