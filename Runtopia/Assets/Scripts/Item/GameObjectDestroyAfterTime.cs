using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    float time = 1f;

    void Start()
    {
        Destroy(gameObject, time);
    }
}
