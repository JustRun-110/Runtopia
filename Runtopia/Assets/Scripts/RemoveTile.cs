using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTile : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(gameObject, .1f);
        }
    }
}
