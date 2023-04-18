using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    void Update()
    {
         GameObject panda =  GameObject.Find("panda");
         if(panda.transform.position.y <= -20){
            panda.transform.position = new Vector3(1f,1f,1f);
         }
    }
}
