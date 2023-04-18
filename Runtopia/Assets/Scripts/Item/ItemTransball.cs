using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class ItemTransball : MonoBehaviour
{
    [Header("Is Character State Item Used?")]
    public bool transballItemUsed;
    public bool useFirstTransball = true;
    public bool useSecondTransball = false;

    [Header("transball")]
    [SerializeField]
    private GameObject obj_transball;
    private bool isFirstTransballUsed = false;
    private bool isSecondTransballUsed = false;
    private bool secondTransballWaiting = false;
    private GameObject obj_transballStart = null;
    private GameObject obj_transballEnd = null;
    private Vector3 transBallStart = Vector3.zero;
    private Vector3 transBallEnd = Vector3.zero;
    private int transballCount = 0;

    [Header("Position")]
    [SerializeField]
    private Transform spawnPosition = null;

    [Header("Photon")]
    [SerializeField]
    private PhotonView pv;

    [Tooltip("아이템 출력")]
    private GameObject itemUI;

    private void Start()
    {
        //spawnPosition = transform.GetChild(2).transform;
        pv = GetComponent<PhotonView>();

        //item출력위치 찾기
        if (GameObject.Find("ItemImage") != null)
        {
            itemUI = GameObject.Find("ItemImage");
        }
    }

    private void FixedUpdate()
    {
        if (transballItemUsed)
        {
            //TransballItem();
            pv.RPC("TransballItem", RpcTarget.All);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isFirstTransballUsed)
            {
                //PlaceTransball(obj_transballStart);
                pv.RPC("PlaceTransball", RpcTarget.All, "start");

                isFirstTransballUsed = false;
                secondTransballWaiting = true;
            }
            else if (isSecondTransballUsed)
            {
                //PlaceTransball(obj_transballEnd);
                pv.RPC("PlaceTransball", RpcTarget.All, "end");

                isSecondTransballUsed = false;
            }
        }
    }

    [PunRPC]
    private void TransballItem()
    {
        // 트렌스볼 아이템을 사용했고, 아직 모든 트렌스볼 아이템을 사용하기 전이라면
        //if (transballItemUsed)
        //{
            if (useFirstTransball)
            {
                //obj_transballStart = Instantiate(obj_transball, spawnPosition);
                obj_transballStart = PhotonNetwork.Instantiate(obj_transball.name, spawnPosition.position, spawnPosition.rotation);
                obj_transballStart.transform.SetParent(spawnPosition);
                useFirstTransball = false;
                isFirstTransballUsed = true;
            }
            else if (useSecondTransball)
            {
                //obj_transballEnd = Instantiate(obj_transball, spawnPosition);
                obj_transballEnd = PhotonNetwork.Instantiate(obj_transball.name, spawnPosition.position, spawnPosition.rotation);
                obj_transballEnd.transform.SetParent(spawnPosition);
                useSecondTransball = false;
                isSecondTransballUsed = true;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if (secondTransballWaiting)
                {
                    secondTransballWaiting = false;
                    useSecondTransball = true;
                }
            }
        //}


        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    if (isFirstTransballUsed)
        //    {
        //        PlaceTransball(obj_transballStart);
        //        //pv.RPC("PlaceTransball", RpcTarget.All, obj_transballStart);

        //        isFirstTransballUsed = false;
        //        secondTransballWaiting = true;
        //    }
        //    else if (isSecondTransballUsed)
        //    {
        //        PlaceTransball(obj_transballEnd);
        //        //pv.RPC("PlaceTransball", RpcTarget.All, obj_transballStart);

        //        isSecondTransballUsed = false;
        //    }
        //}
    }

    [PunRPC]
    private void PlaceTransball(string str_obj)
    {
        GameObject transball = null;

        if(str_obj == "start")
        {
            transball = obj_transballStart;
        }
        else if(str_obj == "end")
        {
            transball = obj_transballEnd;
        }

        transball.transform.SetParent(null); // 플레이어로 부터 분리하기

        if (transBallStart == Vector3.zero)
        {
            Vector3 position = new Vector3( spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z + 2f);

            transBallStart = position;
        }
        else if (transBallEnd == Vector3.zero)
        {
            Vector3 position = new Vector3(spawnPosition.position.x , spawnPosition.position.y, spawnPosition.position.z + 2f);

            transBallEnd = position;

            obj_transballStart.gameObject.GetComponent<Transball>().setPosition(transBallEnd);
            obj_transballEnd.gameObject.GetComponent<Transball>().setPosition(transBallStart);
        }

        transballCount++;

        if (transballCount == 2) // 초기화
        {
            //키 입력을 통해 조작되는 불리언 값 초기화
            transballItemUsed = false;
            useFirstTransball = true;
            useSecondTransball = false;

            //코드를 통해 조작되는 값 초기화
            isFirstTransballUsed = false;
            isSecondTransballUsed = false;
            obj_transballStart = null;
            obj_transballEnd = null;

            transBallStart = Vector3.zero;
            transBallEnd = Vector3.zero;
            transballCount = 0;

            //ui 바꾸기
            itemUI.SetActive(false);
        }
    }
}
