using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileActive : MonoBehaviour
{

    public float disableTime;
    public float ableTime;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Disable", disableTime+6f, disableTime+ableTime + 3f);
        InvokeRepeating("Able", ableTime-3f, disableTime + ableTime + 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
    void Able()
    {
        gameObject.SetActive(true);
    }
}
