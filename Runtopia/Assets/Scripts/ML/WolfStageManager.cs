using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WolfStageManager : MonoBehaviour
{
    public Transform currentTransform;
    public GameObject goodItem;

    public int goodItemCount = 30;

    public List<GameObject> goodList = new List<GameObject>();

    // Add new variables for constant movement
    public float movementSpeed = 5.0f;

    public void Start()
    {
        currentTransform = gameObject.transform;
    }

    public void SetStageObject()
    {
        foreach(var obj in goodList)
        {
            Destroy(obj);
        }
        
        // List initialization
        goodList.Clear();
        
        //create good item
        for (int i = 0; i < goodItemCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-28.0f, 28.0f), 2f, Random.Range(-28.0f, 28.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            
            goodList.Add(Instantiate(goodItem, transform.position + pos, rot, transform));
        }
    }

    // Add Update method to move goodItem objects forward at a constant speed
    // void Update()
    // {
    //     Transform currentTransform = gameObject.transform;
    //     
    //     foreach (GameObject obj in goodList)
    //     {
    //         if (obj == null) continue; // Skip if the object has been destroyed
    //
    //         // Move the object forward
    //         obj.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);
    //
    //         // Check if the object has reached the boundary around the WolfStageManager object
    //         float minX = currentTransform.position.x - 30.0f;
    //         float maxX = currentTransform.position.x +30.0f;
    //         float minZ = currentTransform.position.z -30.0f;
    //         float maxZ = currentTransform.position.z +30.0f;
    //
    //         if (obj.transform.position.x < minX || obj.transform.position.x > maxX ||
    //             obj.transform.position.z < minZ || obj.transform.position.z > maxZ)
    //         {
    //             // Move the object back to its starting position within the boundary
    //             Vector3 startPosition = transform.position + new Vector3(Random.Range(-30.0f, 30.0f), 2f, Random.Range(-30.0f, 30.0f));
    //             obj.transform.position = startPosition;
    //             obj.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    //         }
    //     }
    // }
}