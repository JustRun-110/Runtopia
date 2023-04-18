using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class SpawnItem : MonoBehaviour
{
    [Header("Is Character State Item Used?")]
    public bool useItem;
    public bool isTree;
    public bool isCannon;

    [Header("Spawn")]
    private GameObject obj_itemPrefab = null;
    [SerializeField]
    private GameObject obj_treePrefab;
    [SerializeField]
    private GameObject obj_cannonPrefab;
    [SerializeField]
    private GameObject obj_item = null;
    private bool isItemUsed = false;

    [Header("Position")]
    [SerializeField]
    private Transform spawnPosition;
    [SerializeField]
    private Transform spawnPositionSky;
    private Transform currentSpawnPos;

    [Header("Photon")]
    [SerializeField]
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (useItem)
        {
            //UseItem();
            pv.RPC("UseItem", RpcTarget.All);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isItemUsed)
            {
                Debug.Log(useItem + "아이템을 배치합니다");
                obj_item.transform.SetParent(null);
                obj_item = null;
                isItemUsed = false;
            }
        }
    }

    [PunRPC]
    private void UseItem()
    {
        Debug.Log(useItem + "아이템을 사용합니다");

        if (isTree)
        {
            obj_itemPrefab = obj_treePrefab;

            if (!obj_itemPrefab) return;

            obj_item = PhotonNetwork.Instantiate(obj_itemPrefab.name, spawnPosition.position, spawnPosition.rotation);
            obj_item.transform.SetParent(spawnPosition);
        }
        else if (isCannon)
        {
            obj_itemPrefab = obj_cannonPrefab;

            if (!obj_itemPrefab) return;

            obj_item = PhotonNetwork.Instantiate(obj_itemPrefab.name, spawnPositionSky.position, spawnPositionSky.rotation);
            obj_item.transform.SetParent(spawnPositionSky);
        }

        useItem = false;
        isItemUsed = true;

        Debug.Log(useItem + "아이템을 생성합니다");

        ResetValue();
    }

    private void ResetValue()
    {
        obj_itemPrefab = null;
        isTree = false;
        isCannon = false;
    }
}
