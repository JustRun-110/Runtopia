using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [Header("Inventory")]
    public string currentItem = "";
    private bool isItemUsed = false;

    [Header("Item")]
    private string speed = "speed";
    private string jump = "jump";
    private string tree = "tree";
    private string cannon = "cannon";
    private string transball = "transball";

    [Tooltip("아이템 출력")]
    private GameObject itemUI;

    private UserControlThirdPerson inputs;
    public ItemTransball transballItem;
    public ItemStatus statusItem;
    public SpawnItem spawnItem;

    private void Start()
    {
        inputs = GetComponentInChildren<UserControlThirdPerson>();
        transballItem = GetComponent<ItemTransball>();
        statusItem = GetComponent<ItemStatus>();
        spawnItem = GetComponent<SpawnItem>();

        //item출력위치 찾기
        if (GameObject.Find("ItemImage") != null)
        {
            itemUI = GameObject.Find("ItemImage");
        }
    }

    private void Update()
    {
        if(inputs.state.useItem)
        {
            Debug.Log("사용해 "+ currentItem);

            if (currentItem.Equals("")) return;

            UseItem();

            currentItem = "";
        }
    }

    private void UseItem()
    {
        //itemUI.SetActive(false);
        
        if(currentItem.Equals(speed))
        {
            statusItem.speedItemUsed = true;
            itemUI.SetActive(false);
        }
        else if (currentItem.Equals(jump))
        {
            statusItem.jumpItemUsed = true;
            itemUI.SetActive(false);
        }
        else if (currentItem.Equals(tree))
        {
            spawnItem.isTree = true;
            spawnItem.useItem = true;
            itemUI.SetActive(false);
        }
        else if(currentItem.Equals(cannon))
        {
            spawnItem.isCannon = true;
            spawnItem.useItem = true;
            itemUI.SetActive(false);
        }
        else if(currentItem.Equals(transball))
        {
            transballItem.transballItemUsed = true;
        }

    }
}
