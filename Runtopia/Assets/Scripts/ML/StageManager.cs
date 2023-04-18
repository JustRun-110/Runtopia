using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject goodItem;
    public GameObject badItem;

    public int goodItemCount = 30;
    public int badItemCount = 10;

    public List<GameObject> goodList = new List<GameObject>();
    public List<GameObject> badList = new List<GameObject>();
    

    public void SetStageObject()
    {
        foreach (var obj in goodList)
        {
            Destroy(obj);
        }
        foreach (var obj in badList)
        {
            Destroy(obj);
        }
        
        // List 초기화
        goodList.Clear();
        badList.Clear();
        
        //good item 생성
        for (int i = 0; i < goodItemCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            
            goodList.Add(Instantiate(goodItem, transform.position + pos, rot, transform));
        }
        //bad item 생성
        for (int i = 0; i < badItemCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            
            badList.Add(Instantiate(badItem, transform.position + pos, rot, transform));
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
