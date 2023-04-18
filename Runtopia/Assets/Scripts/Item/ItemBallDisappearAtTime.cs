using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBallDisappearAtTime : MonoBehaviour
{
    public float timeToDisappear = 3f;

    private void Start()
    {
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(timeToDisappear);

        Destroy(gameObject);
    }
}
