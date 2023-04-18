using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUp : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(!other.transform.root.GetChild(2))
            {
                return;
            }

            GameObject player = other.transform.root.GetChild(2).gameObject;

            Vector3 destination = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);

            player.transform.position =
            Vector3.MoveTowards(player.transform.position, destination, 2);
        }
    }
}
