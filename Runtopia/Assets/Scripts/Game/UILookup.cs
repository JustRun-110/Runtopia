using garden;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookup : MonoBehaviour
{
    Transform cam = null;

    private void LateUpdate()
    {

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }
}
