using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Photon.Pun;
using garden;

public class CallPunRPC : MonoBehaviour
{
    [SerializeField]
    PhotonView pv;
    [SerializeField]
    PlayerPickUpDrop ppd;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        pv = transform.root.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!pv.IsMine) return;
        if(UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            //Printer("나는 테스트용 메세지");
            pv.RPC("Printer", RpcTarget.All, "나는 테스트용 메세지");
        }
    }

    [PunRPC]
    public void Printer(string message)
    {
        Debug.Log(message);
        ppd.Grab();
    }
}
